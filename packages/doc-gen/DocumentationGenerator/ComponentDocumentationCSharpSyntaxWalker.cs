using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocumentationGenerator
{
    public class ComponentDocumentationCSharpSyntaxWalker : SyntaxWalker
    {
        private readonly Action<Comment> _commentLocated;

        public ComponentDocumentationCSharpSyntaxWalker(Action<Comment> commentLocated)
            : base(SyntaxWalkerDepth.StructuredTrivia)
        {
            if (commentLocated == null)
                throw new ArgumentNullException("commentLocated");

            _commentLocated = commentLocated;
        }

        protected override void VisitTrivia(SyntaxTrivia trivia)
        {
            if (_commentTypes.Contains(trivia.Kind()))
            {
                string triviaContent;
                using (var writer = new StringWriter())
                {
                    trivia.WriteTo(writer);
                    triviaContent = writer.ToString();
                }

                // Note: When looking for the containingMethodOrPropertyIfAny, we want MemberDeclarationSyntax
                // types such as ConstructorDeclarationSyntax, MethodDeclarationSyntax, IndexerDeclarationSyntax,
                // PropertyDeclarationSyntax but NamespaceDeclarationSyntax and TypeDeclarationSyntax also
                // inherit from MemberDeclarationSyntax and we don't want those
                var containingNode = trivia.Token.Parent;
                var containingMethodOrPropertyIfAny = TryToGetContainingNode<MemberDeclarationSyntax>(
                    containingNode,
                    n => !(n is NamespaceDeclarationSyntax) && !(n is TypeDeclarationSyntax)
                );
                var containingTypeIfAny = TryToGetContainingNode<TypeDeclarationSyntax>(containingNode);
                var containingNameSpaceIfAny = TryToGetContainingNode<NamespaceDeclarationSyntax>(containingNode);
                _commentLocated(new Comment(
                    triviaContent,
                    trivia.SyntaxTree.GetLineSpan(trivia.Span).StartLinePosition.Line,
                    containingMethodOrPropertyIfAny,
                    containingTypeIfAny,
                    containingNameSpaceIfAny
                ));
            }

            base.VisitTrivia(trivia);
        }

        private static HashSet<SyntaxKind> _commentTypes = new HashSet<SyntaxKind>(new[]
        {
            SyntaxKind.SingleLineCommentTrivia,
            SyntaxKind.MultiLineCommentTrivia,
            SyntaxKind.DocumentationCommentExteriorTrivia,
            SyntaxKind.SingleLineDocumentationCommentTrivia,
            SyntaxKind.MultiLineDocumentationCommentTrivia
        });

        private T TryToGetContainingNode<T>(SyntaxNode node, Predicate<T> optionalFilter = null)
            where T : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException("node");

            var currentNode = node;
            while (true)
            {
                var nodeOfType = currentNode as T;
                if (nodeOfType != null)
                {
                    if ((optionalFilter == null) || optionalFilter(nodeOfType))
                        return nodeOfType;
                }

                if (currentNode.Parent == null)
                    break;
                currentNode = currentNode.Parent;
            }

            return null;
        }
    }
}
