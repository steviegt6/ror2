using System.Collections.Generic;
using Mono.Cecil;

namespace Void.Optimizer;

// I would love to use UsedImplicitly, but Unity includes it themselves which
// would cause it to get loaded at runtime. Incredible, lol.
// TODO: Include JetBrains.Annotations with aliases.
// ReSharper disable once UnusedType.Global
public static class Patcher {
    // See earlier comment.
    // ReSharper disable once UnusedMember.Global
    public static IEnumerable<string> TargetDLLs {
        get {
            yield break;
        }
    }

    // See earlier comment.
    // ReSharper disable once UnusedMember.Global
    public static void Patch(ref AssemblyDefinition asmDef) {
        _ = asmDef;
    }
}
