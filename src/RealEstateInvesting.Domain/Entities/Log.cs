namespace RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;
public class Log
{
    public Guid Id {get;set;}
    public String CorrelationId {get;set;} = string.Empty;
    public LogLevel Level { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? Exception { get; set; }

    public string? StackTrace { get; set; }

    public string? Endpoint { get; set; }

    public string? HttpMethod { get; set; }

    public Guid? UserId { get; set; }

    public string? RequestBody { get; set; }

    public string? ResponseBody { get; set; }

    public int? ResponseStatus { get; set; }

    public string? IPAddress { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}