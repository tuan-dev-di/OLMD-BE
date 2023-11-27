using System;
using AutoMapper;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Models.Requests.Feedbacks;

namespace OptimizingLastMile.Profiles;

public class FeedBackMapper : Profile
{
    public FeedBackMapper()
    {
        CreateMap<Feedback, FeedBackCreatePayload>().ReverseMap();
    }
}