using System;
namespace OptimizingLastMile.Models.Response.AccountProfile;

public class DriverDetailProfileResponse
{
    public string Name { get; set; }
    public DateTime BirthDay { get; set; }
    public string AvatarUrl { get; set; }
    public string Province { get; set; }
    public string District { get; set; }
    public string Ward { get; set; }
    public string Address { get; set; }
    public string PhoneContact { get; set; }

    public string IdentificationCardFrontUrl { get; set; }
    public string IdentificationCardBackUrl { get; set; }

    public string DrivingLicenseFrontUrl { get; set; }
    public string DrivingLicenseBackUrl { get; set; }

    public string VehicleRegistrationCertificateFrontUrl { get; set; }
    public string VehicleRegistrationCertificateBackUrl { get; set; }
}