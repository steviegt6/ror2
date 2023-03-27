using Mono.Cecil;

namespace Void.Optimizer.Core.Visitors; 

public interface ITypeVisitor : IVisitor {
    void Visit(ref TypeDefinition typeDefinition);

    void VisitNested(ref TypeDefinition nestedTypeDefinition);
}
