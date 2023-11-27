using System;
using System.Security.Principal;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Entites.Enums;
using OptimizingLastMile.Models.Commons;
using OptimizingLastMile.Models.Requests.AccountProfiles;
using OptimizingLastMile.Models.Requests.Drivers;
using OptimizingLastMile.Repositories.Accounts;
using Twilio.Types;

namespace OptimizingLastMile.Services.Accounts;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        this._accountRepository = accountRepository;
    }

    public async Task<Account> GetByUsername(string username)
    {
        return await _accountRepository.GetByUsername(username.Trim());
    }

    public async Task<Account> GetByPhoneNumber(string phoneNumber)
    {
        return await _accountRepository.GetByPhoneNumber(phoneNumber);
    }

    public async Task<Account> GetByEmail(string email)
    {
        return await _accountRepository.GetByEmail(email);
    }

    public async Task<GenericResult<Account>> CreateManagerAcc(string username,
        string password,
        string name,
        DateTime? birthDay,
        string province,
        string district,
        string ward,
        string address,
        string phoneContact)
    {
        var acc = await GetByUsername(username);

        if (acc is not null)
        {
            var error = Errors.Auth.UsernameAlreadyExist();
            return GenericResult<Account>.Fail(error);
        }

        var passEncrypt = BCrypt.Net.BCrypt.HashPassword(password.Trim());

        var newAcc = new Account(username, passEncrypt, RoleEnum.MANAGER, StatusEnum.ACTIVE);
        var createProfileResult = AccountProfile.Create(name, birthDay, province, district, ward, address, phoneContact);

        if (createProfileResult.IsFail)
        {
            return GenericResult<Account>.Fail(createProfileResult.Error);
        }

        newAcc.AccountProfile = createProfileResult.Data;

        _accountRepository.Create(newAcc);
        await _accountRepository.SaveAsync();

        return GenericResult<Account>.Ok(newAcc);
    }

    public async Task<GenericResult<Account>> RegisterByUsername(string username, string password, RoleEnum role)
    {
        var account = await GetByUsername(username);

        if (account is not null)
        {
            var error = Errors.Auth.UsernameAlreadyExist();
            return GenericResult<Account>.Fail(error);
        }

        if (role != RoleEnum.CUSTOMER && role != RoleEnum.DRIVER)
        {
            var error = Errors.Auth.RoleNotAllowRegisterByUsername();
            return GenericResult<Account>.Fail(error);
        }

        var passEncrypt = BCrypt.Net.BCrypt.HashPassword(password);

        var newAcc = new Account(username, passEncrypt, role, StatusEnum.NEW);

        _accountRepository.Create(newAcc);
        await _accountRepository.SaveAsync();

        return GenericResult<Account>.Ok(newAcc);
    }

    public async Task<Account> RegisterByPhonenumber(string phoneNumber, RoleEnum role)
    {
        var account = new Account(phoneNumber, role, StatusEnum.NEW, true);
        _accountRepository.Create(account);
        await _accountRepository.SaveAsync();

        return account;
    }

    public async Task<Account> RegisterByEmail(string email, RoleEnum role)
    {
        var account = new Account(email, role, StatusEnum.NEW, false);
        _accountRepository.Create(account);
        await _accountRepository.SaveAsync();

        return account;
    }

    public async Task<GenericResult> UpdateProfile(Account account, ProfileUpdatePayload payload)
    {
        if (account.Status == StatusEnum.INACTIVE)
        {
            var error = Errors.Auth.AccountIsDisable();
            return GenericResult.Fail(error);
        }

        if (account.AccountProfile is null)
        {
            // Create
            var createResult = AccountProfile.Create(payload.Name,
                payload.BirthDay,
                payload.Province,
                payload.District,
                payload.Ward,
                payload.Address,
                payload.PhoneContact);

            if (createResult.IsFail)
            {
                return GenericResult.Fail(createResult.Error);
            }

            var profile = createResult.Data;
            account.AccountProfile = profile;
        }
        else
        {
            // Update
            var profile = account.AccountProfile;

            profile.SetName(payload.Name);
            profile.SetBirthDay(payload.BirthDay);
            profile.SetProvince(payload.Province);
            profile.SetDistrict(payload.Province);
            profile.SetWard(payload.Ward);
            profile.SetAddress(payload.Address);
            profile.SetPhoneContact(payload.PhoneContact);
        }

        if (account.Status == StatusEnum.NEW)
        {
            account.Status = StatusEnum.ACTIVE;
        }

        await _accountRepository.SaveAsync();

        return GenericResult.Ok();
    }

    public async Task<GenericResult> UpdateDriverProfile(Account account, DriverProfileUpdatePayload payload)
    {
        if (account.Status == StatusEnum.INACTIVE)
        {
            var error = Errors.Auth.AccountIsDisable();
            return GenericResult.Fail(error);
        }

        if (account.Status == StatusEnum.REJECTED)
        {
            var error = Errors.Auth.AccountIsReject();
            return GenericResult.Fail(error);
        }

        if (account.DriverProfile is null)
        {
            // Create
            var createResult = DriverProfile.Create(payload.Name,
                payload.BirthDay.Value,
                payload.AvatarUrl,
                payload.Province,
                payload.District,
                payload.Ward,
                payload.Address,
                payload.PhoneContact,
                payload.IdentificationCardFrontUrl,
                payload.IdentificationCardBackUrl,
                payload.DrivingLicenseFrontUrl,
                payload.DrivingLicenseBackUrl,
                payload.VehicleRegistrationCertificateFrontUrl,
                payload.VehicleRegistrationCertificateBackUrl);

            if (createResult.IsFail)
            {
                return GenericResult.Fail(createResult.Error);
            }

            var driverProfile = createResult.Data;
            account.DriverProfile = driverProfile;
        }
        else
        {
            // Update
            var driverProfile = account.DriverProfile;

            driverProfile.SetName(payload.Name);
            driverProfile.SetBirthDay(payload.BirthDay.Value);
            driverProfile.SetAvatarUrl(payload.AvatarUrl);
            driverProfile.SetProvince(payload.Province);
            driverProfile.SetDistrict(payload.District);
            driverProfile.SetWard(payload.Ward);
            driverProfile.SetAddress(payload.Address);
            driverProfile.SetPhoneContact(payload.PhoneContact);
            driverProfile.SetIdentificationCardFrontUrl(payload.IdentificationCardFrontUrl);
            driverProfile.SetIdentificationCardBackUrl(payload.IdentificationCardBackUrl);
            driverProfile.SetDrivingLicenseFrontUrl(payload.DrivingLicenseFrontUrl);
            driverProfile.SetDrivingLicenseBackUrl(payload.DrivingLicenseBackUrl);
            driverProfile.SetVehicleRegistrationCertificateFrontUrl(payload.VehicleRegistrationCertificateFrontUrl);
            driverProfile.SetVehicleRegistrationCertificateBackUrl(payload.VehicleRegistrationCertificateBackUrl);
        }

        if (account.Status == StatusEnum.NEW)
        {
            account.Status = StatusEnum.PENDING_APPROVE;
        }

        await _accountRepository.SaveAsync();

        return GenericResult.Ok();
    }
}

