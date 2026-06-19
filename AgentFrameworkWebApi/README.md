# Microsoft Agent Framework Web API Sample

The Microsoft Agent Framework (MAF) is an open-source SDK and runtime designed to help developers build, orchestrate, and deploy production-grade AI agents and multi-agent workflows.
github - https://github.com/microsoft/agent-framework


A very small ASP.NET Core Web API that demonstrates the Microsoft Agent Framework on .NET 10.

The sample exposes a single deterministic agent through HTTP. It does not require Azure, OpenAI, or any API keys, which makes it safe to clone, build, run, and use in CI immediately. The `EchoAgent` uses the Agent Framework base types (`AIAgent`, `AgentSession`, and `AgentResponse`) so you can replace it later with an OpenAI, Azure AI Foundry, or multi-agent implementation without changing the API shape.

## What is included

- .NET 10 minimal API project
- `Microsoft.Agents.AI` package reference
- A concrete `AIAgent` implementation in `Agents/EchoAgent.cs`
- A concrete `AgentSession` implementation in `Agents/EchoAgentSession.cs`
- In-memory session storage for repeat chat calls
- Example HTTP requests in `AgentFrameworkWebApi.http`
- OpenAPI endpoint in development at `/openapi/v1.json`

## Prerequisites

- .NET 10 SDK

Verify your SDK:

```bash
dotnet --info
```

## Run

```bash
dotnet restore
dotnet run --project AgentFrameworkWebApi.csproj
```

The project listens on `http://localhost:5294` by default.

## Try it

Get agent metadata:

```bash
curl http://localhost:5294/agent
```

Send a chat message:

```bash
curl -k -X POST "https://localhost:7121/agent/chat" -H "Content-Type: application/json" -d "{\"message\":\"How are you?\"}"
```

Continue a named in-memory session:


## Project layout

```text
AgentFrameworkWebApi/
  Agents/
    EchoAgent.cs
    EchoAgentSession.cs
    SessionStore.cs
  Models/
    AgentInfoResponse.cs
    ChatRequest.cs
    ChatResponse.cs
  Program.cs
```

## Where to customize

- Replace `EchoAgent` with a service-backed agent when you are ready to call an LLM.
- Replace `SessionStore` with a database or distributed cache for production.
- Add input validation, authentication, rate limiting, and output filtering before exposing an agent publicly.

## Notes for public repositories

This sample intentionally avoids secrets and cloud dependencies. If you add provider-specific packages later, keep credentials in user secrets, environment variables, or a secret manager. Do not commit API keys or serialized sessions that contain user data.
