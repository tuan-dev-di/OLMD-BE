using System;
using OptimizingLastMile.Configs;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Repositories.Base;

namespace OptimizingLastMile.Repositories.FeedBacks;

public class FeedBackRepository : BaseRepository<Feedback>, IFeedBackRepository
{
    private readonly OlmDbContext _dbContext;

    public FeedBackRepository(OlmDbContext dbContext) : base(dbContext)
    {
        this._dbContext = dbContext;
    }
}

