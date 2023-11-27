using System;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using OptimizingLastMile.Configure;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Entites.Enums;
using OptimizingLastMile.Models.Commons;
using OptimizingLastMile.Models.Requests.Auths;
using OptimizingLastMile.Models.Response.Auths;
using OptimizingLastMile.Services.Accounts;
using OptimizingLastMile.Services.Auths;
using OptimizingLastMile.Services.Firebases;
using OptimizingLastMile.Services.Otps;
using OptimizingLastMile.Utils;
using Twilio.Exceptions;

namespace OptimizingLastMile.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly string OTP_SERVICE_PENDDING_STATUS = "pending";
    private readonly string OTP_SERVICE_APPROVED_STATUS = "approved";
    private readonly string OTP_SERVICE_MAX_ATTEMPTS_REACHED_STATUS = "max_attempts_reached";


    private readonly IAuthService _authService;
    private readonly IAccountService _accountService;
    private readonly IOtpService _otpService;
    private readonly IFirebaseService _firebaseService;
    private readonly FeatureConfig _featureConfig;

    public AuthController(IAuthService authService,
        IAccountService accountService,
        IOtpService otpService,
        IFirebaseService firebaseService,
        IOptionsSnapshot<FeatureConfig> options)
    {
        this._authService = authService;
        this._accountService = accountService;
        this._otpService = otpService;
        this._firebaseService = firebaseService;
        this._featureConfig = options.Value;
    }

    [HttpPost("login/username")]
    public async Task<IActionResult> LoginByUsernamePassword([FromBody] LoginUsernamePayload payload)
    {
        var account = await _accountService.GetByUsername(payload.Username);

        if (account is null)
        {
            var error = Errors.Auth.UsernameNotExist();
            return BadRequest(EnvelopResponse.Error(error));
        }

        var checkStatus = _authService.CheckStatusActive(account.Status);
        if (checkStatus.IsFail)
        {
            return BadRequest(EnvelopResponse.Error(checkStatus.Error));
        }

        var isCorrectPass = _authService.IsCorrectPassword(payload.Password, account.Password);

        if (!isCorrectPass)
        {
            var error = Errors.Auth.PasswordIncorrect();
            return BadRequest(EnvelopResponse.Error(error));
        }

        var jwtToken = _authService.GenerateToken(account);
        var res = new JwtTokenResponse()
        {
            JwtToken = jwtToken
        };

        return Ok(EnvelopResponse.Ok(res));
    }

    [HttpPost("register/username")]
    public async Task<IActionResult> RegisterByUsername([FromBody] RegisterByUsernamePayload payload)
    {
        var result = await _accountService.RegisterByUsername(payload.Username, payload.Password, payload.Role);

        if (result.IsFail)
        {
            return BadRequest(EnvelopResponse.Error(result.Error));
        }

        var jwtToken = _authService.GenerateToken(result.Data);
        var res = new JwtTokenResponse()
        {
            JwtToken = jwtToken
        };

        return Ok(EnvelopResponse.Ok(res));
    }

    [HttpPost("phone/otp")]
    public async Task<IActionResult> RequestOtp([FromBody] RequestOtpPayload payload)
    {
        if (!_featureConfig.IsEnableOtp)
        {
            var error = Errors.Common.FeatureLock();
            return Ok(EnvelopResponse.Error(error));
        }

        var phoneNumber = MyTools.ConvertPhoneNumberGlobal(payload.PhoneNumber);

        try
        {
            var timeValidOtp = await _otpService.SendOTP(phoneNumber);

            return Ok(EnvelopResponse.Ok(new { OtpValidTime = timeValidOtp }));
        }
        catch (Exception ex)
        {
            var apiException = (ApiException)ex;
            if (apiException.Code == 60203)
            {
                var error = Errors.OTP.TooManyRequest();
                return BadRequest(EnvelopResponse.Error(error));
            }

            var errorUnknown = Errors.Common.UnknownError();
            return BadRequest(EnvelopResponse.Error(errorUnknown));
        }
    }

    [HttpPost("login/phone")]
    public async Task<IActionResult> LoginWithPhone([FromBody] LoginPhonePayload payload)
    {
        if (payload.Role != RoleEnum.DRIVER && payload.Role != RoleEnum.CUSTOMER)
        {
            var error = Errors.Common.MethodNotAllow();
            return BadRequest(EnvelopResponse.Error(error));
        }

        var phoneNumber = MyTools.ConvertPhoneNumberGlobal(payload.PhoneNumber);

        try
        {
            var status = await _otpService.VerifyOTP(phoneNumber, payload.Otp);

            if (status.Equals(OTP_SERVICE_PENDDING_STATUS))
            {
                var error = Errors.OTP.VerificationCodeIncorrect();
                return BadRequest(EnvelopResponse.Error(error));
            }
            else if (status.Equals(OTP_SERVICE_APPROVED_STATUS))
            {
                var account = await _accountService.GetByPhoneNumber(payload.PhoneNumber);

                if (account is not null)
                {
                    var checkStatus = _authService.CheckStatusActive(account.Status);
                    if (checkStatus.IsFail)
                    {
                        return BadRequest(EnvelopResponse.Error(checkStatus.Error));
                    }

                    var jwtToken = _authService.GenerateToken(account);
                    var res = new JwtTokenResponse()
                    {
                        JwtToken = jwtToken
                    };

                    return Ok(EnvelopResponse.Ok(res));
                }

                var newAcc = await _accountService.RegisterByPhonenumber(payload.PhoneNumber, payload.Role);

                var jwtForNewAcc = _authService.GenerateToken(newAcc);

                var resData = new JwtTokenResponse()
                {
                    JwtToken = jwtForNewAcc
                };

                return Ok(EnvelopResponse.Ok(resData));
            }
            else if (status.Equals(OTP_SERVICE_MAX_ATTEMPTS_REACHED_STATUS))
            {
                var error = Errors.OTP.TooManyVerify();
                return BadRequest(EnvelopResponse.Error(error));
            }

            var errorUnknown = Errors.Common.UnknownError();
            return BadRequest(EnvelopResponse.Error(errorUnknown));
        }
        catch (Exception ex)
        {
            var apiException = (ApiException)ex;
            if (apiException.Code == 60202)
            {
                var error = Errors.OTP.TooManyVerify();
                return BadRequest(EnvelopResponse.Error(error));
            }

            var errorOtpTimeOut = Errors.OTP.OtpTimeOut();
            return BadRequest(EnvelopResponse.Error(errorOtpTimeOut));
        }
    }

    [HttpPost("login/email")]
    public async Task<IActionResult> LoginByEmail([FromBody] LoginEmailPayload payload)
    {
        if (payload.Role != RoleEnum.DRIVER && payload.Role != RoleEnum.CUSTOMER)
        {
            var error = Errors.Common.MethodNotAllow();
            return BadRequest(EnvelopResponse.Error(error));
        }

        var isVerifyIdToken = await _firebaseService.VerifyIdToken(payload.AccessToken);
        if (isVerifyIdToken.IsFail)
        {
            return BadRequest(EnvelopResponse.Error(isVerifyIdToken.Error));
        }

        var userRecordResult = await _firebaseService.GetUserInfo(isVerifyIdToken.Data);
        if (userRecordResult.IsFail)
        {
            return BadRequest(EnvelopResponse.Error(userRecordResult.Error));
        }

        string email = userRecordResult.Data.Email;

        var account = await _accountService.GetByEmail(email);

        if (account is not null)
        {
            var checkStatus = _authService.CheckStatusActive(account.Status);
            if (checkStatus.IsFail)
            {
                return BadRequest(EnvelopResponse.Error(checkStatus.Error));
            }

            var jwtToken = _authService.GenerateToken(account);
            var res = new JwtTokenResponse()
            {
                JwtToken = jwtToken
            };

            return Ok(EnvelopResponse.Ok(res));
        }

        var newAcc = await _accountService.RegisterByEmail(email, payload.Role);

        var jwtForNewAcc = _authService.GenerateToken(newAcc);

        var resData = new JwtTokenResponse()
        {
            JwtToken = jwtForNewAcc
        };

        return Ok(EnvelopResponse.Ok(resData));
    }
}

