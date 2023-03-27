using System;

namespace Void.Optimizer.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AssemblyWhitelistAttribute : Attribute {
    public AllowanceType AllowanceType { get; }

    public string[] Assemblies { get; }

    public AssemblyWhitelistAttribute(
        AllowanceType allowanceType,
        params string[] assemblies
    ) {
        AllowanceType = allowanceType;
        Assemblies = assemblies;
    }
    
    public Whitelist ToWhitelist() {
        return new Whitelist(AllowanceType, Assemblies);
    }
}
