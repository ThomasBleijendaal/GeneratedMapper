using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GeneratedMapper.SyntaxReceivers
{
    public class ExtensionMapCandidate
    {
        private readonly GenericNameSyntax _genericNameSyntax;

        public ExtensionMapCandidate(GenericNameSyntax genericNameSyntax) => _genericNameSyntax = genericNameSyntax;

        public GenericNameSyntax Syntax => _genericNameSyntax;
        public TypeSyntax Source => _genericNameSyntax.TypeArgumentList.Arguments[0];
        public TypeSyntax Destination  => _genericNameSyntax.TypeArgumentList.Arguments[1];
    }
}
