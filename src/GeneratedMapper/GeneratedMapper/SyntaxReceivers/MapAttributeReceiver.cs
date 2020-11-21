using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace GeneratedMapper.SyntaxReceivers
{
    public sealed class MapAttributeReceiver : ISyntaxReceiver
    {
        public List<TypeDeclarationSyntax> Candidates { get; } = new List<TypeDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is TypeDeclarationSyntax typeDeclarationSyntax)
            {
                foreach (var attributeList in typeDeclarationSyntax.AttributeLists)
                {
                    foreach (var attribute in attributeList.Attributes)
                    {
                        if (attribute.Name.ToString().Contains("MapTo") || attribute.Name.ToString().Contains("MapFrom"))
                        {
                            Candidates.Add(typeDeclarationSyntax);
                        }
                    }
                }
            }
        }
    }
}
