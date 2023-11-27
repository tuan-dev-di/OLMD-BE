using AutoMapper;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Models.Response.Orders;

namespace OptimizingLastMile.Profiles.Resolvers;

public class OrderDriverDetailResolver : IValueResolver<OrderInformation, OrderDetailResponse, OrderActorResponse>
{
    public OrderActorResponse Resolve(OrderInformation source,
        OrderDetailResponse destination,
        OrderActorResponse destMember,
        ResolutionContext context)
    {
        if (source is null)
        {
            return null;
        }

        var id = source.Driver.Id;
        var role = source.Driver.Role;
        string name = null;
        string phoneContact = null;

        if (source.Driver.DriverProfile is not null)
        {
            name = source.Driver.DriverProfile.Name;
            phoneContact = source.Driver.DriverProfile.PhoneContact;
        }

        var objectMap = new OrderActorResponse
        {
            Id = id,
            Role = role,
            Name = name,
            PhoneContact = phoneContact
        };

        return objectMap;
    }
}