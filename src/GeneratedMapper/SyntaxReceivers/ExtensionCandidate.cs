using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GeneratedMapper.SyntaxReceivers
{
    internal sealed class ExtensionCandidate
    {
        public ExtensionCandidate(TypeDeclarationSyntax source, TypeDeclarationSyntax destination, GenericNameSyntax syntax)
        {
            Source = source;
            Destination = destination;
            Syntax = syntax;
        }

        public TypeDeclarationSyntax Source { get; }
        public TypeDeclarationSyntax Destination { get; }
        public GenericNameSyntax Syntax { get; }
    }
}
