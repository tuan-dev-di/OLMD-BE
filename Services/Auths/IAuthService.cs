using System;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Entites.Enums;
using OptimizingLastMile.Models.Commons;

namespace OptimizingLastMile.Services.Auths;

public interface IAuthService
{
    bool IsCorrectPassword(string password, string encryptPass);
    GenericResult CheckStatusActive(StatusEnum status);
    string GenerateToken(Account account);
}