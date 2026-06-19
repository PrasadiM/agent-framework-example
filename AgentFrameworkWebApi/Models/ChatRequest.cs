namespace AgentFrameworkWebApi.Models;

public sealed record ChatRequest(
    string Message,
    string? SessionId = null);
