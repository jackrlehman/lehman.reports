@echo off
REM Rebuilds documentation from markdown source files
REM Requires: Python 3.9+ and MkDocs installed

echo Checking for Python and MkDocs...

python --version >nul 2>&1
if errorlevel 1 (
    echo.
    echo ERROR: Python not found!
    echo.
    echo To rebuild documentation, you need Python installed.
    echo Download from: https://www.python.org/downloads/
    echo.
    echo After installing Python, run:
    echo   pip install mkdocs mkdocs-material
    echo   mkdocs build
    echo.
    pause
    exit /b 1
)

echo Found Python. Checking for MkDocs...

mkdocs --version >nul 2>&1
if errorlevel 1 (
    echo.
    echo MkDocs not found. Installing...
    pip install mkdocs mkdocs-material
)

echo.
echo Rebuilding documentation...
mkdocs build

echo.
echo Done! Documentation rebuilt in docs-site folder.
echo Open docs-site\index.html in your browser to view.
pause
