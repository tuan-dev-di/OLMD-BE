using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OptimizingLastMile.Models.Response.Notifications;
using OptimizingLastMile.Utils;

namespace OptimizingLastMile.Hubs;

[Authorize(Roles = "MANAGER")]
public class NotificationHub : Hub
{
    private static readonly Dictionary<string, string> GroupName = new Dictionary<string, string>();

    public override async Task OnConnectedAsync()
    {
        var authorId = MyTools.GetUserOfRequest(Context.User.Claims);

        var groupName = $"{GlobalConstant.PREFIX}-{authorId}";
        var connectionId = Context.ConnectionId;

        GroupName.Add(connectionId, groupName);

        await Groups.AddToGroupAsync(connectionId, groupName);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;

        if (GroupName.ContainsKey(connectionId))
        {
            GroupName.Remove(connectionId);
        }

        return base.OnDisconnectedAsync(exception);
    }
}