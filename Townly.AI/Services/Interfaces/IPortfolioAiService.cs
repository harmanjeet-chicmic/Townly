public interface IPortfolioAiService
{
    Task<AiPortfolioResponseDto> HandleAsync(Guid userId, string question);
}