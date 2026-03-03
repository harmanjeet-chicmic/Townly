namespace Townly.AI.Services.Interfaces;

public interface ILlmClient
{
    Task<string> GenerateAsync(string prompt);
}