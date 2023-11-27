using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OptimizingLastMile.Entites.Enums;
using OptimizingLastMile.Models.Commons;
using OptimizingLastMile.Models.Params.Managers;
using OptimizingLastMile.Models.Requests.Managers;
using OptimizingLastMile.Models.Response.Managers;
using OptimizingLastMile.Repositories.Accounts;
using OptimizingLastMile.Services.Accounts;
using OptimizingLastMile.Services.Auths;

namespace OptimizingLastMile.Controllers;

[ApiController]
[Route("api/managers")]
[Authorize(Roles = "ADMIN")]
public class ManagerController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IAccountService _accountService;
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public ManagerController(IAuthService authService,
        IAccountService accountService,
        IAccountRepository accountRepository,
        IMapper mapper)
    {
        this._authService = authService;
        this._accountService = accountService;
        this._accountRepository = accountRepository;
        this._mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateManagerAccount([FromBody] ManagerAccCreatePayload payload)
    {
        var result = await _accountService.CreateManagerAcc(payload.Username,
            payload.Password,
            payload.Name,
            payload.BirthDay,
            payload.Province,
            payload.District,
            payload.Ward,
            payload.Address,
            payload.PhoneContact);

        if (result.IsFail)
        {
            return BadRequest(EnvelopResponse.Error(result.Error));
        }

        var account = result.Data;

        return CreatedAtAction(nameof(GetManagerAccountById), routeValues: new { id = account.Id }, null);
    }

    [HttpGet]
    public async Task<IActionResult> GetListManagerAcc([FromQuery] ManagerAccParam param)
    {
        var paginationData = await _accountRepository.GetPaginationAccountIncludeProfile(param.Search, RoleEnum.MANAGER, param.Page, param.Limit);

        var accountList = paginationData.Data;

        var managerProfileList = accountList.Select(a =>
        {
            var managerProfile = _mapper.Map<ManagerProfileResponse>(a);
            _mapper.Map(a.AccountProfile, managerProfile);
            return managerProfile;
        }).ToList();

        var responseData = _mapper.Map<MultiObjectResponse<ManagerProfileResponse>>(paginationData);
        responseData.Data = managerProfileList;

        return Ok(EnvelopResponse.Ok(responseData));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetManagerAccountById([FromRoute] long id)
    {
        var account = await _accountRepository.GetByIdIncludeProfile(id);

        if (account.Role != RoleEnum.MANAGER)
        {
            return Forbid();
        }

        var dataRes = _mapper.Map<ManagerProfileResponse>(account);
        _mapper.Map(account.AccountProfile, dataRes);

        return Ok(EnvelopResponse.Ok(dataRes));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DisableManager([FromRoute] long id)
    {
        var account = await _accountRepository.GetAccountIncludeOrderCreatedShipping(id);

        if (account is null)
        {
            return NotFound();
        }

        if (account.Role != RoleEnum.MANAGER)
        {
            return Forbid();
        }

        var deactiveResult = account.DeactiveManager();

        if (deactiveResult.IsFail)
        {
            return BadRequest(EnvelopResponse.Error(deactiveResult.Error));
        }

        await _accountRepository.SaveAsync();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateManagerProfile([FromRoute] long id, ManagerProfilePayload payload)
    {
        var account = await _accountRepository.GetByIdIncludeProfile(id);

        if (account is null)
        {
            return NotFound();
        }

        if (account.Role != RoleEnum.MANAGER)
        {
            return Forbid();
        }

        var profile = account.AccountProfile;

        profile.SetName(payload.Name);
        profile.SetProvince(payload.Province);
        profile.SetDistrict(payload.District);
        profile.SetWard(payload.Ward);
        profile.SetAddress(payload.Address);
        profile.SetPhoneContact(payload.PhoneContact);

        var setBirthDayResult = profile.SetBirthDay(payload.BirthDay);

        if (setBirthDayResult.IsFail)
        {
            return BadRequest(EnvelopResponse.Error(setBirthDayResult.Error));
        }

        await _accountRepository.SaveAsync();

        return NoContent();
    }

}

