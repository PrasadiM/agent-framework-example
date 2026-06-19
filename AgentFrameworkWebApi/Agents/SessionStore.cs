using System.Collections.Concurrent;
using Microsoft.Agents.AI;

namespace AgentFrameworkWebApi.Agents;

public sealed class SessionStore
{
    private readonly ConcurrentDictionary<string, Lazy<Task<AgentSession>>> _sessions = new();

    public async Task<AgentSession> GetOrAddAsync(
        string sessionId,
        Func<Task<AgentSession>> sessionFactory)
    {
        var lazySession = _sessions.GetOrAdd(
            sessionId,
            _ => new Lazy<Task<AgentSession>>(sessionFactory));

        return await lazySession.Value;
    }
}
