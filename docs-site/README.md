# MkDocs Compiled Documentation

This folder contains the compiled HTML documentation for Report Builder, generated from the markdown files in the `/docs` folder using MkDocs.

## What This Is

- **Generated Files**: These are automatically created by running `mkdocs build`
- **DO NOT EDIT**: Files in this folder are regenerated each build
- **Distribution**: This folder is included in distribution packages for offline access
- **Local Access**: Open `index.html` in a web browser to view documentation locally

## Rebuilding Documentation

To regenerate this folder after making changes to docs:

```bash
python -m mkdocs build
```

Or use the distribution build script which automatically includes this:

```bash
.\build-distribution.ps1
```

## Structure

- `index.html` - Documentation homepage
- `assets/` - CSS, JavaScript, fonts
- `guides/` - Getting started and usage guides
- `development/` - Development documentation
- `reference/` - API and configuration reference
- `search/` - Search index for offline functionality

## Accessing Documentation

**Online (GitHub Pages)**: Visit the repository's documentation site  
**Offline (Local)**: Open `index.html` in any web browser  
**Development (With Live Reload)**: Run `mkdocs serve`

---

Generated with MkDocs and Material Theme
