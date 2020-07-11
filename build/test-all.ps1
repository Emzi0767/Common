#!/usr/bin/env pwsh
#
# -----------------------------------------------------------------------------
#
# This file is a part of Emzi0767.Common project.
# 
# Copyright 2020 Emzi0767
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
# Test-all
#
# Runs all tests on the library.
# 
# Author:       Emzi0767
# Version:      2020-07-11 18:16
#
# Arguments:
#   .\build\test-all.ps1 <output path> <logger>
#
# Run as:
#   .\build\test-all.ps1 .\path\to\artifact\location Appveyor
#
# or
#   .\build\test-all.ps1 .\path\to\artifact\location Appveyor

param
(
    [parameter(Mandatory = $true)]
    [string] $ResultLocation,

    [parameter(Mandatory = $true)]
    [string] $ResultLogger
)

# Restores the environment
function Restore-Environment()
{
    Write-Host "Restoring environment"
    # Remove-Item ./NuGet.config
}

# Prepares the environment
function Prepare-Environment([string] $target_dir_path)
{
    # Prepare the environment
    # Copy-Item ./.nuget/NuGet.config ./
    
    # Check if the target directory exists
    # If not, create it
    if (-not (Test-Path "$target_dir_path"))
    {
        Write-Host "Target directory does not exist, creating"
        $dir = New-Item -type directory "$target_dir_path"
    }
}

# Tests everything
function Tests-Run-All([string] $target_dir_path, [string] $logger)
{
    # Form target path
    $dir = Get-Item "$target_dir_path"
    $target_dir = $dir.FullName
    Write-Host "Will place results in $target_dir"

    # Run all Tests
    & dotnet test -v minimal -c Release -r "$target_dir" --logger "$logger" --no-build | Out-Host
	if ($LastExitCode -ne 0)
	{
		Write-Host "Running tests failed"
		Return $LastExitCode
	}
    
    Return 0
}

# Prepare environment
Prepare-Environment "$ResultLocation"

# Run all tests
$TestResult = Tests-Run-All "$ResultLocation" "$ResultLogger"

# Restore environment
Restore-Environment

# Check if there were any errors
if ($TestResult -ne 0)
{
    Write-Host "Test run failed with code $BuildResult"
    $host.SetShouldExit($BuildResult)
    Exit $BuildResult
}
else
{
    Exit 0
}
