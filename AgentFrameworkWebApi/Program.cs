using AgentFrameworkWebApi.Agents;
using AgentFrameworkWebApi.Models;
using Microsoft.Agents.AI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<AIAgent, EchoAgent>();
builder.Services.AddSingleton<SessionStore>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => Results.Redirect("/agent"));

app.MapGet("/agent", (AIAgent agent) =>
{
    return Results.Ok(new AgentInfoResponse(
        agent.Id,
        agent.Name,
        agent.Description,
        "POST a JSON body to /agent/chat with a message and optional sessionId."));
})
.WithName("GetAgentInfo");

app.MapPost("/agent/chat", async (
    ChatRequest request,
    AIAgent agent,
    SessionStore sessions,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Message))
    {
        return Results.BadRequest(new { error = "Message is required." });
    }

    var sessionId = string.IsNullOrWhiteSpace(request.SessionId)
        ? Guid.NewGuid().ToString("n")
        : request.SessionId.Trim();

    var session = await sessions.GetOrAddAsync(
        sessionId,
        () => agent.CreateSessionAsync(cancellationToken).AsTask());

    var response = await agent.RunAsync(request.Message, session, cancellationToken: cancellationToken);

    return Results.Ok(new ChatResponse(
        sessionId,
        response.AgentId ?? agent.Id,
        response.Text,
        response.CreatedAt ?? DateTimeOffset.UtcNow));
})
.WithName("ChatWithAgent");

app.Run();
