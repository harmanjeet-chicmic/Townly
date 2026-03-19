using System.ComponentModel.DataAnnotations;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Admin.Kyc.DTOs;

public class UpdateKycStatusRequest
{
    [Required]
    [Range(2, 3, ErrorMessage = "Only 'Approved' or 'Rejected' statuses are allowed for updates.")]
    public KycStatus Status { get; set; }
    public string? Reason { get; set; }
}
