# Move Domain Under src Design

## Goal

Move the Domain project from `Domain/` to `src/PartsAssistant.Domain/` to align it with the other `PartsAssistant.*` projects.

## Structure and References

- Relocate the existing Domain project and source files to `src/PartsAssistant.Domain/`.
- Preserve the project file name, assembly identity, namespaces, project GUID, and build settings.
- Update `PartsAssistant.sln` to point to the new project path and nest the project under the existing `src` solution folder instead of the `Domain` solution folder.
- Update references from `src/PartsAssistant.Evaluation/PartsAssistant.Evaluation.csproj` and `Tests/Domain.Tests/Domain.Tests.csproj` to the new project path.
- Do not change Domain behavior, dependencies, or test logic.

## Verification

- Confirm the solution lists the Domain project at its new path.
- Confirm both project references resolve to `src/PartsAssistant.Domain/PartsAssistant.Domain.csproj`.
- Run the full solution test suite after restore, disabling NuGet audit if its network check fails.