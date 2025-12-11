# Code Patterns

Common patterns used throughout the codebase.

## Service Pattern

All business logic goes in services:

```csharp
public interface IMyService {
    Task<string> DoSomethingAsync(string input);
}

public class MyService : IMyService {
    private readonly ILogger<MyService> _logger;
    
    public MyService(ILogger<MyService> logger) {
        _logger = logger;
    }
    
    public async Task<string> DoSomethingAsync(string input) {
        _logger.LogInformation("Processing {Input}", input);
        return "Result";
    }
}
```

Register in `Program.cs`:
```csharp
builder.Services.AddScoped<IMyService, MyService>();
```

Use in components:
```razor
@inject IMyService Service

@code {
    protected override async Task OnInitializedAsync() {
        var result = await Service.DoSomethingAsync("test");
    }
}
```

## Configuration Pattern

Use `AppConstants.cs` for all constants:

```csharp
public static class AppConstants {
    public const string MyValue = "example";
    public const int MyNumber = 42;
    
    public static class Features {
        public const bool EnableFeatureX = true;
    }
}
```

Access anywhere:
```csharp
var value = AppConstants.MyValue;
if (AppConstants.Features.EnableFeatureX) { }
```

## Exception Handling

Create specific exceptions:

```csharp
public class MyException : Exception {
    public MyException(string message) : base(message) { }
}
```

Throw with context:
```csharp
if (something_bad) {
    throw new MyException("Detailed error message");
}
```

Catch specifically:
```csharp
try {
    DoWork();
}
catch (MyException ex) {
    _logger.LogError(LogEvents.MyEvent, ex, "Work failed");
    Toast.ShowError(ex.Message);
}
catch (Exception ex) {
    _logger.LogError("Unexpected error: {Error}", ex.Message);
}
```

## Logging Pattern

Always log important events:

```csharp
// Initialization
_logger.LogInformation(LogEvents.AppStartup, "Component initialized");

// Operations
_logger.LogInformation(LogEvents.FileProcessing, "Processing {File}", filename);

// Errors
_logger.LogError(LogEvents.Error, ex, "Operation failed for {Id}", itemId);

// Debug (dev only)
_logger.LogDebug("Debug info: {Value}", value);
```

## Validation Pattern

Validate input data:

```csharp
public async Task<Result> ProcessAsync(string data) {
    if (string.IsNullOrEmpty(data)) {
        throw new ArgumentException("Data is required");
    }
    
    if (data.Length > MaxLength) {
        throw new ArgumentException("Data is too long");
    }
    
    // Process...
}
```

Use in forms:
```razor
<input @bind="FormData.Name" />

@if (string.IsNullOrEmpty(FormData.Name)) {
    <p class="text-danger">Name is required</p>
}
```

## Async Pattern

Always use async for I/O operations:

```csharp
// ✓ Good - async
public async Task<string> ReadFileAsync(string path) {
    return await File.ReadAllTextAsync(path);
}

// ✗ Bad - blocking
public string ReadFile(string path) {
    return File.ReadAllText(path);
}
```

In components:
```razor
@code {
    private string _data;
    
    protected override async Task OnInitializedAsync() {
        _data = await Service.GetDataAsync();
    }
}
```

## Component Pattern

Keep components simple and focused:

```razor
@page "/mycomponent"
@inject IMyService Service
@inject ILogger<MyComponent> Logger

<h1>My Component</h1>

@if (_isLoading) {
    <p>Loading...</p>
}
else {
    <p>@_data</p>
}

@code {
    private string _data = "";
    private bool _isLoading = true;
    
    protected override async Task OnInitializedAsync() {
        try {
            _data = await Service.GetDataAsync();
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Failed to load data");
        }
        finally {
            _isLoading = false;
        }
    }
}
```

## Model Pattern

Keep models simple and focused:

```csharp
public class MyModel {
    // Auto-properties
    public string Name { get; set; } = "";
    public int Value { get; set; }
    
    // Methods for model logic
    public bool IsValid() {
        return !string.IsNullOrEmpty(Name) && Value > 0;
    }
}
```

Use in forms:
```razor
<form @onsubmit="HandleSubmit">
    <input @bind="Model.Name" />
    <input type="number" @bind="Model.Value" />
    <button type="submit" disabled="@(!Model.IsValid())">
        Submit
    </button>
</form>

@code {
    private MyModel Model = new();
    
    private async Task HandleSubmit() {
        if (!Model.IsValid()) return;
        await Service.SaveAsync(Model);
    }
}
```

## Dependency Injection Pattern

Constructor injection is preferred:

```csharp
public class MyService {
    private readonly ILogger<MyService> _logger;
    private readonly IConfiguration _config;
    
    // Inject via constructor
    public MyService(
        ILogger<MyService> logger,
        IConfiguration config
    ) {
        _logger = logger;
        _config = config;
    }
}
```

In Blazor components, use `@inject`:

```razor
@inject IMyService Service
@inject ILogger<MyComponent> Logger

@code {
    // Use injected services in code
}
```

## Error Handling Pattern

Consistent error handling:

```csharp
try {
    // Do work
}
catch (OperationCanceledException) {
    // User cancelled
    _logger.LogInformation("Operation cancelled");
}
catch (InvalidOperationException ex) {
    // Expected error
    _logger.LogWarning(ex, "Expected error occurred");
    throw;
}
catch (Exception ex) {
    // Unexpected error
    _logger.LogError(ex, "Unexpected error");
    throw new ApplicationException("Operation failed", ex);
}
```

---

**Next**: [Adding Features](adding-features.md)
