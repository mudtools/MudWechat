<#
.SYNOPSIS
    Runs Mud.HttpUtils.JsonContextScaffolder to generate JsonSerializerContext source files
    for the Mud.Wechat.Work.DataModels project.
.DESCRIPTION
    The scaffolder is distributed as a NuGet dotnet tool (command: mud-jsonctx). This script
    does NOT depend on any local source/build path, so it works on any machine / any clone
    location:
      1. Resolves the target project relative to the repo root (script lives in <repo>\scripts\).
      2. Detects the tool (global `mud-jsonctx` or local `dotnet mud-jsonctx`), installing it
         globally when missing (unless -NoInstall).
      3. Runs the tool to scan [HttpJsonSerializable] and emit *_JsonContext.g.cs grouped by
         SerializerClassName into Mud.Wechat.Work.DataModels/Generated.
    Generated files should be committed (re-run only when [HttpJsonSerializable] annotations
    are added/changed; run scripts/AddHttpJsonSerializable.ps1 first for new DTOs).

    Context namespace note: the scaffolder places each generated context in the namespace of
    the FIRST annotated type of its group. Single-namespace groups (CorpTokenAuthentication /
    InternalAppAuthentication / ProviderAuthentication / Common) are stable; multi-namespace
    groups (Contacts / CorpGroup / ExternalContact) take the namespace of the alphabetically
    first annotated file's folder today. If that drifts on regeneration, consumer `using`
    directives fail the build loudly — fix the usings, never hand-edit generated files.
#>

param(
    # NuGet package id (used only for auto-install)
    [string]$ToolPackageId = "Mud.HttpUtils.JsonContextScaffolder",
    # Version to install; pinned to the repo-wide locked Mud.HttpUtils version.
    # 3.0.0 is served by the repo nuget.config local source until it lands on nuget.org.
    [string]$ToolVersion = "3.0.0",
    # Target project (relative to repo root or absolute)
    [string]$TargetProject = "Mud.Wechat.Work.DataModels/Mud.Wechat.Work.DataModels.csproj",
    # Output directory (relative to repo root or absolute)
    [string]$OutputDir = "Mud.Wechat.Work.DataModels/Generated",
    # Auto-complete polymorphic derived types within the same assembly.
    # Default OFF for this repo: every DTO is individually annotated, so derived roots would be
    # pure duplication; worse, the flag drags the OPEN generic WechatChatbotResponse<> (derived
    # from WechatWorkResponse) into CommonJsonContext, where STJ source-gen emits no metadata for
    # open generics (SYSLIB1030) — a dead registration. Known trade-off: with the flag off the
    # tool prints AOT003 warnings for response DTOs deriving from WechatWorkResponse; those are
    # false positives here — deserialization targets are always concrete DTO types, and no
    # base-typed polymorphic mapping is required by the SDK pipeline.
    [switch]$AutoDerivedTypes = $false,
    # Preview only, do not write files
    [switch]$DryRun = $false,
    # Auto-install the tool as a global dotnet tool when missing (default on)
    [switch]$InstallTool = $true,
    # Do not auto-install; error if the tool is missing
    [switch]$NoInstall = $false
)

# Repo root = parent of the script directory (script lives in <repo>\scripts\),
# bound to the script location, works on any clone path
$RepoRoot = Split-Path $PSScriptRoot -Parent

# Fix working directory to the repo root so relative paths and the repo nuget.config
# (local Mud.HttpUtils source) are resolved consistently.
Set-Location $RepoRoot

# Resolve a path relative to the repo root into an absolute path.
function Resolve-RepoPath([string]$p) {
    if ([System.IO.Path]::IsPathRooted($p)) { return $p }
    return Join-Path $RepoRoot $p
}

$project = Resolve-RepoPath $TargetProject
$output  = Resolve-RepoPath $OutputDir

if (-not (Test-Path $project)) {
    Write-Error "Target project not found: $project"
    exit 1
}

# ---------------------------------------------------------------------------
# Locate / install the scaffolder tool (dotnet tool, no local source path).
# Supports both install shapes:
#   - global tool: `mud-jsonctx` on PATH
#   - local tool : `dotnet mud-jsonctx` within a dotnet-tools manifest directory
# ---------------------------------------------------------------------------

# Returns the invocation array for the tool, or $null if not found.
function Find-ToolInvocation {
    # 1) global: command on PATH
    try {
        $null = Get-Command "mud-jsonctx" -ErrorAction Stop
        return @("mud-jsonctx")
    } catch { }

    # 2) local: dotnet can resolve the manifest tool (run from manifest dir or subdir)
    try {
        $out = & dotnet mud-jsonctx --help 2>&1
        if ($LASTEXITCODE -eq 0) {
            return @("dotnet", "mud-jsonctx")
        }
    } catch { }

    return $null
}

$toolInvocation = Find-ToolInvocation

if ($null -eq $toolInvocation) {
    if ($NoInstall -or -not $InstallTool) {
        Write-Error ("Tool 'mud-jsonctx' not found (neither global nor local). Install it first:" +
                     "  global: dotnet tool install --global $ToolPackageId" +
                     "  local : dotnet new tool-manifest; dotnet tool install $ToolPackageId" +
                     "  or pass -InstallTool to let this script install it automatically.")
        exit 1
    }

    Write-Host "Tool '$ToolPackageId' not detected, installing as global dotnet tool..."
    $installArgs = @("tool", "install", "--global", $ToolPackageId, "--version", $ToolVersion)

    & dotnet @installArgs
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Tool installation failed, exit code: $LASTEXITCODE"
        exit $LASTEXITCODE
    }

    $dotnetToolsPath = Join-Path $env:USERPROFILE ".dotnet\tools"
    if ($env:PATH -notlike "*$dotnetToolsPath*") {
        $env:PATH = "$dotnetToolsPath;$env:PATH"
    }

    $toolInvocation = Find-ToolInvocation
    if ($null -eq $toolInvocation) {
        Write-Error "Tool still not found after install. Ensure dotnet tools path is on PATH: $dotnetToolsPath"
        exit 1
    }
}

$toolMode = if ($toolInvocation.Count -eq 1) { "global (mud-jsonctx)" } else { "local (dotnet mud-jsonctx)" }
Write-Host "Detected tool invocation: $toolMode"

# ---------------------------------------------------------------------------
# Run the tool
# ---------------------------------------------------------------------------
Write-Host "==== Mud.HttpUtils JsonContext Scaffolder ===="
Write-Host "Command    : $($toolInvocation -join ' ') (dotnet tool: $ToolPackageId)"
Write-Host "AutoDerived: $AutoDerivedTypes"
Write-Host "Dry run    : $DryRun"
Write-Host "Project    : $project"
Write-Host "Output     : $output"
Write-Host ""

$toolArgs = @(
    "--project", (Resolve-Path $project)
    "-o", $output
)
if ($AutoDerivedTypes) { $toolArgs += "--auto-derived-types" }
if ($DryRun)           { $toolArgs += "--dry-run" }

& $toolInvocation @toolArgs
$exitCode = $LASTEXITCODE

if ($exitCode -ne 0) {
    Write-Error "Scaffolder failed for $project, exit code: $exitCode"
    exit $exitCode
}

Write-Host ""
Write-Host "==== Done ===="
if (-not $DryRun) {
    Write-Host "Generated context files are in: $output"
    Write-Host "Add them to version control, e.g.: git add $output"
}
