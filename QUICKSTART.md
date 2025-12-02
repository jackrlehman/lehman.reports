# Quick Start - Tauri Desktop App

## One-Time Setup

```bash
# 1. Install Rust (if not already installed)
# Visit: https://rustup.rs/
# Or run: curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh

# 2. Install Tauri dependencies
cd src-tauri
npm install
cd ..

# 3. Build .NET project
dotnet build
```

## Development - Quick Start

### Easy Mode (One Command)
```bash
# In one terminal, run the full dev setup:
cd src-tauri
npm run dev
```

This automatically:
- Starts your .NET server on localhost:5000
- Starts Tauri dev environment
- Opens the app in a window

If port 5000 is blocked, close any existing dotnet processes:
```bash
taskkill /F /IM dotnet.exe
```

### Advanced Mode (Two Terminals)

Terminal 1 - Run .NET server:
```bash
dotnet run
# Watch for: "Now listening on: http://localhost:5000"
```

Terminal 2 - Run Tauri:
```bash
cd src-tauri
npm run dev
```

## Building for Distribution

Create a standalone EXE:

```bash
cd src-tauri
npm run build
```

Find your executable:
```
src-tauri/target/release/report-builder.exe
src-tauri/target/release/bundle/msi/Report Builder_*.msi (installer)
```

## Troubleshooting

**"Failed to load Blazor app"**
- Make sure dotnet server is running
- Check no other app is using port 5000
- Look for error messages in the Tauri dev terminal

**"Error: Failed to start Blazor server"**
- Rust can't find dotnet.exe
- Add Rust to PATH: `rustup self update`
- Or manually navigate: check that `ReportBuilder.Web.csproj` exists

**Port 5000 already in use**
```bash
# Find and kill the process
taskkill /F /IM dotnet.exe
# Or change port in Program.cs
```

**Large build size**
- First build is big because it includes .NET runtime
- Subsequent builds are cached
- Final EXE is ~100-150 MB (that's normal for .NET apps)

## File Structure

```
lehman.reports/
├── Components/          # Blazor components
├── Models/             # Data models
├── Services/           # Business logic
├── wwwroot/            # Static files
├── Program.cs          # .NET entry point
├── ReportBuilder.Web.csproj
├── src-tauri/          # Tauri desktop app
│   ├── src/
│   │   ├── main.rs     # Rust backend
│   │   └── main.ts     # TypeScript frontend
│   ├── index.html      # Window HTML
│   ├── Cargo.toml      # Rust deps
│   ├── package.json    # npm deps
│   └── tauri.conf.json # Tauri config
└── dev.ps1             # Helper script
```

## Next Steps

- Modify `src-tauri/tauri.conf.json` to change window size/title
- Update `Program.cs` if you change the port
- See TAURI_README.md for advanced configuration
