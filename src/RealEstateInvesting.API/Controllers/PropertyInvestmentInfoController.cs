using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Application.Properties.InvestmentInfo;

namespace RealEstateInvesting.API.Controllers.Properties;

[ApiController]
[Route("api/properties/{propertyId:guid}/investinfo")]
public class PropertyInvestmentInfoController : ControllerBase
{
    private readonly PropertyInvestmentInfoService _service;

    public PropertyInvestmentInfoController(
        PropertyInvestmentInfoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetInvestmentInfo(Guid propertyId)
    {
        var result = await _service.GetAsync(propertyId);
        return Ok(result);
    }
}
