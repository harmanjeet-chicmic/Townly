
namespace Townly.AI.Services.Interfaces;
public interface IPortfolioContextBuilder
{
    Task<string> BuildAsync(Guid userId);
}