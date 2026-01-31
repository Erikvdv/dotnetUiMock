using Mediator;

namespace UiMock.Tests.Mocks;

/// <summary>
/// MediatR Request type for testing handlers not in startup assembly
/// </summary>
#pragma warning disable CS8767
public record TestRequest(string Input) : IRequest<TestResponse>;
#pragma warning restore CS8767
