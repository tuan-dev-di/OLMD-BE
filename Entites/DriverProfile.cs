using System;
using System.Net;
using System.Xml.Linq;
using OptimizingLastMile.Models.Commons;

namespace OptimizingLastMile.Entites;

public class DriverProfile
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public DateTime BirthDay { get; private set; }
    public string AvatarUrl { get; private set; }
    public string Province { get; private set; }
    public string District { get; private set; }
    public string Ward { get; private set; }
    public string Address { get; private set; }
    public string PhoneContact { get; private set; }

    public string IdentificationCardFrontUrl { get; private set; }
    public string IdentificationCardBackUrl { get; private set; }

    public string DrivingLicenseFrontUrl { get; private set; }
    public string DrivingLicenseBackUrl { get; private set; }

    public string VehicleRegistrationCertificateFrontUrl { get; private set; }
    public string VehicleRegistrationCertificateBackUrl { get; private set; }

    private DriverProfile(string name,
        DateTime birthDay,
        string avatarUrl,
        string province,
        string district,
        string ward,
        string address,
        string phoneContact,
        string identificationCardFrontUrl,
        string identificationCardBackUrl,
        string drivingLicenseFrontUrl,
        string drivingLicenseBackUrl,
        string vehicleRegistrationCertificateFrontUrl,
        string vehicleRegistrationCertificateBackUrl)
    {
        Name = name.Trim();
        BirthDay = birthDay;
        AvatarUrl = avatarUrl.Trim();
        Province = province.Trim();
        District = district.Trim();
        Ward = ward.Trim();
        Address = address.Trim();
        PhoneContact = phoneContact.Trim();
        IdentificationCardFrontUrl = identificationCardFrontUrl.Trim();
        IdentificationCardBackUrl = identificationCardBackUrl.Trim();
        DrivingLicenseFrontUrl = drivingLicenseFrontUrl.Trim();
        DrivingLicenseBackUrl = drivingLicenseBackUrl.Trim();
        VehicleRegistrationCertificateFrontUrl = vehicleRegistrationCertificateFrontUrl.Trim();
        VehicleRegistrationCertificateBackUrl = vehicleRegistrationCertificateBackUrl.Trim();
    }

    public static GenericResult<DriverProfile> Create(string name,
        DateTime birthDay,
        string avatarUrl,
        string province,
        string district,
        string ward,
        string address,
        string phoneContact,
        string identificationCardFrontUrl,
        string identificationCardBackUrl,
        string drivingLicenseFrontUrl,
        string drivingLicenseBackUrl,
        string vehicleRegistrationCertificateFrontUrl,
        string vehicleRegistrationCertificateBackUrl)
    {
        var validateBirthDay = IsBirthDayValid(birthDay);
        if (validateBirthDay.IsFail)
        {
            return GenericResult<DriverProfile>.Fail(validateBirthDay.Error);
        }

        return GenericResult<DriverProfile>.Ok(new(
            name,
            birthDay,
            avatarUrl,
            province,
            district,
            ward,
            address,
            phoneContact,
            identificationCardFrontUrl,
            identificationCardBackUrl,
            drivingLicenseFrontUrl,
            drivingLicenseBackUrl,
            vehicleRegistrationCertificateFrontUrl,
            vehicleRegistrationCertificateBackUrl));
    }


    private static GenericResult IsBirthDayValid(DateTime birthDay)
    {
        if (birthDay >= DateTime.UtcNow)
        {
            var error = Errors.AccountProfile.InvalidBirthDay();
            return GenericResult.Fail(error);
        }

        return GenericResult.Ok();
    }

    public void SetName(string name)
    {
        if (name is not null)
        {
            Name = name.Trim();
        }
    }

    public GenericResult SetBirthDay(DateTime birthDay)
    {
        var validateBirthDay = IsBirthDayValid(birthDay);
        if (validateBirthDay.IsFail)
        {
            return GenericResult.Fail(validateBirthDay.Error);
        }

        BirthDay = birthDay;

        return GenericResult.Ok();

    }

    public void SetProvince(string province)
    {
        if (province is not null)
        {
            Province = province.Trim();
        }
    }

    public void SetDistrict(string district)
    {
        if (district is not null)
        {
            District = district.Trim();
        }
    }

    public void SetWard(string ward)
    {
        if (ward is not null)
        {
            Ward = ward.Trim();
        }
    }

    public void SetAddress(string address)
    {
        if (address is not null)
        {
            Address = address.Trim();
        }
    }

    public void SetPhoneContact(string phoneContact)
    {
        if (phoneContact is not null)
        {
            PhoneContact = phoneContact.Trim();
        }
    }

    public void SetAvatarUrl(string avatarUrl)
    {
        if (avatarUrl is not null)
        {
            AvatarUrl = avatarUrl.Trim();
        }
    }

    public void SetIdentificationCardFrontUrl(string identificationCardFrontUrl)
    {
        if (identificationCardFrontUrl is not null)
        {
            IdentificationCardFrontUrl = identificationCardFrontUrl.Trim();
        }
    }

    public void SetIdentificationCardBackUrl(string identificationCardBackUrl)
    {
        if (identificationCardBackUrl is not null)
        {
            IdentificationCardBackUrl = identificationCardBackUrl.Trim();
        }
    }

    public void SetDrivingLicenseFrontUrl(string drivingLicenseFrontUrl)
    {
        if (drivingLicenseFrontUrl is not null)
        {
            DrivingLicenseFrontUrl = drivingLicenseFrontUrl.Trim();
        }
    }

    public void SetDrivingLicenseBackUrl(string drivingLicenseBackUrl)
    {
        if (drivingLicenseBackUrl is not null)
        {
            DrivingLicenseBackUrl = drivingLicenseBackUrl.Trim();
        }
    }

    public void SetVehicleRegistrationCertificateFrontUrl(string vehicleRegistrationCertificateFrontUrl)
    {
        if (vehicleRegistrationCertificateFrontUrl is not null)
        {
            VehicleRegistrationCertificateFrontUrl = vehicleRegistrationCertificateFrontUrl.Trim();
        }
    }


    public void SetVehicleRegistrationCertificateBackUrl(string vehicleRegistrationCertificateBackUrl)
    {
        if (vehicleRegistrationCertificateBackUrl is not null)
        {
            VehicleRegistrationCertificateBackUrl = vehicleRegistrationCertificateBackUrl.Trim();
        }
    }
}

