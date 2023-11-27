using System;
using AutoMapper;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Models.Response.Orders;

namespace OptimizingLastMile.Profiles.Resolvers;

public class OrderOwnerDetailResolver : IValueResolver<OrderInformation, OrderDetailResponse, OrderActorResponse>
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

        var id = source.Owner.Id;
        var role = source.Owner.Role;
        string name = null;
        string phoneContact = null;

        if (source.Owner.AccountProfile is not null)
        {
            name = source.Owner.AccountProfile.Name;
            phoneContact = source.Owner.AccountProfile.PhoneContact;
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