using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Application.Admin.Properties.DTOs;

public class AdminPropertyQuery
{
    public PropertyStatus? Status { get; set; }

    public string? Search { get; set; }

    public string? Location { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}