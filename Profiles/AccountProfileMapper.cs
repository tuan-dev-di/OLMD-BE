using System;
using AutoMapper;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Models.Response.AccountProfile;
using OptimizingLastMile.Models.Response.Managers;

namespace OptimizingLastMile.Profiles;

public class AccountProfileMapper : Profile
{
    public AccountProfileMapper()
    {
        CreateMap<AccountProfile, ManagerProfileResponse>();
        CreateMap<AccountProfile, AccountDetailProfileResponse>();
    }
}

