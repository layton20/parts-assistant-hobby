# PartsAssistant.Langfuse Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add a `PartsAssistant.Langfuse` class library with the OpenTelemetry OTLP exporter package to the solution.

**Architecture:** Create the project under `src/`, follow shared repository build properties, and register it in the existing `src` solution folder. Keep the project dependency-only for now; add no tracing or Langfuse runtime behavior.

**Tech Stack:** .NET 10, NuGet, `OpenTelemetry.Exporter.OpenTelemetryProtocol`.

**Spec:** `docs/superpowers/specs/2026-09-28-partsassistant-langfuse-design.md`

## Global Constraints

- Project path: `src/PartsAssistant.Langfuse/PartsAssistant.Langfuse.csproj`.
- Target framework: `net10.0` via shared build properties.
- Namespace and assembly naming convention: `PartsAssistant.Langfuse`.
- Package: `OpenTelemetry.Exporter.OpenTelemetryProtocol`, latest stable version compatible at implementation time.
- Do not add Langfuse SDK/API calls, tracing, configuration, credentials, project references, or runtime behavior.

## Review Focus

- NuGet restore or target framework incompatibility: addressed by restoring and building the solution.
- Missing solution registration or scaffold residue: addressed by checking solution build and removing `Class1.cs`.
- No runtime inputs or behavioral code are introduced, so no behavior-level tests are needed.

---

### Task 1: Add the Langfuse integration library

**Files:**
- Create: `src/PartsAssistant.Langfuse/PartsAssistant.Langfuse.csproj` and the SDK-generated project scaffold.
- Modify: `PartsAssistant.sln`.
- Delete: `src/PartsAssistant.Langfuse/Class1.cs`.

**Interfaces:**
- Consumes: repository-wide target framework, nullable, implicit using, and warning settings from `Directory.Build.props`.
- Produces: buildable `PartsAssistant.Langfuse` library referencing `OpenTelemetry.Exporter.OpenTelemetryProtocol`.

- [ ] **Step 1: Create and register the class library**

Run:

```sh
dotnet new classlib --name PartsAssistant.Langfuse --output src/PartsAssistant.Langfuse
dotnet sln PartsAssistant.sln add src/PartsAssistant.Langfuse/PartsAssistant.Langfuse.csproj --solution-folder src
```

Expected: the project exists under `src/` and appears under the `src` solution folder.

- [ ] **Step 2: Remove the generated class and install the exporter**

Delete `src/PartsAssistant.Langfuse/Class1.cs`, then run:

```sh
dotnet add src/PartsAssistant.Langfuse/PartsAssistant.Langfuse.csproj package OpenTelemetry.Exporter.OpenTelemetryProtocol
```

Expected: the project file contains the package reference and restore succeeds.

- [ ] **Step 3: Verify the solution**

Run:

```sh
dotnet build PartsAssistant.sln
dotnet test PartsAssistant.sln --no-build
```

Expected: solution build and existing tests pass.
