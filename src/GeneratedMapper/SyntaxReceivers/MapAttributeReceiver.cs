using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace GeneratedMapper.SyntaxReceivers
{
    public sealed class MapAttributeReceiver : ISyntaxReceiver
    {
        public List<TypeDeclarationSyntax> Candidates { get; } = new List<TypeDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is TypeDeclarationSyntax typeDeclarationSyntax)
            {
                if (typeDeclarationSyntax.AttributeLists.Any(
                    x => x.Attributes.Any(
                        x => x.Name.ToString().Contains("MapTo") || x.Name.ToString().Contains("MapFrom"))))
                {
                    Candidates.Add(typeDeclarationSyntax);
                }
            }
        }
    }
}
