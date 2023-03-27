using System;

namespace Void.Optimizer.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class TypeWhitelistAttribute : Attribute {
    public AllowanceType AllowanceType { get; }

    public string[] Types { get; }

    public TypeWhitelistAttribute(
        AllowanceType allowanceType,
        params string[] types
    ) {
        AllowanceType = allowanceType;
        Types = types;
    }
    
    public Whitelist ToWhitelist() {
        return new Whitelist(AllowanceType, Types);
    }
}
