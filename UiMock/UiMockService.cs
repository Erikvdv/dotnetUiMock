using System.Reflection;
using System.Text.Json;

namespace DotnetUiMock;

public class UiMockService
{
    public List<ServiceMocks> ServiceMocksList { get; set; } = [];
    
    public List<SelectedScenario> SelectedScenarios { get; } = [];

    public void Init()
    {
        var savedScenarios = ReadFromDisk();
        if (savedScenarios != null)
        {
            SelectedScenarios.AddRange(savedScenarios);
            return;
        }

        LoadDefaults();
    }
    
    public void LoadDefaults()
    {
        SelectedScenarios.Clear();
        foreach (var serviceMock in ServiceMocksList)
        foreach (var mock in serviceMock.MethodMocks)
            SelectedScenarios.Add(new SelectedScenario(serviceMock.ServiceName, mock.Name,
                mock.Scenarios.First().Name));
        SaveToDisk();
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
        var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty,"wwwroot", "uimock-selection.txt");
        filePath = Path.Combine("wwwroot", "uimock-selection.txt");

        return filePath;
    }
}