namespace Townly.AI.DTOs;

public class AiPortfolioResponseDto
{
    public string Summary { get; set; } = string.Empty;
    public List<string> KeyFactors { get; set; } = new();
    public string RiskLevel { get; set; } = string.Empty;
    public string Confidence { get; set; } = string.Empty;
}