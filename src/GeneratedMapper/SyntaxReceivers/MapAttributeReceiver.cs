﻿using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneratedMapper.SyntaxReceivers
{
    internal sealed class MapAttributeReceiver : ISyntaxReceiver
    {
        public List<TypeDeclarationSyntax> Candidates { get; } = new();
        private readonly List<TypeDeclarationSyntax> _cachedSyntaxes = new();
        public List<ExtensionCandidate> ExtensionCandidates { get; } = new();
        private readonly List<ExtensionMapCandidate> _extensionCandidates = new();
        public List<ExtensionCandidate> ProjectionCanidates { get; } = new();
        private readonly List<ExtensionMapCandidate> _projectionCandidates = new();

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

                var hasAfterMapMethod = methods.Any(
                    x => x.ParameterList.Parameters.Count >= 2 &&
                        x.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)) &&
                        x.ReturnType.ChildTokens().Any(x => x.IsKind(SyntaxKind.VoidKeyword) || x.ValueText.Equals(nameof(Task))));

                if (hasAfterMapMethod)
                {
                    ClassesWithAfterMapMethods.Add(typeDeclarationSyntax);
                }
                _cachedSyntaxes.Add(typeDeclarationSyntax);
            }

            if (syntaxNode is MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                if (memberAccessExpressionSyntax.Name is GenericNameSyntax nameSyntax && nameSyntax.TypeArgumentList.Arguments.Count == 2)
                {
                    switch (memberAccessExpressionSyntax.Name.Identifier.Text)
                    {
                        case "MapTo":
                            _extensionCandidates.Add(new ExtensionMapCandidate(nameSyntax));
                            break;
                        case "ProjectTo":
                            _projectionCandidates.Add(new ExtensionMapCandidate(nameSyntax));
                            break;
                    }
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

            foreach (var projectionCandidate in _projectionCandidates)
            {
                var match = _cachedSyntaxes.FirstOrDefault(x => x.Identifier.Text == (projectionCandidate.Source as IdentifierNameSyntax)?.Identifier.Text);
                if (!Candidates.Contains(match))
                {
                    Candidates.Add(match);
                }
                ProjectionCanidates.Add(new ExtensionCandidate(match, _cachedSyntaxes.FirstOrDefault(x => x.Identifier.Text == (projectionCandidate.Destination as IdentifierNameSyntax)?.Identifier.Text), projectionCandidate.Syntax));
            }

            _cachedSyntaxes.Clear();
        }
    }
}
