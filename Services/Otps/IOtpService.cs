using System;
namespace OptimizingLastMile.Services.Otps;

public interface IOtpService
{
    Task<DateTime> SendOTP(string phoneOrMail);
    Task<string> VerifyOTP(string phoneOrMail, string otp);
}

