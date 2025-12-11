# Development Setup

Set up your development environment.

## Prerequisites

- .NET 9.0 SDK or later
- Git
- Code editor (VS Code, Visual Studio, or Rider)
- 4GB RAM minimum

## Installation

### 1. Install .NET SDK

Download: [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)

Choose: **.NET 9.0 SDK** (not just Runtime)

### 2. Verify Installation

```powershell
dotnet --version
# Output: 9.0.x or higher
```

### 3. Clone Repository

```powershell
git clone <your-repo-url>
cd lehman.reports
```

### 4. Restore Dependencies

```powershell
dotnet restore
```

Downloads all NuGet packages.

### 5. Build Project

```powershell
dotnet build
```

Should complete with **0 errors**.

## Running the Application

### Recommended: Watch Mode

Auto-reloads on file changes:

```powershell
dotnet watch run
```

Changes to `.cs` or `.razor` files automatically rebuild and reload the browser.

### Once-Off Run

```powershell
dotnet run
```

Run application once. Must restart to see changes.

## IDE Setup

### Visual Studio 2022

1. Open `ReportBuilder.sln`
2. All tools included
3. Press F5 to debug

**Features:**
- Debugger
- Code completion
- Hot reload
- Built-in terminal

### VS Code

1. Install extensions:
   - C# Dev Kit (Microsoft)
   - Blazor extension
   - REST Client (optional)

2. Open folder: `File → Open Folder → lehman.reports`

3. Run: `dotnet watch run` in terminal

**Features:**
- Code completion
- Debugging
- IntelliSense

### JetBrains Rider

1. Open `ReportBuilder.sln`
2. All tools included
3. Press Ctrl+D to debug

**Features:**
- Advanced refactoring
- Code analysis
- Hot reload

## File Organization

```
lehman.reports/
├── Components/          # Blazor UI components
│   ├── Pages/          # Page components (.razor)
│   ├── Shared/         # Reusable components
│   └── Layout/         # Layout components
├── Models/             # Data classes
├── Services/           # Business logic
├── Config/             # Configuration files
├── wwwroot/            # Static assets
└── Program.cs          # Application startup
```

### Key Files

| File | Purpose |
|------|---------|
| `Program.cs` | App initialization, dependency injection |
| `AppConstants.cs` | Configuration constants |
| `Config/appsettings.json` | Runtime settings |
| `.editorconfig` | Code style rules |

## First Steps

### 1. Run Application

```powershell
dotnet watch run
```

### 2. Open Browser

Navigate to: `http://localhost:5000`

### 3. Make a Change

Edit `Components/Pages/Home.razor`:

```razor
<h1>Hello, World! ← Change this text</h1>
```

Save file. Browser reloads automatically (watch mode).

### 4. Explore Codebase

- `Components/Pages/MobileAppReport.razor` - Main report form
- `Services/MobileAppReportGenerator.cs` - PDF generation
- `Services/PdfReportParser.cs` - PDF import
- `Models/MobileAppReportConfig.cs` - Data model

## Debugging

### Visual Studio / Rider

1. Set breakpoint (click in margin)
2. Press F5 to debug
3. Step through code with F10/F11
4. Inspect variables

### VS Code

1. Set breakpoint
2. Open debug view (Ctrl+Shift+D)
3. Choose ".NET 5+ and .NET Core"
4. Press F5
5. Step through code

### Browser Console

Press `F12` to open developer tools:

- **Console** - See JavaScript errors
- **Network** - See HTTP requests
- **Elements** - Inspect HTML

## Common Tasks

### Run Tests

```powershell
dotnet test
```

(Currently no tests - add with xUnit)

### Clean Build

```powershell
dotnet clean
dotnet build
```

### Build Release

```powershell
dotnet build -c Release
```

Optimized build for distribution.

### View Project Structure

```powershell
dotnet sln list
```

Shows all projects in solution.

## Environment Variables

Development-specific settings:

```json
// Config/appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

Logs more details in development mode.

## Troubleshooting

### Port 5000 in Use

```powershell
# Kill existing process
taskkill /F /IM dotnet.exe

# Try again
dotnet watch run
```

### Build Fails

```powershell
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### Hot Reload Not Working

Restart `dotnet watch run`

### IDE Can't Find Classes

Restart IDE - it needs to rebuild IntelliSense cache

## Next Steps

- [Project architecture](architecture.md)
- [Code patterns](code-patterns.md)
- [Adding features](adding-features.md)
