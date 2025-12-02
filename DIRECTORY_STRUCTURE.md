lehman.reports/
├── 📄 Program.cs (UPDATED - listens on localhost:5000)
├── 📄 ReportBuilder.Web.csproj (RENAMED from ReportBuilder.csproj)
├── 📄 ReportBuilder.sln (UPDATED - references new structure)
├── 
├── 📚 Components/
│   ├── Pages/
│   │   └── MobileAppReport.razor
│   └── Layout/
│       ├── NavMenu.razor
│       └── MainLayout.razor
├── 
├── 📚 Models/
│   └── MobileAppReportConfig.cs
├── 
├── 📚 Services/
│   ├── MobileAppReportGenerator.cs
│   ├── PdfReportParser.cs
│   └── ToastService.cs
├── 
├── 📚 wwwroot/ (Static files)
├── 
├── 🚀 src-tauri/ (NEW - Tauri Desktop App)
│   ├── 📄 Cargo.toml (Rust dependencies)
│   ├── 📄 build.rs (Rust build script)
│   ├── 📄 package.json (npm dependencies)
│   ├── 📄 tauri.conf.json (Tauri configuration)
│   ├── 📄 index.html (Window HTML)
│   ├── 📄 vite.config.js (Build config)
│   ├── 📄 tsconfig.json (TypeScript config)
│   ├── 📄 tsconfig.node.json
│   │
│   ├── 📂 src/
│   │   ├── 📄 main.rs (Rust backend - launches .NET server)
│   │   └── 📄 main.ts (TypeScript frontend)
│   │
│   └── 📂 node_modules/ (npm packages, auto-generated)
├── 
├── 📖 Documentation (NEW)
│   ├── TAURI_README.md (Complete technical guide)
│   ├── QUICKSTART.md (5-minute setup guide)
│   └── IMPLEMENTATION_SUMMARY.md (What was done)
├── 
├── 🛠️ Helper Scripts (NEW)
│   └── dev.ps1 (PowerShell helper for common tasks)
└── 

Key Files Changed:
  ✏️  Program.cs - Added localhost:5000 URL binding
  ✏️  ReportBuilder.csproj → ReportBuilder.Web.csproj
  ✏️  ReportBuilder.sln - Updated references
  ✏️  .gitignore - Added Tauri ignore rules

How to Use:
  1. One-time setup: cd src-tauri && npm install
  2. Development: cd src-tauri && npm run dev
  3. Build EXE: cd src-tauri && npm run build
  4. More help: ./dev.ps1 or see QUICKSTART.md
