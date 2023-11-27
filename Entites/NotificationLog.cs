using System;
using OptimizingLastMile.Entites.Enums;

namespace OptimizingLastMile.Entites;

public class NotificationLog
{
    public Guid Id { get; set; }
    public NotificationTypeEnum NotificationType { get; set; }

    public bool IsRead { get; set; }

    public Guid? OrderId { get; set; }
    public long? DriverId { get; set; }

    public long? ReceiverId { get; set; }

    public DateTime CreatedDate { get; set; }

    public Account Driver { get; set; }
    public Account Receiver { get; set; }

    public string Content { get => BuildContent(); }

    private string BuildContent()
    {
        if (NotificationType == NotificationTypeEnum.ASSIGNED_ORDER)
        {
            return $"Bạn được chỉ định giao đơn hàng với id là: {OrderId}";
        }

        if (NotificationType == NotificationTypeEnum.NEW_DRIVER_REGISTRATION)
        {
            return $"Người có tên {Driver.DriverProfile.Name} đăng ký làm người vận chuyển mới";
        }

        if (NotificationType == NotificationTypeEnum.DELIVERY_ORDER_SUCCESSFUL)
        {
            if (Receiver.Role == RoleEnum.MANAGER)
            {
                return $"Đơn hàng {OrderId} đã giao thành công";
            }
            else if (Receiver.Role == RoleEnum.CUSTOMER)
            {
                return $"Đơn hàng {OrderId} đã giao thành công, mời bạn đánh giá";
            }
        }

        return string.Empty;
    }
}

