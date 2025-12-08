# Developer Tooling and Release Guide

This document describes the recommended tooling, local workflows and scripts for building, testing, documenting and releasing the CryptoNet repository.

Note: script and IDE actions are shown as-is; when running scripts from the repository root use the Scripts/ or repository root path as appropriate (for example: __run_docs.ps1__, __run_release.ps1__).

---

## 1. Build & Test (local)

You can build and run tests using any of the following:

- Visual Studio: use __Build > Build Solution__ and the Test Explorer.
- Visual Studio Code: use the integrated terminal or Test Explorer extensions.
- dotnet CLI:

~~~powershell
dotnet build
dotnet test --configuration Release --no-build
~~~

There are convenience PowerShell scripts in the repository root (or the Scripts folder):

~~~powershell
# Build & test wrapper
.\run_build_and_test.ps1   # if present
# Docker-based build
.\run_docker_build.ps1
~~~

Docker (from solution folder):

~~~
docker build . --file .\Dockerfile --tag cryptonet-service:latest
~~~

Or use the preserved PowerShell wrapper:

~~~powershell
./run_docker_build.ps1
~~~

---

## 2. Release

Use the release helper script to create preview or production releases:

Preview:
~~~powershell
.\run_release.ps1 -VersionNumber 3.0.0 -IsPreview $true
~~~

Production:
~~~powershell
.\run_release.ps1 -VersionNumber 3.0.0 -IsPreview $false
~~~

Tags must conform to the version format expected by CI (see CONTRIBUTING.md and release scripts).

---

## 3. Documentation (DocFX)

Documentation is generated with DocFX and published by CI (pipeline `5-static.yml`). To work with docs locally:

- Install DocFX (one-time):
~~~
dotnet tool install -g docfx
~~~

- Generate docs locally:
~~~powershell
.\run_docs.ps1
~~~

This script typically cleans, builds the code with XML docs, runs DocFX and serves the site locally.

### Update `index.md` from `README.md`

To sync the landing page:
~~~powershell
.\run_update_index.ps1
~~~

The update script will add the required YAML front matter and append README content to `index.md`.

---

## 4. Code Coverage

Quick steps to run coverage locally (Windows PowerShell):

~~~powershell
.\run_codecoverage.ps1
# or
cd .\CryptoNet.UnitTests\
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:coverage-report
~~~

If the test project is missing coverage collectors, add them once:

~~~bash
cd .\CryptoNet.UnitTests\
dotnet add package coverlet.collector
dotnet add package coverlet.msbuild
dotnet tool install --global dotnet-reportgenerator-globaltool
~~~

---

## 5. Scripts & Paths

Common scripts referenced in this repo:

- __run_docs.ps1__ — generate documentation with DocFX.
- __run_release.ps1__ — create a release tag and invoke CI release workflows.
- __run_codecoverage.ps1__ — run tests and generate coverage reports.
- __run_update_index.ps1__ — synchronize README -> index.md for documentation landing.

Scripts are typically in the Scripts/ directory or repository root. Check the script header for exact behavior and required parameters.

---

## 6. Notes & Best Practices

- Doc generation depends on XML comments. Ensure public APIs include XML `<summary>` tags before generating docs.
- The project uses Central Package Management (Directory.Packages.props). Do not hardcode package versions in individual .csproj files.
- Core libraries target .NET Standard 2.0 — avoid .NET 8+ APIs in those projects.
- Build and test locally before opening a PR. CI is configured to treat warnings as errors.

---

## 7. Quick Local Setup Checklist

~~~text
[ ] Install .NET SDK(s) required by the repository
[ ] Install DocFX tool: dotnet tool install -g docfx
[ ] Install report generator tool: dotnet tool install --global dotnet-reportgenerator-globaltool
[ ] Run dotnet build and dotnet test locally
[ ] Run .\run_docs.ps1 to validate documentation generation
[ ] Run .\run_codecoverage.ps1 to validate coverage generation (optional)
~~~

---

If you want, I can:
- apply the same formatting and script-name convention to other docs (for example `docs/examples.md`), or
- create a small PR with these changes.