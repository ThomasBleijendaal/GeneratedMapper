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
        public List<TypeDeclarationSyntax> ClassesWithAfterMapMethods { get; } = new List<TypeDeclarationSyntax>();

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

                var methods = typeDeclarationSyntax.Members.OfType<MethodDeclarationSyntax>().ToArray();

                var hasExtensionMethod = methods.Any(x =>
                    x!.ParameterList.Parameters.FirstOrDefault()?.ChildTokens()
                        .Any(x => x.IsKind(SyntaxKind.ThisKeyword)) ?? false);
                if (hasExtensionMethod)
                {
                    ClassesWithExtensionMethods.Add(typeDeclarationSyntax);
                }

                var hasAfterMapMethod = methods.Any(x =>
                    x.ParameterList.Parameters.Count >= 2 &&
                    x.Modifiers.Any(m => m.Kind() == SyntaxKind.StaticKeyword) &&
                    (x.ReturnType as PredefinedTypeSyntax)?.Keyword.Text == "void");
                if (hasAfterMapMethod)
                {
                    ClassesWithAfterMapMethods.Add(typeDeclarationSyntax);
                }
            }
        }
    }
}
