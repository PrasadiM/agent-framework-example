namespace AgentFrameworkWebApi.Models;

public sealed record AgentInfoResponse(
    string Id,
    string? Name,
    string? Description,
    string Usage);
