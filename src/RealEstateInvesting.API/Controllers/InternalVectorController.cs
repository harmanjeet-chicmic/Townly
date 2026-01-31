using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Infrastructure.VectorSearch;

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

    [HttpPost("index-properties")]
    public async Task<IActionResult> IndexActiveProperties()
    {
        var properties =
            await _propertyRepository.GetByStatusAsync(PropertyStatus.Active);

        int indexed = 0;

        foreach (var property in properties)
        {
            await _indexer.IndexAsync(property);
            indexed++;
        }

        return Ok(new
        {
            Message = "Vector indexing completed",
            IndexedProperties = indexed
        });
    }
}
