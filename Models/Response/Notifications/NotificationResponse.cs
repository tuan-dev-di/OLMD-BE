using System;
using OptimizingLastMile.Entites.Enums;

namespace OptimizingLastMile.Models.Response.Notifications;

public class NotificationResponse
{
    public Guid Id { get; set; }
    public NotificationTypeEnum NotificationType { get; set; }

    public bool IsRead { get; set; }

    public long? ReceiverId { get; set; }

    public DateTime CreatedDate { get; set; }

    public string Content { get; set; }
}