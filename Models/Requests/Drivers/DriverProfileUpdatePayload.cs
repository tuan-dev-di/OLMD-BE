using System;
using System.ComponentModel.DataAnnotations;

namespace OptimizingLastMile.Models.Requests.Drivers;

public class DriverProfileUpdatePayload
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; }

    [Required]
    public DateTime? BirthDay { get; set; }

    [Required]
    public string AvatarUrl { get; set; }

    [Required]
    [StringLength(200)]
    public string Province { get; set; }

    [Required]
    [StringLength(200)]
    public string District { get; set; }

    [Required]
    [StringLength(200)]
    public string Ward { get; set; }

    [Required]
    [StringLength(200)]
    public string Address { get; set; }

    [Required]
    [StringLength(11)]
    public string PhoneContact { get; set; }


    [Required]
    public string IdentificationCardFrontUrl { get; set; }

    [Required]
    public string IdentificationCardBackUrl { get; set; }


    [Required]
    public string DrivingLicenseFrontUrl { get; set; }

    [Required]
    public string DrivingLicenseBackUrl { get; set; }


    [Required]
    public string VehicleRegistrationCertificateFrontUrl { get; set; }

    [Required]
    public string VehicleRegistrationCertificateBackUrl { get; set; }
}