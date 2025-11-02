# Versioning Strategy

## Overview
Hotkey Typer uses automated versioning via GitHub Actions to ensure consistent version numbers across all build artifacts and runtime displays.

## Version Properties

The application uses four distinct version properties, all set consistently during the build process:

| Property | Purpose | Format | Example | Usage |
|----------|---------|--------|---------|-------|
| `Version` | NuGet/Package version | `Major.Minor.Patch[-prerelease]` | `0.0.123` | Package metadata, general version |
| `FileVersion` | File version (Windows) | `Major.Minor.Patch[.Revision]` | `0.0.123` | File properties in Windows Explorer |
| `AssemblyVersion` | Assembly identity | `Major.Minor.Patch.Revision` | `0.0.123.0` | .NET assembly binding |
| `InformationalVersion` | Display version | `Major.Minor.Patch[-prerelease][+build]` | `0.0.123` | About dialog, detailed version info |

## Build Workflow Versioning

### Automated Builds (CI/CD)

The `.github/workflows/build-selfcontained.yml` workflow automatically sets all version properties:

```yaml
env:
  VERSION: 0.0.${{ github.run_number }}
  
steps:
- name: Publish
    run: >-
      dotnet publish
   /p:Version=$env:VERSION
   /p:FileVersion=$env:VERSION
      /p:AssemblyVersion=$env:VERSION
      /p:InformationalVersion=$env:VERSION
```

**Version Format**: `0.0.{RUN_NUMBER}`
- **Major**: 0 (pre-release/beta)
- **Minor**: 0 (no minor versions yet)
- **Patch**: GitHub Actions run number (auto-incrementing)

**Example**: Build #47 becomes version `0.0.47`

### Local Development Builds

When building locally without the workflow, the project file provides development defaults:

```xml
<Version Condition="'$(Version)' == ''">0.0.0-dev</Version>
<FileVersion Condition="'$(FileVersion)' == ''">0.0.0</FileVersion>
<AssemblyVersion Condition="'$(AssemblyVersion)' == ''">0.0.0.0</AssemblyVersion>
<InformationalVersion Condition="'$(InformationalVersion)' == ''">0.0.0-dev+local</InformationalVersion>
```

**Local Build Version**: `0.0.0-dev` (displayed as "dev" in About dialog)

## Runtime Version Display

The About dialog displays the version retrieved from the assembly at runtime:

```csharp
private void MnuAbout_Click(object? sender, EventArgs e)
{
    // Get version from assembly (set by build workflow)
    var version = typeof(Form1).Assembly.GetName().Version;
 string versionStr;
 
    if (version != null && version.Major > 0)
    {
        // Use version from assembly (e.g., "0.0.123" from build)
   versionStr = $"{version.Major}.{version.Minor}.{version.Build}";
    }
    else
 {
        // Development build fallback
   versionStr = "dev";
    }

    AboutDialog.Show(this, versionStr);
}
```

**Display Logic**:
- CI/CD builds: Shows actual version (e.g., "0.0.47")
- Local development: Shows "dev"

## Update Manager Integration

The UpdateManager uses Updatum to check GitHub Releases for updates:

```csharp
private static readonly UpdatumManager AppUpdater = new("jamesmontemagno", "app-hotkeytyper")
{
    FetchOnlyLatestRelease = true,
};
```

**Update Detection**:
1. Compares current assembly version with latest GitHub Release tag
2. Release tags follow format: `v0.0.{BUILD_NUMBER}`
3. Asset naming: `HotkeyTyper_win-x64_v0.0.{BUILD_NUMBER}.exe`

## File Naming Convention

### Executable Files
**Pattern**: `HotkeyTyper_{RUNTIME_ID}_v{VERSION}.exe`

**Examples**:
- `HotkeyTyper_win-x64_v0.0.47.exe`
- `HotkeyTyper_win-arm64_v0.0.47.exe`

### Zip Archives
**Pattern**: `HotkeyTyper_{RUNTIME_ID}_{VERSION}.zip`

**Examples**:
- `HotkeyTyper_win-x64_0.0.47.zip`
- `HotkeyTyper_win-arm64_0.0.47.zip`

## GitHub Release Naming

### Tag Format
**Pattern**: `v{MAJOR}.{MINOR}.{PATCH}`

**Example**: `v0.0.47`

### Release Title
**Pattern**: `HotkeyTyper {VERSION}`

**Example**: `HotkeyTyper 0.0.47`

### Release Body
Includes:
- Build information (run number, runtimes)
- Changelog from recent releases (via Updatum)
- Download links (automatically added by GitHub)

## Version Consistency Checklist

When making a release, ensure:

- ? All four version properties are set in the build workflow
- ? Version format is consistent: `0.0.{BUILD_NUMBER}`
- ? Assembly version can be read at runtime
- ? About dialog displays the correct version
- ? File names include the version
- ? GitHub Release tag matches the version
- ? Updatum can detect and download updates

## Troubleshooting

### Issue: About dialog shows "dev"
**Cause**: Assembly version is `0.0.0` (Major = 0)
**Solution**: Ensure build workflow sets `/p:AssemblyVersion` correctly

### Issue: Updatum not detecting updates
**Cause**: Release tag format mismatch  
**Solution**: Verify tag is `v{VERSION}` format (e.g., `v0.0.47`)

### Issue: File version shows 0.0.0 in Explorer
**Cause**: `FileVersion` not set during build  
**Solution**: Check workflow has `/p:FileVersion=$env:VERSION`

### Issue: Version mismatch between About and filename
**Cause**: One version property not set correctly  
**Solution**: All four properties must be set to the same value

## Future Versioning Plans

When moving to 1.0 release:

1. **Pre-1.0 (Current)**:
   - Format: `0.0.{BUILD}` (e.g., `0.0.47`)
   - Indicates beta/pre-release status

2. **Version 1.0**:
   - Format: `1.0.{BUILD}` (e.g., `1.0.0`)
   - Update `VERSION` in workflow: `1.0.${{ github.run_number }}`

3. **Minor Versions**:
   - Format: `1.1.{BUILD}` (e.g., `1.1.0`)
   - Update for feature additions

4. **Semantic Versioning**:
   - Consider full semver: `MAJOR.MINOR.PATCH`
   - Breaking changes increment MAJOR
   - Features increment MINOR  
   - Bugfixes increment PATCH

## References

- **Assembly Versioning**: [Microsoft Docs - Assembly Versioning](https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/versioning)
- **Semantic Versioning**: [semver.org](https://semver.org/)
- **Updatum Library**: [GitHub - Updatum](https://github.com/Redth/Updatum)
- **GitHub Actions**: [GitHub Actions Contexts](https://docs.github.com/en/actions/learn-github-actions/contexts)

## Summary

**Key Points**:
- ? Single source of truth: GitHub Actions run number
- ? All version properties set consistently
- ? Clear distinction between CI builds and local dev builds
- ? Runtime version display matches build version
- ? File naming includes version for traceability
- ? Update detection works correctly via Updatum

**Version Flow**:
```
GitHub Actions Run #47
    ?
VERSION = 0.0.47
    ?
Build: /p:Version=0.0.47 /p:FileVersion=0.0.47 /p:AssemblyVersion=0.0.47 /p:InformationalVersion=0.0.47
    ?
Runtime: Assembly.GetName().Version ? 0.0.47
    ?
About Dialog: "Version 0.0.47"
    ?
File: HotkeyTyper_win-x64_v0.0.47.exe
 ?
Release: v0.0.47 (tag) + HotkeyTyper 0.0.47 (title)
    ?
Updatum: Detects v0.0.47 as latest
```

All version references are now consistently set and displayed throughout the application lifecycle.
