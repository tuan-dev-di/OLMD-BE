using System;
using AutoMapper;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Models.Response.Notifications;

namespace OptimizingLastMile.Profiles;

public class NotificationMapper : Profile
{
    public NotificationMapper()
    {
        CreateMap<NotificationLog, NotificationResponse>();
    }
}

