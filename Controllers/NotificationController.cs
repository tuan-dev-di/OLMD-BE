using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OptimizingLastMile.Entites.Enums;
using OptimizingLastMile.Hubs;
using OptimizingLastMile.Models.Commons;
using OptimizingLastMile.Models.Params.Notifications;
using OptimizingLastMile.Models.Response.Notifications;
using OptimizingLastMile.Repositories.Accounts;
using OptimizingLastMile.Repositories.Notifications;
using OptimizingLastMile.Utils;
using static OptimizingLastMile.Models.Commons.Errors;

namespace OptimizingLastMile.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    // if change prefix -> need to update some where point to one thing
    private static readonly string PREFIX = "account";

    private readonly INotificationRepository _notificationRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IHubContext<NotificationHub> _notificationHub;
    private readonly IMapper _mapper;

    public NotificationController(INotificationRepository notificationRepository,
        IAccountRepository accountRepository,
        IHubContext<NotificationHub> notificationHub,
        IMapper mapper)
    {
        this._notificationRepository = notificationRepository;
        this._accountRepository = accountRepository;
        this._notificationHub = notificationHub;
        this._mapper = mapper;
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "CUSTOMER,DRIVER,MANAGER")]
    public async Task<IActionResult> MarkNotificationIsRead([FromRoute] Guid id)
    {
        var authorId = MyTools.GetUserOfRequest(User.Claims);

        var notification = await _notificationRepository.GetById(id);

        if (notification is null)
        {
            return NotFound();
        }

        if (authorId != notification.ReceiverId)
        {
            return Forbid();
        }

        notification.IsRead = true;
        await _notificationRepository.SaveAsync();

        return NoContent();
    }

    [HttpGet]
    [Authorize(Roles = "CUSTOMER,DRIVER,MANAGER")]
    public async Task<IActionResult> GetNotifications([FromQuery] NotificationParam param)
    {
        var authorId = MyTools.GetUserOfRequest(User.Claims);

        var notiPaging = await _notificationRepository.GetNotificationPaging(authorId, param.Page, param.Limit);

        var dataRes = _mapper.Map<MultiObjectResponse<NotificationResponse>>(notiPaging);

        return Ok(EnvelopResponse.Ok(dataRes));
    }

    [HttpGet("testnotification")]
    [Authorize(Roles = "MANAGER")]
    public async Task<IActionResult> TriggerNoti()
    {
        var authorId = MyTools.GetUserOfRequest(User.Claims);

        var notiToSent = new NotificationResponse
        {
            Id = Guid.NewGuid(),
            CreatedDate = DateTime.UtcNow,
            IsRead = false,
            NotificationType = NotificationTypeEnum.ASSIGNED_ORDER,
            Content = "test content"
        };

        // Sent notification
        var groupName = MyTools.GetGroupName(authorId);
        await _notificationHub.Clients.Group(groupName).SendAsync(GlobalConstant.WEBSOCKET_METHOD, notiToSent);

        return Ok();
    }
}

