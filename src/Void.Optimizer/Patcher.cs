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

using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using Mono.Cecil;
using Void.Optimizer.Core;

namespace Void.Optimizer;

// I would love to use UsedImplicitly, but Unity includes it themselves which
// would cause it to get loaded at runtime. Incredible, lol.
// TODO: Include JetBrains.Annotations with aliases.
// ReSharper disable once UnusedType.Global
public static class Patcher {
    private static readonly ManualLogSource logger =
        Logger.CreateLogSource(typeof(Patcher).FullName);

    private static readonly string[] target_dlls = ResolveDllNames().ToArray();

    private static readonly List<string> patched_dlls = new();

    // See earlier comment.
    // ReSharper disable once UnusedMember.Global
    public static IEnumerable<string> TargetDLLs {
        get {
            PatchLoader.InitializePatchesForAssembly(typeof(Patcher).Assembly);
            LogList(
                $"Targeting {target_dlls.Length} assemblies: ",
                target_dlls
            );

            foreach (var asm in target_dlls) {
                logger.LogInfo("Requesting assembly for patching: " + asm);
                yield return asm;
            }
        }
    }

    // See earlier comment.
    // ReSharper disable once UnusedMember.Global
    public static void Patch(ref AssemblyDefinition assemblyDefinition) {
        var name = assemblyDefinition.Name.Name;
        logger.LogInfo("Patching assembly: " + name);

        PatchLoader.PatchAssembly(ref assemblyDefinition);
        patched_dlls.Add(name + ".dll");
    }

    // See earlier comment.
    // ReSharper disable once UnusedMember.Global
    public static void Finish() {
        LogList(
            $"Patched {patched_dlls.Count} assemblies: ",
            target_dlls
        );

        var notPatched = target_dlls.Except(patched_dlls).ToList();

        if (notPatched.Count > 0) {
            LogList(
                $"{notPatched.Count} assemblies were not patched: ",
                notPatched
            );
        }
    }

    // Resolve all DLL names that BepInEx would normally support patching.
    private static IEnumerable<string> ResolveDllNames() {
        // TODO: Scrapped cos BepInEx handles patching all assemblies poorly...
        /*// Not too concerned about finding unmanaged assemblies -- BepInEx just
        // ignores our requests to patch them. Not a big deal.
        var asms = Utility.GetUniqueFilesInDirectories(
            Paths.DllSearchPaths,
            "*.dll"
        );
        asms = asms.Where(
            asm => !asm.EndsWith("System.dll")
                && !asm.EndsWith("mscorlib.dll")
        );
        return asms.Select(Path.GetFileName);*/
        yield return "Rewired_Core.dll";
        yield return "RoR2.dll";
    }

    private static void LogList(string msg, IReadOnlyList<string> items) {
        var spaces = new string(' ', msg.Length);

        for (var i = 0; i < items.Count; i++) {
            var log = i == 0 ? msg : spaces;
            logger.LogInfo(log + items[i]);
        }
    }
}
