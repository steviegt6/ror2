using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;
using Void.Optimizer.Core.Visitors;

namespace Void.Optimizer.Core;

internal static class PatchLoader {
    private static readonly List<IAssemblyPatch> patches = new();

    internal static void InitializePatchersForAssembly(Assembly assembly) {
        // instantiate all IVisitor types
        foreach (var type in assembly.GetTypes()) {
            if (type.IsInterface || type.IsAbstract)
                continue;

            if (type.GetInterface(nameof(IVisitor)) is null)
                continue;

            var visitor = (IVisitor)Activator.CreateInstance(type);
            var patch = CreatePatchFromVisitor(visitor);
            if (patch is not null)
                patches.Add(patch);
        }
    }

    internal static void PatchAssembly(
        ref AssemblyDefinition assemblyDefinition
    ) {
        foreach (var patch in patches)
            patch.Apply(ref assemblyDefinition);
    }
    
    private static IAssemblyPatch? CreatePatchFromVisitor(IVisitor visitor) {
        return visitor switch {
            ITypeVisitor typeVisitor => new TypeVisitorPatch(typeVisitor),
            IMemberVisitor memVisitor => new MemberVisitorPatch(memVisitor),
            _ => null,
        };
    }
}
