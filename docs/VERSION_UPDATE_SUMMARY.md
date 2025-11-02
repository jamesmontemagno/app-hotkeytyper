# Version Consistency Update Summary

## Problem
The build workflow was setting `Version`, `FileVersion`, and `AssemblyVersion`, but not `InformationalVersion`, which could lead to inconsistent version information across different contexts.

## Changes Made

### 1. **Updated `.github/workflows/build-selfcontained.yml`**

**Before:**
```yaml
/p:Version=$env:VERSION
/p:FileVersion=$env:VERSION
/p:AssemblyVersion=0.0.0.0
```

**After:**
```yaml
/p:Version=$env:VERSION
/p:FileVersion=$env:VERSION
/p:AssemblyVersion=$env:VERSION
/p:InformationalVersion=$env:VERSION
```

**Changes**:
- ? Added `/p:InformationalVersion=$env:VERSION` to set the informational version
- ? Changed `AssemblyVersion` to use `$env:VERSION` instead of hardcoded `0.0.0.0`

### 2. **Updated `HotkeyTyper/HotkeyTyper.csproj`**

**Before:**
```xml
<Version>0.0.0</Version>
<FileVersion>0.0.0</FileVersion>
<AssemblyVersion>0.0.0.0</AssemblyVersion>
```

**After:**
```xml
<!-- Version properties are set by build workflow -->
<!-- Default to dev build versions when building locally -->
<Version Condition="'$(Version)' == ''">0.0.0-dev</Version>
<FileVersion Condition="'$(FileVersion)' == ''">0.0.0</FileVersion>
<AssemblyVersion Condition="'$(AssemblyVersion)' == ''">0.0.0.0</AssemblyVersion>
<InformationalVersion Condition="'$(InformationalVersion)' == ''">0.0.0-dev+local</InformationalVersion>
```

**Changes**:
- ? Made version properties conditional (only used if not set by build)
- ? Added `InformationalVersion` property
- ? Added documentation comments
- ? Used development-friendly defaults for local builds

### 3. **Created `VERSIONING_STRATEGY.md`**

Comprehensive documentation explaining:
- All four version properties and their purposes
- How versioning works in CI/CD vs local development
- Runtime version display logic
- Update manager integration
- File naming conventions
- Troubleshooting guide

## Version Properties Explained

| Property | Set By Workflow | Default (Local) | Purpose |
|----------|----------------|-----------------|---------|
| `Version` | ? `0.0.{BUILD}` | `0.0.0-dev` | NuGet/Package version |
| `FileVersion` | ? `0.0.{BUILD}` | `0.0.0` | Windows file properties |
| `AssemblyVersion` | ? `0.0.{BUILD}.0` | `0.0.0.0` | .NET assembly identity |
| `InformationalVersion` | ? `0.0.{BUILD}` | `0.0.0-dev+local` | Display version |

## Benefits

### 1. **Complete Version Consistency**
All four version properties now use the same value during CI builds:
- Example build #47: All properties = `0.0.47`

### 2. **Better Local Development Experience**
Local builds now show clear development indicators:
- About dialog shows "dev" instead of "0.0.0"
- InformationalVersion includes "+local" suffix

### 3. **Improved Traceability**
- Windows file properties show correct version
- Assembly binding uses consistent version
- Update detection works reliably

### 4. **Future-Proof**
The conditional properties allow easy override for:
- Pre-release builds (`0.0.123-beta`)
- Release candidates (`1.0.0-rc.1`)
- Stable releases (`1.0.0`)

## Version Flow

**CI/CD Build (Run #47)**:
```
GitHub Actions
  ?
VERSION = 0.0.47
  ?
Build Properties:
  Version = 0.0.47
  FileVersion = 0.0.47
  AssemblyVersion = 0.0.47
  InformationalVersion = 0.0.47
  ?
Runtime Assembly.GetName().Version = 0.0.47
  ?
About Dialog: "Version 0.0.47"
  ?
Files:
  - HotkeyTyper_win-x64_v0.0.47.exe
  - HotkeyTyper_win-x64_0.0.47.zip
  ?
GitHub Release: v0.0.47
```

**Local Development Build**:
```
Local dotnet build
  ?
No VERSION env variable
  ?
Project Defaults:
  Version = 0.0.0-dev
  FileVersion = 0.0.0
  AssemblyVersion = 0.0.0.0
  InformationalVersion = 0.0.0-dev+local
  ?
Runtime Assembly.GetName().Version = 0.0.0
  ?
About Dialog: "dev"
```

## Testing Recommendations

### 1. Test Local Build
```powershell
dotnet build HotkeyTyper/HotkeyTyper.csproj
./HotkeyTyper/bin/Debug/net10.0-windows/HotkeyTyper.exe
# Check: About dialog should show "dev"
```

### 2. Test CI Build
```powershell
# Trigger workflow, then download artifact
# Check: About dialog should show "0.0.{BUILD_NUMBER}"
# Check: File properties should show same version
```

### 3. Test Update Detection
```powershell
# With old version installed
# New version released
# Check: Update notification appears
# Check: Download and install works
```

## Backward Compatibility

? **Fully backward compatible**:
- Existing releases unaffected
- Update detection still works
- Version comparison logic unchanged
- No breaking changes to build process

## Future Enhancements

Potential improvements:
1. **Semantic versioning**: Switch to `MAJOR.MINOR.PATCH` format when reaching 1.0
2. **Pre-release builds**: Support `-alpha`, `-beta`, `-rc` suffixes
3. **Build metadata**: Add Git commit SHA to InformationalVersion
4. **Version bumping**: Automated MAJOR/MINOR increment based on commit messages

## Validation Checklist

Before merging, verify:
- ? Workflow YAML syntax is valid
- ? Project file XML is valid
- ? Local build works and shows "dev"
- ? CI build sets all four properties
- ? About dialog displays correctly
- ? File names include version
- ? Update detection works

## Summary

**What Changed**:
1. Added `InformationalVersion` to workflow
2. Changed `AssemblyVersion` to use runtime version instead of hardcoded value
3. Made project file version properties conditional with development defaults
4. Created comprehensive versioning documentation

**Why It Matters**:
- Ensures complete version consistency across all contexts
- Improves developer experience for local builds
- Makes troubleshooting easier with clear version indicators
- Provides foundation for future versioning enhancements

**Impact**:
- ? No breaking changes
- ? Better version tracking
- ? Clearer local development experience
- ? More reliable update detection
- ? Professional Windows file properties

All version references are now consistently set and displayed throughout the application lifecycle!
