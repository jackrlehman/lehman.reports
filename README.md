# Report Builder

Professional PDF report generation application for mobile app analytics.

## Quick Start

### Development
```powershell
dotnet watch run
# Launches at http://localhost:5000 with auto-reload
```

### Distribution
```powershell
.\build-distribution.ps1 -Version "1.05"
# Creates ReportBuilder-v1.05.zip ready to distribute
```

## 📚 Documentation

**Quick access:**

```powershell
# Option 1: Open in browser (no setup needed)
.\VIEW-DOCS.bat

# Option 2: Live preview with auto-reload (requires Python)
# First install: pip install mkdocs mkdocs-material
mkdocs serve
# Then open http://localhost:8000
```

The documentation covers:
- Getting started & installation
- Development setup & architecture  
- Building distributions for users
- Configuration & API reference
- FAQ & troubleshooting

## Requirements

- **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Python 3.9+** (optional, only for developing docs) - [Download](https://python.org)

## Project Structure

```
/Components      - Razor UI components
/Config          - Application configuration
/docs            - Documentation source (MkDocs)
/docs-site       - Generated documentation (HTML)
/Models          - Data models
/Services        - Business logic
/wwwroot         - Static assets
```

## License

Open Software License (OSL) v3.0 - See [docs/license.md](docs/license.md)
