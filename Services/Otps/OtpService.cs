using System;
using Microsoft.Extensions.Options;
using OptimizingLastMile.Configure;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

namespace OptimizingLastMile.Services.Otps;

public class OtpService : IOtpService
{
    private readonly OTPConfig _config;

    public OtpService(IOptionsSnapshot<OTPConfig> options)
    {
        this._config = options.Value;
    }

    public async Task<DateTime> SendOTP(string phoneOrMail)
    {
        TwilioClient.Init(_config.TWILIO_ACCOUNT_SID, _config.TWILIO_AUTH_TOKEN);

        var verification = await VerificationResource.CreateAsync(
            to: phoneOrMail,
            channel: "sms",
            pathServiceSid: _config.PATH_SERVICE_SID,
            locale: "vi"
        );

        return verification.DateCreated.Value.AddMinutes(10);
    }

    public async Task<string> VerifyOTP(string phoneOrMail, string otp)
    {
        TwilioClient.Init(_config.TWILIO_ACCOUNT_SID, _config.TWILIO_AUTH_TOKEN);

        var verificationCheck = await VerificationCheckResource.CreateAsync(
            to: phoneOrMail,
            code: otp,
            pathServiceSid: _config.PATH_SERVICE_SID
        );

        return verificationCheck.Status;
    }
}

