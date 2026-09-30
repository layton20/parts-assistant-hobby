# PartsAssistant.Langfuse Project Design

## Goal

Add a class library to the PartsAssistant solution as the home for future Langfuse/OpenTelemetry integration.

## Project Structure

- Create `src/PartsAssistant.Langfuse/PartsAssistant.Langfuse.csproj` as a class library using the repository's shared build properties and `net10.0` target.
- Add the project to `PartsAssistant.sln` under the existing `src` solution folder.
- Remove the scaffolded `Class1.cs` file.
- Add the `OpenTelemetry.Exporter.OpenTelemetryProtocol` NuGet package to the new project, using the latest stable compatible version at implementation time.
- Use the `PartsAssistant.Langfuse` namespace and assembly naming convention.

## Scope Boundaries

This change creates the integration project and installs its exporter dependency only. It does not add tracing, Langfuse SDK/API calls, configuration, credentials, project references, or runtime behavior.

## Verification

- Restore and build the solution.
- Run the existing solution tests to verify no regressions.
