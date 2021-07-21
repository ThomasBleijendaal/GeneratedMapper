using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace GeneratedMapper.SyntaxReceivers
{
    internal sealed class MapAttributeReceiver : ISyntaxReceiver
    {
        public List<TypeDeclarationSyntax> Candidates { get; } = new();
        private readonly List<TypeDeclarationSyntax> _cachedSyntaxes = new();
        public List<ExtensionCandidate> ExtensionCandidates { get; } = new();
        private readonly List<ExtensionMapCandidate> _extensionCandidates = new();

        public List<TypeDeclarationSyntax> ClassesWithExtensionMethods { get; } = new();
        public List<TypeDeclarationSyntax> ClassesWithAfterMapMethods { get; } = new();

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
                _cachedSyntaxes.Add(typeDeclarationSyntax);
            }

            if (syntaxNode is MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                if (memberAccessExpressionSyntax.Name.Identifier.Text == "MapTo" && memberAccessExpressionSyntax.Name is GenericNameSyntax nameSyntax && nameSyntax.TypeArgumentList.Arguments.Count == 2)
                {
                    _extensionCandidates.Add(new ExtensionMapCandidate(nameSyntax));
                }
                //Todo: Add check here to see if method call is generic and calls MapTo in sub callings
                // Then add to extension candidate with Generic arguments correctly assigned
            }
        }

        public void TrimCandidates()
        {
            foreach (var extensionCandidate in _extensionCandidates)
            {
                var match = _cachedSyntaxes.FirstOrDefault(x => x.Identifier.Text == (extensionCandidate.Source as IdentifierNameSyntax)?.Identifier.Text);
                if (!Candidates.Contains(match))
                {
                    Candidates.Add(match);
                }
                ExtensionCandidates.Add(new ExtensionCandidate(match, _cachedSyntaxes.FirstOrDefault(x => x.Identifier.Text == (extensionCandidate.Destination as IdentifierNameSyntax)?.Identifier.Text), extensionCandidate.Syntax));
            }
            _cachedSyntaxes.Clear();
        }
    }
}
