using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
using Mono.Cecil;
using Void.Optimizer.Core.Visitors;

namespace Void.Optimizer.Core;

internal static class PatchLoader {
    private static readonly ManualLogSource logger =
        Logger.CreateLogSource(typeof(PatchLoader).FullName);

    private static readonly List<IAssemblyPatch> patches = new();

    internal static void InitializePatchesForAssembly(Assembly assembly) {
        logger.LogInfo("Loading patches for assembly: " + assembly.FullName);

        foreach (var type in assembly.GetTypes()) {
            if (type.IsInterface || type.IsAbstract)
                continue;

            if (type.GetInterface(nameof(IVisitor)) is null)
                continue;

            var visitor = (IVisitor)Activator.CreateInstance(type);
            var patch = CreatePatchFromVisitor(visitor);
            if (patch is null)
                continue;

            logger.LogInfo("Loaded patch: " + type.FullName);
            patches.Add(patch);
        }
    }

    internal static void PatchAssembly(
        ref AssemblyDefinition assemblyDefinition
    ) {
        logger.LogInfo("Patching assembly: " + assemblyDefinition.Name.Name);

        foreach (var patch in patches) {
            logger.LogInfo("Applying patch: " + patch.GetType().FullName);
            patch.Apply(ref assemblyDefinition);
        }
    }

    private static IAssemblyPatch? CreatePatchFromVisitor(IVisitor visitor) {
        return visitor switch {
            ITypeVisitor typeVisitor => new TypeVisitorPatch(typeVisitor),
            IMemberVisitor memVisitor => new MemberVisitorPatch(memVisitor),
            _ => null,
        };
    }
}
