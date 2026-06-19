namespace AgentFrameworkWebApi.Models;

public sealed record ChatResponse(
    string SessionId,
    string AgentId,
    string Message,
    DateTimeOffset CreatedAt);
