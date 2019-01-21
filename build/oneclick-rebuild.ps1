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
# One-click rebuild
#
# Rebuilds all components with default values.
#
# Author: Emzi0767
# Version: 2018-08-30 14:34
#
# Arguments: none
#
# Run as: just run it

# ------------ Defaults -----------

# Output path for bots binaries and documentation.
$target = ".\artifacts"

# Version suffix data. Default is none. General format is 5-digit number.
# The version will be formatted as such: $version-$suffix-$build_number
# e.g. 1.0.0-ci-00420
# If build_number is set to -1, it will be ignored.
$suffix = ""
$build_number = -1

# --------- Execute build ---------
Set-Location ..
& .\build\rebuild-all.ps1 -ArtifactLocation "$target" -Configuration "Release" -VersionSuffix "$suffix" -BuildNumber $build_number | Out-Host
if ($LastExitCode -ne 0)
{
    Write-Host "----------------------------------------------------------------"
    Write-Host "                          BUILD FAILED"
    Write-Host "----------------------------------------------------------------"
}

Write-Host "Press any key to continue..."
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
Exit $LastExitCode
