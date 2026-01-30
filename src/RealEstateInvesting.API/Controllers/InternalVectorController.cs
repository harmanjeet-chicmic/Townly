using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Infrastructure.VectorSearch;
using RealEstateInvesting.Application.Common.Interfaces;

namespace RealEstateInvesting.Api.Controllers;

[ApiController]
[Route("api/internal/vector")]
public class InternalVectorController : ControllerBase
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly PropertyVectorIndexer _indexer;

    public InternalVectorController(
        IPropertyRepository propertyRepository,
        PropertyVectorIndexer indexer)
    {
        _propertyRepository = propertyRepository;
        _indexer = indexer;
    }

    /// <summary>
    /// Manually sync all ACTIVE properties into vector DB
    /// </summary>
    [HttpPost("sync-properties")]
    public async Task<IActionResult> SyncActiveProperties()
    {
        var activeProperties = await _propertyRepository
            .GetByStatusAsync(PropertyStatus.Active);

        int count = 0;

        foreach (var property in activeProperties)
        {
            await _indexer.IndexAsync(property);
            count++;
        }

        return Ok(new
        {
            Message = "Vector sync completed",
            IndexedProperties = count
        });
    }
}
