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
using System.IO;
using Newtonsoft.Json;

namespace Void.Build.NuGet.Shared.Models;

/// <summary>
///     A class that represents the manifest of an assembly cache.
/// </summary>
public sealed class AssemblyCacheManifest {
    public const int CURRENT_VERSION = 1;

    /// <summary>
    ///     The version of the manifest.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    ///     The items in the assembly cache.
    /// </summary>
    [JsonProperty("assemblies")]
    public AssemblyCacheItem[] Assemblies { get; set; } = Array.Empty<AssemblyCacheItem>();

    public string ToJson() {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public void ToFile(string path) {
        File.WriteAllText(path, ToJson());
    }

    public static AssemblyCacheManifest Create() {
        return new AssemblyCacheManifest {
            Version = CURRENT_VERSION,
        };
    }

    public static AssemblyCacheManifest? FromFile(string path) {
        if (!File.Exists(path))
            return null;

        var json = File.ReadAllText(path);
        return FromJson(json);
    }

    public static AssemblyCacheManifest? FromJson(string json) {
        return JsonConvert.DeserializeObject<AssemblyCacheManifest>(json);
    }
}

/// <summary>
///     A class that represents an item in an assembly cache.
/// </summary>
public sealed class AssemblyCacheItem {
    /// <summary>
    ///     The settings applied to this modified, cached assembly.
    /// </summary>
    [JsonProperty("settings")]
    public AssemblyCacheSettings Settings { get; set; }

    /// <summary>
    ///     The name of the assembly.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     The hash of the source assembly.
    /// </summary>
    [JsonProperty("source_hash")]
    public string SourceHash { get; set; } = string.Empty;
}

/// <summary>
///     The settings applied to a modified, cached assembly.
/// </summary>
[Flags]
public enum AssemblyCacheSettings {
    /// <summary>
    ///     The assembly has not been modified.
    /// </summary>
    Unmodified = 0b0000,

    /// <summary>
    ///     The assembly has been publicized.
    /// </summary>
    Publicized = 0b0001,
}
