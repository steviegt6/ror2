using System.Linq;
using Mono.Cecil;

namespace Void.Optimizer.Core;

public interface IAllowanceProvider {
    bool IsAssemblyAllowed(
        AssemblyDefinition assemblyDefinition,
        Whitelist assemblyWhitelist,
        Whitelist typeWhitelist
    );

    bool IsTypeAllowed(
        TypeDefinition typeDefinition,
        Whitelist assemblyWhitelist,
        Whitelist typeWhitelist
    );
}

public sealed class DefaultAllowanceProvider : IAllowanceProvider {
    public bool IsAssemblyAllowed(
        AssemblyDefinition assemblyDefinition,
        Whitelist assemblyWhitelist,
        Whitelist typeWhitelist
    ) {
        var items = assemblyWhitelist.Items;
        var asmName = assemblyDefinition.Name.Name;

        return assemblyWhitelist.AllowanceType switch {
            AllowanceType.Whitelist => items.Contains(asmName),
            AllowanceType.Blacklist => !items.Contains(asmName),
            AllowanceType.None => true,
            _ => false,
        };
    }

    public bool IsTypeAllowed(
        TypeDefinition typeDefinition,
        Whitelist assemblyWhitelist,
        Whitelist typeWhitelist
    ) {
        var items = typeWhitelist.Items;
        var typeName = typeDefinition.FullName;

        return typeWhitelist.AllowanceType switch {
            AllowanceType.Whitelist => items.Contains(typeName),
            AllowanceType.Blacklist => !items.Contains(typeName),
            AllowanceType.None => true,
            _ => false,
        };
    }
}
