# Move Domain Under src Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Relocate the Domain project to `src/PartsAssistant.Domain` and keep all solution references working.

**Architecture:** Move the existing Domain project directory without renaming its project file, assembly, namespaces, or project GUID. Update the solution entry and solution-folder nesting in place, plus the Evaluation and Domain.Tests project references.

**Tech Stack:** .NET 10, SDK-style C# projects, Visual Studio solution.

**Spec:** `docs/superpowers/specs/2026-09-28-move-domain-under-src-design.md`

## Global Constraints

- Move the existing Domain project and source files to `src/PartsAssistant.Domain/`.
- Preserve the project file name, assembly identity, namespaces, project GUID, and build settings.
- Nest the project under the existing `src` solution folder.
- Update project references from Evaluation and Domain.Tests.
- Do not change Domain behavior, dependencies, or test logic.

## Review Focus

- Project file moves but source or enum files do not: verify the moved project still compiles.
- Solution path changes but project GUID/configuration is lost: verify the existing GUID and configuration entries remain.
- Solution nesting remains under `Domain`: inspect the nested-project mapping and confirm it points to `src`.
- Evaluation retains its old relative reference: build Evaluation through the solution.
- Domain.Tests retains its old relative reference: run the full solution test suite.

---

### Task 1: Relocate Domain Project and Update References

**Files:**
- Move: `Domain/` to `src/PartsAssistant.Domain/`
- Modify: `PartsAssistant.sln`
- Modify: `src/PartsAssistant.Evaluation/PartsAssistant.Evaluation.csproj`
- Modify: `Tests/Domain.Tests/Domain.Tests.csproj`

**Interfaces:**
- Consumes: existing project identity `{B0C14F68-CF71-4E0F-8772-767D201608D8}` and project file `PartsAssistant.Domain.csproj`.
- Produces: Domain project path `src/PartsAssistant.Domain/PartsAssistant.Domain.csproj`; Evaluation reference `..\PartsAssistant.Domain\PartsAssistant.Domain.csproj`; Domain.Tests reference `..\..\src\PartsAssistant.Domain\PartsAssistant.Domain.csproj`.

- [ ] **Step 1: Move the existing Domain directory**

Confirm `src/PartsAssistant.Domain` does not already exist, then run from the repository root:

```sh
mv Domain src/PartsAssistant.Domain
```

- [ ] **Step 2: Verify the moved project builds**

Run:

```sh
dotnet build src/PartsAssistant.Domain/PartsAssistant.Domain.csproj -p:NuGetAudit=false
```

Expected: the Domain project builds successfully using the shared root build properties.

- [ ] **Step 3: Update the solution project path and nesting**

In `PartsAssistant.sln`, change the project path for GUID `{B0C14F68-CF71-4E0F-8772-767D201608D8}` to `src\PartsAssistant.Domain\PartsAssistant.Domain.csproj`. Remove the now-unused `Domain` solution-folder entry and its GUID `{996CEBED-A05B-5D42-3FF0-982E087A2599}` mapping. Nest the existing Domain project GUID under the existing `src` solution-folder GUID `{827E0CD3-B72D-47B6-A68D-7590B98EB39B}`. Keep all project configuration entries unchanged.

- [ ] **Step 4: Update both project references**

Set the Evaluation reference to `..\PartsAssistant.Domain\PartsAssistant.Domain.csproj`. Set the Domain.Tests reference to `..\..\src\PartsAssistant.Domain\PartsAssistant.Domain.csproj`.

- [ ] **Step 5: Verify solution structure and tests**

Run `dotnet sln PartsAssistant.sln list` and confirm the Domain project appears at its new path. Then run:

```sh
dotnet test PartsAssistant.sln -p:NuGetAudit=false
```

Expected: all solution tests pass with no unresolved project-reference warnings.