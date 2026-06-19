using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Linq;

namespace AgentFrameworkWebApi.Agents;

public sealed class EchoAgent : AIAgent
{
    protected override string? IdCore => "sample-echo-agent";

    public override string? Name => "Sample Echo Agent";

    public override string? Description =>
        "A minimal Microsoft Agent Framework agent used by the sample Web API.";

    protected override ValueTask<AgentSession> CreateSessionCoreAsync(
        CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult<AgentSession>(new EchoAgentSession());
    }

    protected override ValueTask<JsonElement> SerializeSessionCoreAsync(
        AgentSession session,
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken cancellationToken = default)
    {
        var serialized = JsonSerializer.SerializeToElement(new
        {
            kind = "sample-agent-session"
        }, jsonSerializerOptions);

        return ValueTask.FromResult(serialized);
    }

    protected override ValueTask<AgentSession> DeserializeSessionCoreAsync(
        JsonElement serializedState,
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult<AgentSession>(new EchoAgentSession());
    }

    protected override Task<AgentResponse> RunCoreAsync(
        IEnumerable<ChatMessage> messages,
        AgentSession? session = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var latestUserMessage = messages
            .LastOrDefault(message => message.Role == ChatRole.User)?
            .Text?
            .Trim();

        string text;

        if (string.IsNullOrWhiteSpace(latestUserMessage))
        {
            text = "Hello! I am a sample agent running with the Microsoft Agent Framework. POST a JSON body to /agent/chat with a message and optional sessionId.";
        }
        else
        {
            var lower = latestUserMessage.ToLowerInvariant();

            // Greeting
            if (lower.Contains("hello") || lower.Contains("hi") || lower.Contains("hey"))
            {
                text = "Hello! I can explain this sample, echo a message, or provide simple guidance. Try asking: \"What does this sample do?\"";
            }
            // Ask to explain or demonstrate
            else if (lower.Contains("explain") || lower.Contains("demonstrate") || lower.Contains("what does this"))
            {
                text = "This sample demonstrates a minimal Microsoft Agent Framework agent hosted in an ASP.NET Core Web API. The API exposes /agent (GET) for agent info and /agent/chat (POST) to send messages. Sessions are tracked by sessionId and the agent returns structured responses.";
            }
            // Questions / how-to
            else if (lower.Contains("how") || lower.Contains("how do") || lower.EndsWith("?") || lower.Contains("help"))
            {
                text = "I don't have external web access from this sample. For real AI responses, integrate an AI provider (for example via Microsoft.Extensions.AI) and implement RunCoreAsync to call the model. This sample returns local, deterministic replies.";
            }
            // Short summary instead of raw echo
            else
            {
                var preview = latestUserMessage.Length <= 200
                    ? latestUserMessage
                    : latestUserMessage.Substring(0, 197) + "...";
                text = $"Received your message: \"{preview}\". If you want a different behavior, integrate an AI provider or modify the agent's RunCoreAsync.";
            }
        }

        var response = new AgentResponse(new ChatMessage(ChatRole.Assistant, text))
        {
            AgentId = Id,
            ResponseId = Guid.NewGuid().ToString("n"),
            CreatedAt = DateTimeOffset.UtcNow,
            FinishReason = ChatFinishReason.Stop
        };

        return Task.FromResult(response);
    }

    protected override async IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(
        IEnumerable<ChatMessage> messages,
        AgentSession? session = null,
        AgentRunOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await RunCoreAsync(messages, session, options, cancellationToken);
        yield return new AgentResponseUpdate(ChatRole.Assistant, response.Text);
    }
}


//using System.Runtime.CompilerServices;
//using System.Text.Json;
//using Microsoft.Agents.AI;
//using Microsoft.Extensions.AI;

//namespace AgentFrameworkWebApi.Agents;

//public sealed class EchoAgent : AIAgent
//{
//    protected override string? IdCore => "sample-echo-agent";

//    public override string? Name => "Sample Echo Agent";

//    public override string? Description =>
//        "A minimal Microsoft Agent Framework agent used by the sample Web API.";

//    protected override ValueTask<AgentSession> CreateSessionCoreAsync(
//        CancellationToken cancellationToken = default)
//    {
//        return ValueTask.FromResult<AgentSession>(new EchoAgentSession());
//    }

//    protected override ValueTask<JsonElement> SerializeSessionCoreAsync(
//        AgentSession session,
//        JsonSerializerOptions? jsonSerializerOptions = null,
//        CancellationToken cancellationToken = default)
//    {
//        var serialized = JsonSerializer.SerializeToElement(new
//        {
//            kind = "sample-agent-session"
//        }, jsonSerializerOptions);

//        return ValueTask.FromResult(serialized);
//    }

//    protected override ValueTask<AgentSession> DeserializeSessionCoreAsync(
//        JsonElement serializedState,
//        JsonSerializerOptions? jsonSerializerOptions = null,
//        CancellationToken cancellationToken = default)
//    {
//        return ValueTask.FromResult<AgentSession>(new EchoAgentSession());
//    }

//    protected override Task<AgentResponse> RunCoreAsync(
//        IEnumerable<ChatMessage> messages,
//        AgentSession? session = null,
//        AgentRunOptions? options = null,
//        CancellationToken cancellationToken = default)
//    {
//        var latestUserMessage = messages
//            .LastOrDefault(message => message.Role == ChatRole.User)?
//            .Text;

//        var text = string.IsNullOrWhiteSpace(latestUserMessage)
//            ? "Hello from Microsoft Agent Framework running in an ASP.NET Core Web API."
//            : $"Echo from Microsoft Agent Framework: {latestUserMessage}";

//        var response = new AgentResponse(new ChatMessage(ChatRole.Assistant, text))
//        {
//            AgentId = Id,
//            ResponseId = Guid.NewGuid().ToString("n"),
//            CreatedAt = DateTimeOffset.UtcNow,
//            FinishReason = ChatFinishReason.Stop
//        };

//        return Task.FromResult(response);
//    }

//    protected override async IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(
//        IEnumerable<ChatMessage> messages,
//        AgentSession? session = null,
//        AgentRunOptions? options = null,
//        [EnumeratorCancellation] CancellationToken cancellationToken = default)
//    {
//        var response = await RunCoreAsync(messages, session, options, cancellationToken);
//        yield return new AgentResponseUpdate(ChatRole.Assistant, response.Text);
//    }
//}
