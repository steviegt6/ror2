using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using Mono.Cecil;
using Void.Optimizer.Core;

namespace Void.Optimizer;

// I would love to use UsedImplicitly, but Unity includes it themselves which
// would cause it to get loaded at runtime. Incredible, lol.
// TODO: Include JetBrains.Annotations with aliases.
// ReSharper disable once UnusedType.Global
public static class Patcher {
    // See earlier comment.
    // ReSharper disable once UnusedMember.Global
    public static IEnumerable<string> TargetDLLs => ResolveDllNames();

    // See earlier comment.
    // ReSharper disable once UnusedMember.Global
    public static void Patch(ref AssemblyDefinition assemblyDefinition) {
        PatchLoader.InitializePatchersForAssembly(typeof(Patcher).Assembly);
        PatchLoader.PatchAssembly(ref assemblyDefinition);
    }

    // Resolve all DLL names that BepInEx would normally support patching.
    private static IEnumerable<string> ResolveDllNames() {
        // Not too concerned about finding unmanaged assemblies -- BepInEx just
        // ignores our requests to patch them. Not a big deal.
        var asms = Utility.GetUniqueFilesInDirectories(
            Paths.DllSearchPaths,
            "*.dll"
        );
        asms = asms.Where(
            asm => !asm.EndsWith("System.dll")
                && !asm.EndsWith("mscorlib.dll")
        );
        return asms.Select(Path.GetFileName);
    }
}
