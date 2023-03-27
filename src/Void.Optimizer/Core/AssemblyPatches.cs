using System.Reflection;
using Mono.Cecil;
using Void.Optimizer.Core.Attributes;
using Void.Optimizer.Core.Visitors;

namespace Void.Optimizer.Core;

public interface IAssemblyPatch {
    void Apply(ref AssemblyDefinition assemblyDefinition);
}

internal abstract class VisitorPatch<T> : IAssemblyPatch where T : IVisitor {
    protected abstract T Visitor { get; }

    public void Apply(ref AssemblyDefinition assemblyDefinition) {
        var type = Visitor.GetType();
        var asmList = type.GetCustomAttribute<AssemblyWhitelistAttribute>();
        var typeList = type.GetCustomAttribute<TypeWhitelistAttribute>();
        asmList ??= new AssemblyWhitelistAttribute(AllowanceType.None);
        typeList ??= new TypeWhitelistAttribute(AllowanceType.None);

        IAllowanceProvider allowanceProvider;
        if (Visitor is IAllowanceProvider asmProvider)
            allowanceProvider = asmProvider;
        else
            allowanceProvider = new DefaultAllowanceProvider();

        DoApply(
            ref assemblyDefinition,
            asmList.ToWhitelist(),
            typeList.ToWhitelist(),
            allowanceProvider
        );
    }

    protected abstract void DoApply(
        ref AssemblyDefinition assemblyDefinition,
        Whitelist assemblyWhitelist,
        Whitelist typeWhitelist,
        IAllowanceProvider allowanceProvider
    );
}

internal sealed class TypeVisitorPatch : VisitorPatch<ITypeVisitor> {
    protected override ITypeVisitor Visitor { get; }

    public TypeVisitorPatch(ITypeVisitor visitor) {
        Visitor = visitor;
    }

    protected override void DoApply(
        ref AssemblyDefinition assemblyDefinition,
        Whitelist assemblyWhitelist,
        Whitelist typeWhitelist,
        IAllowanceProvider allowanceProvider
    ) {
        var assemblyAllowed = allowanceProvider.IsAssemblyAllowed(
            assemblyDefinition,
            assemblyWhitelist,
            typeWhitelist
        );

        if (!assemblyAllowed)
            return;

        for (var i = 0; i < assemblyDefinition.MainModule.Types.Count; i++) {
            var type = assemblyDefinition.MainModule.Types[i];
            var typeAllowed = allowanceProvider.IsTypeAllowed(
                type,
                assemblyWhitelist,
                typeWhitelist
            );

            if (!typeAllowed)
                continue;

            Visitor.Visit(ref type);
            RecurseNested(type);
            assemblyDefinition.MainModule.Types[i] = type;
        }
    }

    private void RecurseNested(TypeDefinition type) {
        if (!type.HasNestedTypes)
            return;

        for (var i = 0; i < type.NestedTypes.Count; i++) {
            var nested = type.NestedTypes[i];
            Visitor.VisitNested(ref nested);
            RecurseNested(nested);
            type.NestedTypes[i] = nested;
        }
    }
}

internal sealed class MemberVisitorPatch : VisitorPatch<IMemberVisitor> {
    protected override IMemberVisitor Visitor { get; }

    public MemberVisitorPatch(IMemberVisitor visitor) {
        Visitor = visitor;
    }

    protected override void DoApply(
        ref AssemblyDefinition assemblyDefinition,
        Whitelist assemblyWhitelist,
        Whitelist typeWhitelist,
        IAllowanceProvider allowanceProvider
    ) {
        var assemblyAllowed = allowanceProvider.IsAssemblyAllowed(
            assemblyDefinition,
            assemblyWhitelist,
            typeWhitelist
        );

        if (!assemblyAllowed)
            return;

        foreach (var type in assemblyDefinition.MainModule.Types) {
            RecurseType(
                type,
                assemblyWhitelist,
                typeWhitelist,
                allowanceProvider
            );
        }
    }

    private void RecurseType(
        TypeDefinition typeDefinition,
        Whitelist assemblyWhitelist,
        Whitelist typeWhitelist,
        IAllowanceProvider allowanceProvider
    ) {
        var typeAllowed = allowanceProvider.IsTypeAllowed(
            typeDefinition,
            assemblyWhitelist,
            typeWhitelist
        );

        if (!typeAllowed)
            return;

        if (typeDefinition.HasMethods) {
            for (var i = 0; i < typeDefinition.Methods.Count; i++) {
                var methodDef = typeDefinition.Methods[i];
                Visitor.Visit(ref methodDef);
                typeDefinition.Methods[i] = methodDef;
            }
        }

        if (typeDefinition.HasFields) {
            for (var i = 0; i < typeDefinition.Fields.Count; i++) {
                var fieldDef = typeDefinition.Fields[i];
                Visitor.Visit(ref fieldDef);
                typeDefinition.Fields[i] = fieldDef;
            }
        }

        if (typeDefinition.HasProperties) {
            for (var i = 0; i < typeDefinition.Properties.Count; i++) {
                var propertyDef = typeDefinition.Properties[i];
                Visitor.Visit(ref propertyDef);
                typeDefinition.Properties[i] = propertyDef;
            }
        }

        if (typeDefinition.HasEvents) {
            for (var i = 0; i < typeDefinition.Events.Count; i++) {
                var eventDef = typeDefinition.Events[i];
                Visitor.Visit(ref eventDef);
                typeDefinition.Events[i] = eventDef;
            }
        }

        if (!typeDefinition.HasNestedTypes)
            return;

        foreach (var nested in typeDefinition.NestedTypes) {
            RecurseType(
                nested,
                assemblyWhitelist,
                typeWhitelist,
                allowanceProvider
            );
        }
    }
}
