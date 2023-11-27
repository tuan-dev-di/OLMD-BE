using System;
using AutoMapper;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Models.Response.AccountProfile;

namespace OptimizingLastMile.Profiles;

public class DriverProfileMapper : Profile
{
    public DriverProfileMapper()
    {
        CreateMap<DriverProfile, DriverDetailProfileResponse>();
    }
}

