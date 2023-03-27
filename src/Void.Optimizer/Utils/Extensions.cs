using System.Linq;
using Mono.Cecil;

namespace Void.Optimizer.Utils;

public static class Extensions {
    public static TypeDefinition? GetType(
        this AssemblyDefinition assemblyDefinition,
        string typeName
    ) {
        return assemblyDefinition.MainModule.GetType(typeName);
    }
    
    // extension to get method from TypeDefinition
    public static MethodDefinition? GetMethod(
        this TypeDefinition typeDefinition,
        string methodName
    ) {
        return typeDefinition.Methods.FirstOrDefault(m => m.Name == methodName);
    }
}
