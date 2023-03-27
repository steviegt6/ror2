using Mono.Cecil;

namespace Void.Optimizer.Core.Visitors;

public interface IMemberVisitor : IVisitor {
    void Visit(ref MethodDefinition methodDefinition);

    void Visit(ref FieldDefinition fieldDefinition);

    void Visit(ref PropertyDefinition propertyDefinition);

    void Visit(ref EventDefinition eventDefinition);
}
