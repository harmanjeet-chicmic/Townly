using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;

namespace RealEstateInvesting.Infrastructure.VectorSearch
{
    public class PropertyVectorSyncJob
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly PropertyVectorIndexer _indexer;

        public PropertyVectorSyncJob(
            IPropertyRepository propertyRepository,
            PropertyVectorIndexer indexer)
        {
            _propertyRepository = propertyRepository;
            _indexer = indexer;
        }

        public async Task RunAsync()
        {
            var activeProperties = await _propertyRepository
                .GetByStatusAsync(PropertyStatus.Active);

            foreach (var property in activeProperties)
            {
                await _indexer.IndexAsync(property);
            }
        }
    }
}
