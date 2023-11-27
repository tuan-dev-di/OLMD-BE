using System;
using FirebaseAdmin.Auth;
using OptimizingLastMile.Models.Commons;

namespace OptimizingLastMile.Services.Firebases;

public interface IFirebaseService
{
    Task<GenericResult<string>> VerifyIdToken(string idToken);
    Task<GenericResult<UserRecord>> GetUserInfo(string uid);
}

