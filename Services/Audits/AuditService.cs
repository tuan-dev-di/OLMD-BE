using System;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Entites.Enums;
using OptimizingLastMile.Models.Response.Orders;

namespace OptimizingLastMile.Services.Audits;

public class AuditService : IAuditService
{
    public List<OrderHistoryResponse> BuildOrderHistory(OrderInformation order)
    {
        List<OrderHistoryResponse> result = new List<OrderHistoryResponse>();

        foreach (var audit in order.OrderAudits)
        {
            string content = "";

            switch (audit.OrderStatus)
            {
                case OrderStatusEnum.CREATED:
                    content = "Đơn hàng đã được ghi nhận bới người quản lý hệ thống";
                    break;
                case OrderStatusEnum.PROCESSING:
                    content = "Đơn hàng đang được xử lý";
                    break;
                case OrderStatusEnum.PICK_OFF:
                    content = "Đơn hàng đã được người vận chuyển nhận";
                    break;
                case OrderStatusEnum.SHIPPING:
                    content = "Đơn hàng đang được vẫn chuyển tới bạn";
                    break;
                case OrderStatusEnum.DELIVERED:
                    content = "Đơn hàng đã chuyển đến nơi";
                    break;
                case OrderStatusEnum.DELIVERY_FAILED:
                    content = $"Đơn hàng vận chuyển không thành công với lý do: {audit.Description}";
                    break;
                case OrderStatusEnum.DELETED:
                    content = "Đơn hàng đã bị hủy bỏ bới người quản lý hệ thống";
                    break;
                default:
                    break;
            }

            var line = new OrderHistoryResponse
            {
                TimeEvent = audit.CreatedDate,
                Content = content
            };

            result.Add(line);
        }

        return result;
    }
}