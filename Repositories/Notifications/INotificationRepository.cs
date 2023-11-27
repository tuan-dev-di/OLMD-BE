using System;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Repositories.Base;
using OptimizingLastMile.Utils;

namespace OptimizingLastMile.Repositories.Notifications;

public interface INotificationRepository : IBaseRepository<NotificationLog>
{
    Task<Pagination<NotificationLog>> GetNotificationPaging(long receiverId, int pageNumber, int pageSize);
}
