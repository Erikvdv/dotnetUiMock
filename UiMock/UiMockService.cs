using System.Reflection;
using System.Text.Json;

namespace DotnetUiMock;

public class UiMockService
{
    public List<ServiceMocks> ServiceMocksList { get; set; } = [];

    public List<SelectedScenario> SelectedScenarios { get; } = [];

    public void Init()
    {
        var defaultScenarios = GetDefaultScenarios();
        var savedScenarios = ReadFromDisk();

        if (savedScenarios == null)
        {
            SelectedScenarios.AddRange(defaultScenarios);
            return;
        }

        if (defaultScenarios.Any(
                scenario => !savedScenarios.Any(savedScenario => savedScenario.ServiceName == scenario.ServiceName &&
                savedScenario.MethodName == scenario.MethodName)))
        {
            SelectedScenarios.AddRange(defaultScenarios);
            return;
        }

        SelectedScenarios.AddRange(savedScenarios);
    }

    public void LoadDefaults()
    {
        SelectedScenarios.Clear();
        var defaultScenarios = GetDefaultScenarios();
        SaveToDisk();
    }

    private List<SelectedScenario> GetDefaultScenarios()
    {
        List<SelectedScenario> scenarios = [];
        foreach (var serviceMock in ServiceMocksList)
        {
            foreach (var mock in serviceMock.MethodMocks)
                scenarios.Add(new SelectedScenario(serviceMock.ServiceName, mock.Name,
                    mock.Scenarios.First().Name, mock.DelayMs));
        }

        return scenarios;
    }

    public void ToggleService(string serviceName, bool isMocked)
    {
        var service = ServiceMocksList.FirstOrDefault(x => x.ServiceName == serviceName);

        if (service == null)
            return;

        service.IsMocked = isMocked;
        SaveToDisk();
    }

    public void UpdateSelectedScenario(SelectedScenario updatedScenario)
    {
        var scenario = SelectedScenarios
            .FirstOrDefault(x =>
                x.ServiceName == updatedScenario.ServiceName && x.MethodName == updatedScenario.MethodName);

        if (scenario == null) return;

        scenario.Scenario = updatedScenario.Scenario;
        scenario.DelayMs = updatedScenario.DelayMs;
        SaveToDisk();
    }

    public void InvokeDelegates(string serviceName, List<MethodMocks> mocks, object mockedService)
    {
        foreach (var scenario in SelectedScenarios.Where(x => x.ServiceName == serviceName))
        {
            var handler = mocks
                .First(x => x.Name == scenario.MethodName).Scenarios
                .First(x => x.Name == scenario.Scenario).Handler;
            handler.DynamicInvoke(mockedService, scenario.DelayMs);
        }
    }

    private void SaveToDisk()
    {
        var json = JsonSerializer.Serialize(SelectedScenarios);
        File.WriteAllText(GetStateFilePath(), json);
    }

    private List<SelectedScenario>? ReadFromDisk()
    {
        var filePath = GetStateFilePath();

        if (!File.Exists(filePath))
        {
            return null;
        }

        var content = File.ReadAllText(filePath);
        var selectedScenarios = JsonSerializer.Deserialize<List<SelectedScenario>>(content);
        return selectedScenarios;
    }

    private static string GetStateFilePath()
    {
        var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty,
            "wwwroot", "uimock-selection.txt");
        filePath = Path.Combine("wwwroot", "uimock-selection.txt");

        return filePath;
    }
}