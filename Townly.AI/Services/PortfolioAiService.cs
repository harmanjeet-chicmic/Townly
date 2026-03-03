using System.Text.Json;
using Townly.AI.DTOs;
using Townly.AI.Services.Interfaces;

namespace Townly.AI.Services;

public class PortfolioAiService : IPortfolioAiService
{
    private readonly IPortfolioContextBuilder _contextBuilder;
    private readonly ILlmClient _llmClient;

    public PortfolioAiService(
        IPortfolioContextBuilder contextBuilder,
        ILlmClient llmClient)
    {
        _contextBuilder = contextBuilder;
        _llmClient = llmClient;
    }

    public async Task<AiPortfolioResponseDto> HandleAsync(Guid userId, string question)
    {
        // 1️⃣ Build portfolio context
        var context = await _contextBuilder.BuildAsync(userId);

        // 2️⃣ Build prompt
        var prompt = BuildPrompt(context, question);

        // 3️⃣ Call Gemini
        var rawResponse = await _llmClient.GenerateAsync(prompt);

        // 🔎 DEBUG LOG
        Console.WriteLine("===== RAW GEMINI RESPONSE =====");
        Console.WriteLine(rawResponse);
        Console.WriteLine("================================");

        // 4️⃣ Parse safely
        return ParseJson(rawResponse);
    }

    private string BuildPrompt(string context, string question)
    {
        return $$"""
You are Townly AI — a smart, friendly portfolio intelligence assistant.

Your personality:
- Professional but conversational.
- Clear and easy to understand.
- Helpful and calm.
- Slightly human, not robotic.

You can ONLY help with:
- Portfolio performance
- Investment returns
- Property analysis
- Risk assessment
- Income & diversification

If the user's question is OUTSIDE these topics:
- Do NOT analyze portfolio data.
- Respond naturally and politely.
- Guide the user back to portfolio-related topics.
- Still return valid JSON.

Out-of-scope response example style:
Friendly, short, helpful, not robotic.

Always return STRICT JSON only.
Do NOT wrap in markdown.
Do NOT add explanation outside JSON.

JSON format:

{
  "summary": "Main explanation in a natural tone.",
  "keyFactors": ["Important supporting points if relevant."],
  "riskLevel": "Low | Medium | High | N/A",
  "confidence": "High | Medium | Low"
}

If the question is portfolio-related:
- Use provided context.
- Explain clearly.
- Highlight key drivers.
- Assign a reasonable risk level.

Portfolio Context:
{{context}}

User Question:
{{question}}
""";
    }
    private AiPortfolioResponseDto ParseJson(string raw)
    {
        try
        {
            raw = raw.Trim();

            // Remove markdown if present
            if (raw.StartsWith("```"))
            {
                raw = raw.Replace("```json", "")
                         .Replace("```", "")
                         .Trim();
            }

            // Extract JSON block
            var jsonStart = raw.IndexOf("{");
            var jsonEnd = raw.LastIndexOf("}");

            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var cleanJson = raw.Substring(jsonStart, jsonEnd - jsonStart + 1);

                var result = JsonSerializer.Deserialize<AiPortfolioResponseDto>(
                    cleanJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (result != null)
                    return result;
            }

            throw new Exception("Invalid JSON structure.");
        }
        catch
        {
            return new AiPortfolioResponseDto
            {
                Summary = "Unable to analyze portfolio at the moment.",
                KeyFactors = new List<string>
                {
                    "AI response parsing failed."
                },
                RiskLevel = "Unknown",
                Confidence = "Low"
            };
        }
    }
}