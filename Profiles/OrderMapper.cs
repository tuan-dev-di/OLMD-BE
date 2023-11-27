using System;
using AutoMapper;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Models.Requests.Orders;
using OptimizingLastMile.Models.Response.Orders;
using OptimizingLastMile.Profiles.Resolvers;

namespace OptimizingLastMile.Profiles;

public class OrderMapper : Profile
{
    public OrderMapper()
    {
        CreateMap<OrderInformation, OrderDetailResponse>()
            .ForMember(des => des.Owner, c => c.MapFrom<OrderOwnerDetailResolver>())
            .ForMember(des => des.Driver, c => c.MapFrom<OrderDriverDetailResolver>());

        CreateMap<OrderInformation, OrderUpdatePayload>().ReverseMap();

        CreateMap<OrderInformation, OrderResponse>()
            .ForMember(des => des.Owner, c => c.MapFrom<OrderOwnerResolver>())
            .ForMember(des => des.Driver, c => c.MapFrom<OrderDriverResolver>());
    }
}

