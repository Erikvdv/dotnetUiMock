using DotnetUiMock;
using Mediator;
using NSubstitute;

namespace UiMock.Tests.Mocks;

/// <summary>
/// Mock implementation of IRequestHandler for TestRequest/TestResponse
/// Used to test BaseMockService with async MediatR handlers
/// </summary>
public class TestRequestHandlerMock : BaseMockService<IRequestHandler<TestRequest, TestResponse>>
{
    public TestRequestHandlerMock()
    {
        FriendlyName = "Test Request Handler";
        MethodMocks =
        [
            new("Handle", [
                new MockScenario("success", HandleSuccess),
                new MockScenario("withcount", HandleWithCount)
            ], 150)
        ];
    }

    private static void HandleSuccess(object service, int delay)
    {
        (service as IRequestHandler<TestRequest, TestResponse>)?
            .Handle(Arg.Any<TestRequest>(), Arg.Any<CancellationToken>())
            .Returns<ValueTask<TestResponse>>(async _ =>
            {
                await Task.Delay(delay);
                return new TestResponse { Result = "mocked response", Count = 1 };
            });
    }

    private static void HandleWithCount(object service, int delay)
    {
        (service as IRequestHandler<TestRequest, TestResponse>)?
            .Handle(Arg.Any<TestRequest>(), Arg.Any<CancellationToken>())
            .Returns<ValueTask<TestResponse>>(async _ =>
            {
                await Task.Delay(delay);
                return new TestResponse { Result = "mocked response with count", Count = 5 };
            });
    }
}

