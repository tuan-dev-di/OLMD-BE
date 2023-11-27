using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Entites.Enums;
using OptimizingLastMile.Hubs;
using OptimizingLastMile.Models.Commons;
using OptimizingLastMile.Models.Params.Drivers;
using OptimizingLastMile.Models.Requests.AccountProfiles;
using OptimizingLastMile.Models.Requests.Drivers;
using OptimizingLastMile.Models.Response.AccountProfile;
using OptimizingLastMile.Models.Response.Notifications;
using OptimizingLastMile.Repositories.Accounts;
using OptimizingLastMile.Repositories.Notifications;
using OptimizingLastMile.Services.Accounts;
using OptimizingLastMile.Utils;

namespace OptimizingLastMile.Controllers;

[ApiController]
[Route("api/account-profile")]
public class AccountProfileController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IAccountService _accountService;
    private readonly IHubContext<NotificationHub> _notificationHub;
    private readonly IMapper _mapper;

    public AccountProfileController(IAccountRepository accountRepository,
        INotificationRepository notificationRepository,
        IAccountService accountService,
        IHubContext<NotificationHub> notificationHub,
        IMapper mapper)
    {
        this._accountRepository = accountRepository;
        this._notificationRepository = notificationRepository;
        this._accountService = accountService;
        this._notificationHub = notificationHub;
        this._mapper = mapper;
    }

    [HttpGet("min")]
    [Authorize(Roles = "MANAGER")]
    public async Task<IActionResult> GetMinAccount([FromQuery] RoleEnum role)
    {
        if (role != RoleEnum.DRIVER && role != RoleEnum.CUSTOMER)
        {
            return BadRequest();
        }

        var accountMins = await _accountRepository.GetAccountMin(role);

        return Ok(EnvelopResponse.Ok(accountMins));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "ADMIN,MANAGER,CUSTOMER")]
    public async Task<IActionResult> GetDetailProfile([FromRoute] long id)
    {
        var authorId = MyTools.GetUserOfRequest(User.Claims);

        var account = await _accountRepository.GetByIdIncludeProfile(id);

        if (account is null)
        {
            return NotFound();
        }

        if (account.Id != authorId)
        {
            return Forbid();
        }

        if (account.Status == StatusEnum.INACTIVE)
        {
            var error = Errors.Auth.AccountIsDisable();
            return BadRequest(EnvelopResponse.Error(error));
        }

        var profileDetail = _mapper.Map<ProfileDetailResponse>(account);

        return Ok(EnvelopResponse.Ok(profileDetail));
    }

    [HttpGet("drivers/{id}")]
    [Authorize(Roles = "DRIVER,MANAGER")]
    public async Task<IActionResult> GetDetailDriverProfile([FromRoute] long id)
    {
        var authorId = MyTools.GetUserOfRequest(User.Claims);
        var authorRoleStr = MyTools.GetRoleOfAuthRequest(User.Claims);

        var authorRole = Enum.Parse<RoleEnum>(authorRoleStr);

        var account = await _accountRepository.GetByIdIncludeProfile(id);

        if (account is null)
        {
            return NotFound();
        }

        if (authorRole == RoleEnum.DRIVER && account.Id != authorId)
        {
            return Forbid();
        }

        if (account.Role != RoleEnum.DRIVER)
        {
            return Forbid();
        }

        if (account.Status == StatusEnum.INACTIVE)
        {
            var error = Errors.Auth.AccountIsDisable();
            return BadRequest(EnvelopResponse.Error(error));
        }

        if (account.Status == StatusEnum.REJECTED)
        {
            var error = Errors.Auth.AccountIsReject();
            return BadRequest(EnvelopResponse.Error(error));
        }

        var driverProfile = _mapper.Map<DriverProfileResponse>(account);

        return Ok(EnvelopResponse.Ok(driverProfile));
    }

    [HttpGet("drivers")]
    [Authorize(Roles = "MANAGER")]
    public async Task<IActionResult> GetManagerProfileList([FromQuery] DriverAccParam param)
    {
        var accPagination = await _accountRepository.GetPaginationAccountIncludeProfile(param.Search, RoleEnum.DRIVER, param.Page, param.Limit);

        var dataResponse = _mapper.Map<MultiObjectResponse<DriverProfileResponse>>(accPagination);

        return Ok(EnvelopResponse.Ok(dataResponse));
    }

    [HttpPut("drivers/{id}")]
    [Authorize(Roles = "DRIVER")]
    public async Task<IActionResult> UpdateDriverProfile([FromRoute] long id, [FromBody] DriverProfileUpdatePayload payload)
    {
        var authorId = MyTools.GetUserOfRequest(User.Claims);

        var account = await _accountRepository.GetByIdIncludeProfile(id);

        if (account is null)
        {
            return NotFound();
        }

        if (account.Id != authorId)
        {
            return Forbid();
        }

        var updateResult = await _accountService.UpdateDriverProfile(account, payload);

        if (updateResult.IsFail)
        {
            return BadRequest(EnvelopResponse.Error(updateResult.Error));
        }

        if (account.Status == StatusEnum.PENDING_APPROVE)
        {
            var listAllManagerActive = await _accountRepository.GetAccountsByRoleAndStatus(RoleEnum.MANAGER, StatusEnum.ACTIVE);
            var listNoti = new List<NotificationLog>();

            foreach (var manager in listAllManagerActive)
            {
                var noti = new NotificationLog
                {
                    NotificationType = NotificationTypeEnum.NEW_DRIVER_REGISTRATION,
                    IsRead = false,
                    DriverId = account.Id,
                    ReceiverId = manager.Id,
                    CreatedDate = DateTime.UtcNow
                };

                _notificationRepository.Create(noti);
                listNoti.Add(noti);
            }

            await _notificationRepository.SaveAsync();

            var notiToSent = _mapper.Map<List<NotificationResponse>>(listNoti);

            foreach (var noti in notiToSent)
            {
                var groupName = MyTools.GetGroupName(noti.ReceiverId.Value);

                var group = _notificationHub.Clients.Group(groupName);

                if (group is not null)
                {
                    await group.SendAsync(GlobalConstant.WEBSOCKET_METHOD, notiToSent);
                }
            }
        }

        return NoContent();
    }

    [HttpPut("drivers/{id}/status")]
    [Authorize(Roles = "MANAGER")]
    public async Task<IActionResult> UpdateDriverStatus([FromRoute] long id, [FromBody] DriverStatusUpdatePayload payload)
    {
        var driverAcc = await _accountRepository.GetAccountIncludeOrderShipping(id);

        if (driverAcc.Role != RoleEnum.DRIVER)
        {
            return Forbid();
        }

        switch (payload.Status)
        {
            case StatusEnum.ACTIVE:
                {
                    if (driverAcc.Status == StatusEnum.ACTIVE ||
                        driverAcc.Status == StatusEnum.INACTIVE ||
                        driverAcc.Status == StatusEnum.PENDING_APPROVE)
                    {
                        driverAcc.Status = StatusEnum.ACTIVE;
                        break;
                    }
                    var error = Errors.Common.MethodNotAllow();
                    return BadRequest(EnvelopResponse.Error(error));
                }
            case StatusEnum.INACTIVE:
                {
                    if (driverAcc.Status == StatusEnum.ACTIVE || driverAcc.Status == StatusEnum.INACTIVE)
                    {
                        var deactiveResult = driverAcc.DeactiveDriver();
                        if (deactiveResult.IsFail)
                        {
                            return BadRequest(EnvelopResponse.Error(deactiveResult.Error));
                        }
                        break;
                    }
                    var error = Errors.Common.MethodNotAllow();
                    return BadRequest(EnvelopResponse.Error(error));
                }
            case StatusEnum.REJECTED:
                {
                    if (driverAcc.Status == StatusEnum.PENDING_APPROVE)
                    {
                        driverAcc.Status = StatusEnum.REJECTED;
                        break;
                    }
                    var error = Errors.Common.MethodNotAllow();
                    return BadRequest(EnvelopResponse.Error(error));
                }
            default:
                {
                    var error = Errors.Common.MethodNotAllow();
                    return BadRequest(EnvelopResponse.Error(error));
                }
        }

        await _accountRepository.SaveAsync();

        return NoContent();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN,MANAGER,CUSTOMER")]
    public async Task<IActionResult> UpdateProfile([FromRoute] long id, ProfileUpdatePayload payload)
    {
        var authorId = MyTools.GetUserOfRequest(User.Claims);

        var account = await _accountRepository.GetByIdIncludeProfile(id);

        if (account is null)
        {
            return NotFound();
        }

        if (account.Id != authorId)
        {
            return Forbid();
        }

        var updateResult = await _accountService.UpdateProfile(account, payload);

        if (updateResult.IsFail)
        {
            return BadRequest(EnvelopResponse.Error(updateResult.Error));
        }

        return NoContent();
    }

    [HttpDelete("{id}/status")]
    [Authorize(Roles = "CUSTOMER")]
    public async Task<IActionResult> UpdateProfileStatus([FromRoute] long id)
    {
        var authorId = MyTools.GetUserOfRequest(User.Claims);
        var account = await _accountRepository.GetAccountIncludeOwnershipOrder(id);

        if (authorId != account.Id)
        {
            return Forbid();
        }

        var deactiveResult = account.DeactiveCustomer();

        if (deactiveResult.IsFail)
        {
            return BadRequest(EnvelopResponse.Error(deactiveResult.Error));
        }

        await _accountRepository.SaveAsync();

        return NoContent();
    }
}

