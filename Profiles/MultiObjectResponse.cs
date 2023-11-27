using System;
using AutoMapper;
using OptimizingLastMile.Models.Commons;
using OptimizingLastMile.Utils;

namespace OptimizingLastMile.Profiles;

public class MultiObjectResponse : Profile
{
    public MultiObjectResponse()
    {
        CreateMap(typeof(Pagination<>), typeof(MultiObjectResponse<>));

    }
}