/* Copyright (C) 2023 Tomat & Contributors
 * 
 * Licensed under the GNU Lesser General Public License, version 2.1; you may
 * not use this file except in compliance with the License.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License
 * for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Mono.Cecil;
using Void.Build.NuGet.Shared;
using Void.Build.NuGet.Shared.Models;
using Void.Build.NuGet.Shared.Util;

namespace Void.Build.NuGet.AssemblyPublicizer.Tasks;

/// <summary>
///     Publicizes and caches assemblies.
/// </summary>
public sealed class AssemblyPublicizerTask : VoidTask {
    [Required]
    public string ProjectDirectory { get; set; } = string.Empty;

    [Required]
    public string[] AssemblyPaths { get; set; } = Array.Empty<string>();

    [Required]
    public string[] AssemblySearchDirectories { get; set; } = Array.Empty<string>();

    [Output]
    public string[] OutputAssemblyPaths { get; set; } = Array.Empty<string>();

    protected override bool Execute(VoidContext ctx) {
        Log.LogMessage($"Using project directory: {ProjectDirectory}");

        var assemblyCacheDir = Path.Combine(ProjectDirectory, VoidPaths.ASSEMBLY_CACHE_DIR);
        Directory.CreateDirectory(assemblyCacheDir);

        var assemblyManifestPath = Path.Combine(ProjectDirectory, VoidPaths.ASSEMBLY_CACHE_MANIFEST_PATH);
        AssemblyCacheManifest? manifest;

        if (File.Exists(assemblyManifestPath)) {
            Log.LogMessage($"Found assembly cache manifest at '{assemblyManifestPath}'.");
            manifest = AssemblyCacheManifest.FromFile(assemblyManifestPath);

            if (manifest is null) {
                Log.LogWarning("Failed to load resolved assembly cache manifest.");
                manifest = AssemblyCacheManifest.Create();
            }
            else {
                Log.LogMessage("Successfully loaded resolved assembly cache manifest:");
                Log.LogMessage(File.ReadAllText(assemblyManifestPath));
            }
        }
        else {
            Log.LogMessage($"No assembly cache manifest found at '{assemblyManifestPath}'.");
            manifest = AssemblyCacheManifest.Create();
        }

        PublicizeAssemblies(manifest, assemblyCacheDir);
        OutputAssemblyPaths = manifest.Assemblies.Select(x => Path.Combine(assemblyCacheDir, x.Name + ".dll")).ToArray();

        Log.LogEnumerable(
            "Using the following assemblies from the assembly cache: ",
            "No assemblies were found in the assembly cache; something likely went wrong!",
            OutputAssemblyPaths
        );

        return true;
    }

    private void PublicizeAssemblies(AssemblyCacheManifest manifest, string cacheDir) {
        Log.LogEnumerable(
            "Publicizing the following assemblies: ",
            "No assemblies found to publicize; something likely went wrong!",
            AssemblyPaths
        );

        var hashes = new Dictionary<string, string>();
        var cachedAssemblies = manifest.Assemblies.ToDictionary(x => x.Name, x => x);

        foreach (var assemblyPath in AssemblyPaths) {
            var hash = FileHasher.GetSha256Hash(assemblyPath);
            hashes.Add(assemblyPath, hash);
        }

        Log.LogEnumerable(
            "Computed hashes for the following assemblies: ",
            "No hashes were computed for the assemblies; something likely went wrong!",
            hashes.Select(x => $"{x.Key} -> {x.Value}")
        );

        Log.LogEnumerable(
            "Using the following directories to search for assemblies: ",
            "No directories were specified to search for assemblies; something likely went wrong!",
            AssemblySearchDirectories
        );

        var assemblyResolver = new DefaultAssemblyResolver();
        foreach (var searchDirectory in AssemblySearchDirectories)
            assemblyResolver.AddSearchDirectory(searchDirectory);

        foreach (var assemblyPath in AssemblyPaths) {
            Log.LogMessage("Publicizing assembly: " + assemblyPath);

            var hash = hashes[assemblyPath];
            var assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);

            if (cachedAssemblies.ContainsKey(assemblyName)) {
                if (cachedAssemblies[assemblyName].SourceHash == hash) {
                    Log.LogMessage($"Assembly '{assemblyName}' is already publicized and cached.");
                    continue;
                }

                Log.LogMessage($"Assembly '{assemblyName}' is already cached, but the source hash does not match; invalidated.");
                cachedAssemblies.Remove(assemblyName);
            }
            else {
                Log.LogMessage($"Assembly '{assemblyName}' is not cached.");
            }

            // Until we support additional operations on assemblies, we can assume
            // every assembly just wants to be publicized.
            const AssemblyCacheSettings settings = AssemblyCacheSettings.Publicized;
            var assemblyDefinition = AssemblyModifier.ModifyAssembly(
                assemblyPath,
                assemblyResolver,
                settings
            );
            var cachedAssembly = new AssemblyCacheItem {
                Name = assemblyName,
                SourceHash = hash,
                Settings = settings,
            };
            cachedAssemblies.Add(assemblyName, cachedAssembly);

            var path = Path.Combine(cacheDir, assemblyName + ".dll");
            assemblyDefinition.Write(path);
        }

        manifest.Assemblies = cachedAssemblies.Values.ToArray();
        manifest.ToFile(Path.Combine(ProjectDirectory, VoidPaths.ASSEMBLY_CACHE_MANIFEST_PATH));
    }
}
