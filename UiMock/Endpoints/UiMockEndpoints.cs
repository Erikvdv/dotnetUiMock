using DotnetUiMock.Components;
using DotnetUiMock.Components.Pages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DotnetUiMock.Endpoints;

public static class UiMockEndpoints
{
    public static void MapUiMockEndpoints(this WebApplication app)
    {
        var path = app.MapGroup("uimock").AllowAnonymous();
        path.MapGet("", GetHomePage);
        path.MapPost("/update", Update);
        path.MapPost("/toggle", Toggle);
        path.MapPost("/reset", Reset);

        var apipath = app.MapGroup("uimock/api");
        apipath.MapGet("", ApiGetServices);
        apipath.MapPost("/update", ApiUpdate);
    }

    private static RazorComponentResult GetHomePage(HttpContext context, UiMockService uiMockService)
    {
        var component =
            new HomePage().GetRenderFragment(new HomePage.Input(uiMockService.ServiceMocksList,
                uiMockService.SelectedScenarios));
        var app = new App().GetComponent(new App.Input(component, false));
        return app;
    }

    private static RazorComponentResult Update(SelectedScenario scenario, UiMockService uiMockService)
    {
        uiMockService.UpdateSelectedScenario(scenario);
        var component =
            new HomePage().GetComponent(new HomePage.Input(uiMockService.ServiceMocksList,
                uiMockService.SelectedScenarios));
        return component;
    }
    
    private static RazorComponentResult Toggle(ToggleRequest toggleRequest, UiMockService uiMockService)
    {
        uiMockService.ToggleService(toggleRequest.ServiceName, toggleRequest.IsMocked == "true");
        var component =
            new HomePage().GetComponent(new HomePage.Input(uiMockService.ServiceMocksList,
                uiMockService.SelectedScenarios));
        return component;
    }
    
    private static RazorComponentResult Reset(UiMockService uiMockService)
    {
        uiMockService.LoadDefaults();
        var component =
            new HomePage().GetComponent(new HomePage.Input(uiMockService.ServiceMocksList,
                uiMockService.SelectedScenarios));
        return component;
    }

    private static IResult ApiGetServices(UiMockService uiMockService)
    {
        return Results.Ok(uiMockService.ServiceMocksList);
    }

    private static IResult ApiUpdate(SelectedScenario scenario, UiMockService uiMockService)
    {
        uiMockService.UpdateSelectedScenario(scenario);
        return Results.Ok();
    }
}