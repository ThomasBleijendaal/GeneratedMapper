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
        public List<TypeDeclarationSyntax> Candidates { get; } = new List<TypeDeclarationSyntax>();
        private List<TypeDeclarationSyntax> _cachedSyntaxes = new List<TypeDeclarationSyntax>();
        public List<Tuple<TypeDeclarationSyntax, TypeDeclarationSyntax>> ExtensionCandidates { get; } = new List<Tuple<TypeDeclarationSyntax, TypeDeclarationSyntax>>();
        private List<Tuple<TypeSyntax, TypeSyntax>> _extensionCandidates = new List<Tuple<TypeSyntax, TypeSyntax>>();

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
                _cachedSyntaxes.Add(typeDeclarationSyntax);
            }

            if (syntaxNode is MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                if (memberAccessExpressionSyntax.Name.Identifier.Text == "MapTo" && memberAccessExpressionSyntax.Name is GenericNameSyntax nameSyntax && nameSyntax.TypeArgumentList.Arguments.Count == 2)
                {
                    _extensionCandidates.Add(new Tuple<TypeSyntax, TypeSyntax>(nameSyntax.TypeArgumentList.Arguments[0], nameSyntax.TypeArgumentList.Arguments[1]));
                }
            }
        }

        public void TrimCandidates()
        {
            foreach (var extensionCandidate in _extensionCandidates)
            {
                var match = _cachedSyntaxes.FirstOrDefault(x => x.Identifier.Text == (extensionCandidate.Item1 as IdentifierNameSyntax)?.Identifier.Text);
                if (!Candidates.Contains(match))
                {
                    Candidates.Add(match);
                    ExtensionCandidates.Add(new Tuple<TypeDeclarationSyntax, TypeDeclarationSyntax>(match, _cachedSyntaxes.FirstOrDefault(x => x.Identifier.Text == (extensionCandidate.Item2 as IdentifierNameSyntax)?.Identifier.Text)));
                }
            }
            _cachedSyntaxes.Clear();
        }
    }
}
