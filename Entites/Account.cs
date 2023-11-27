using System;
using OptimizingLastMile.Entites.Enums;
using OptimizingLastMile.Models.Commons;

namespace OptimizingLastMile.Entites;

public class Account
{
    public long Id { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public string Username { get; private set; }
    public string Password { get; private set; }
    public RoleEnum Role { get; private set; }
    public StatusEnum Status { get; set; }

    public AccountProfile AccountProfile { get; set; }
    public DriverProfile DriverProfile { get; set; }

    public int CountOrderShipping
    {
        get => OrderReceived.Count(o => o.CurrentOrderStatus == OrderStatusEnum.SHIPPING);
    }

    public int CountOrderCreatedShipping
    {
        get => OrderCreated.Count(o => o.CurrentOrderStatus == OrderStatusEnum.SHIPPING);
    }

    public int CountOwnershipOrderInProcess
    {
        get => OwnershipOrder.Count(o => o.CurrentOrderStatus != OrderStatusEnum.DELETED);
    }

    public List<OrderInformation> OrderReceived { get; set; }
    public List<OrderInformation> OrderCreated { get; set; }
    public List<OrderInformation> OwnershipOrder { get; set; }

    protected Account() { }

    public Account(string username, string password, RoleEnum role, StatusEnum status)
    {
        Username = username.Trim();
        Password = password;
        Role = role;
        Status = status;
    }

    public Account(string emailOrPhone, RoleEnum role, StatusEnum status, bool isPhone)
    {
        if (isPhone)
        {
            PhoneNumber = emailOrPhone;
        }
        else
        {
            Email = emailOrPhone;
        }
        Role = role;
        Status = status;
    }

    public GenericResult DeactiveDriver()
    {
        if (Role != RoleEnum.DRIVER)
        {
            var error = Errors.Common.MethodNotAllow();
            return GenericResult.Fail(error);
        }

        if (CountOrderShipping > 0)
        {
            var error = Errors.AccountProfile.NotDeactiveDriver();
            return GenericResult.Fail(error);
        }

        Status = StatusEnum.INACTIVE;
        return GenericResult.Ok();
    }

    public GenericResult DeactiveManager()
    {
        if (Role != RoleEnum.MANAGER)
        {
            var error = Errors.Common.MethodNotAllow();
            return GenericResult.Fail(error);
        }

        if (CountOrderCreatedShipping > 0)
        {
            var error = Errors.AccountProfile.NotDeactiveManager();
            return GenericResult.Fail(error);
        }

        Status = StatusEnum.INACTIVE;
        return GenericResult.Ok();
    }

    public GenericResult DeactiveCustomer()
    {
        if (Role != RoleEnum.CUSTOMER)
        {
            var error = Errors.Common.MethodNotAllow();
            return GenericResult.Fail(error);
        }

        if (CountOwnershipOrderInProcess > 0)
        {
            var error = Errors.AccountProfile.NotDeactiveCustomer();
            return GenericResult.Fail(error);
        }

        Status = StatusEnum.INACTIVE;
        return GenericResult.Ok();
    }
}

