// This file is part of Emzi0767.Common project.
//
// Copyright © 2020-2025 Emzi0767
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Emzi0767.Utilities;

/// <summary>
/// Gets information about current runtime.
/// </summary>
public static class RuntimeInformation
{
    /// <summary>
    /// Gets the current runtime's version.
    /// </summary>
    public static string Version { get; }

    static RuntimeInformation()
    {
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        var mscorlib = loadedAssemblies.Select(x => new { Assembly = x, AssemblyName = x.GetName() })
            .FirstOrDefault(x => x.AssemblyName.Name == "mscorlib"
#if NETCOREAPP || NETSTANDARD
                 || x.AssemblyName.Name == "System.Private.CoreLib"
#endif
            );

#if NETCOREAPP || NETSTANDARD
        var location = mscorlib.Assembly.Location;
        var assemblyFile = new FileInfo(location);
        var versionFile = new FileInfo(Path.Combine(assemblyFile.Directory.FullName, ".version"));
        if (versionFile.Exists)
        {
            var lines = File.ReadAllLines(versionFile.FullName, new UTF8Encoding(false));

            if (lines.Length >= 2)
            {
                Version = lines[1];
                return;
            }
        }
#endif

        var infVersion = mscorlib.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (infVersion != null)
        {
            var infVersionString = infVersion.InformationalVersion;
            if (!string.IsNullOrWhiteSpace(infVersionString))
            {
                Version = infVersionString.Split(' ').First();
                return;
            }
        }

        Version = mscorlib.AssemblyName.Version.ToString();
    }
}
