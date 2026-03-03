using Townly.AI.DTOs;

namespace Townly.AI.Services.Interfaces;

public interface IPortfolioAiService
{
    Task<AiPortfolioResponseDto> HandleAsync(Guid userId, string question);
}