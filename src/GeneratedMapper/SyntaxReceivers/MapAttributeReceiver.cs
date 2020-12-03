using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace GeneratedMapper.SyntaxReceivers
{
    internal sealed class MapAttributeReceiver : ISyntaxReceiver
    {
        public List<TypeDeclarationSyntax> Candidates { get; } = new List<TypeDeclarationSyntax>();

        public List<TypeDeclarationSyntax> ClassesWithExtensionMethods { get; } = new List<TypeDeclarationSyntax>();

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

                var hasExtensionMethod = typeDeclarationSyntax.Members
                    .Where(x => x is MethodDeclarationSyntax)
                    .Select(x => x as MethodDeclarationSyntax)
                    .Any(x => x!.ParameterList.Parameters.FirstOrDefault()?.ChildTokens().Any(x => x.IsKind(SyntaxKind.ThisKeyword)) ?? false);

                if (hasExtensionMethod)
                {
                    ClassesWithExtensionMethods.Add(typeDeclarationSyntax);
                }
            }
        }
    }
}
