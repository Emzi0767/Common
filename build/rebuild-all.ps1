#!/usr/bin/env pwsh
#
# -----------------------------------------------------------------------------
#
# This file is a part of Emzi0767.Common project.
# 
# Copyright 2019 Emzi0767
# 
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
# 
#   http://www.apache.org/licenses/LICENSE-2.0
#   
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
#
# -----------------------------------------------------------------------------
#
# Rebuild-all
#
# Rebuilds the project and places artifacts in specified directories.
# 
# Author:       Emzi0767
# Version:      2018-08-30 14:41
#
# Arguments:
#   .\build\rebuild-all.ps1 <output path> <configuration> [version suffix] [build-number]
#
# Run as:
#   .\build\rebuild-all.ps1 .\path\to\artifact\location Debug/Release version-suffix build-number

param
(
    [parameter(Mandatory = $true)]
    [string] $ArtifactLocation,

    [parameter(Mandatory = $true)]
    [string] $Configuration,

    [parameter(Mandatory = $false)]
    [string] $VersionSuffix,

    [parameter(Mandatory = $false)]
    [int] $BuildNumber = -1
)

# Check if configuration is valid
if ($Configuration -ne "Debug" -and $Configuration -ne "Release")
{
    Write-Host "Invalid configuration specified. Must be Release or Debug."
    Exit 1
}

# Check if we have a version prefix
if (-not $VersionSuffix -or -not $BuildNumber -or $BuildNumber -eq -1)
{
    # Nope
    Write-Host "Building production packages"

    # Invoke the build script
    & .\build\rebuild-lib.ps1 -ArtifactLocation "$ArtifactLocation" -Configuration "$Configuration" | Out-Host
}
else
{
    # Yup
    Write-Host "Building nightly packages"

    # Invoke the build script
    & .\build\rebuild-lib.ps1 -ArtifactLocation "$ArtifactLocation" -Configuration "$Configuration" -VersionSuffix "$VersionSuffix" -BuildNumber $BuildNumber | Out-Host
}

# Check if it failed
if ($LastExitCode -ne 0)
{
    Write-Host "Build failed with code $LastExitCode"
    $host.SetShouldExit($LastExitCode)
    Exit $LastExitCode
}
Exit 0
