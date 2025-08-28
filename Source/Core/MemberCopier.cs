using Core.Declarations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using Public;

namespace Core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class MemberCopierAttribute : Attribute
    {
        public Type DestinationType { get; set; }
    }
    public class MemberCopier
    {
        public static ClassDeclarationSyntax CreateCopierClass(INamedTypeSymbol sourceType)
        {
            var sourceClassName = $"{sourceType.ContainingNamespace.Name}.{sourceType.Name}";
            var attributes = sourceType.GetAttributes();
            var attr = attributes[0];
            var typeSymbol = attr.NamedArguments[0].Value;
            var destinationType = (INamedTypeSymbol)typeSymbol.Value;


            var destinationClassName = $"{destinationType.ContainingNamespace.Name}.{destinationType.Name}";
            var className = $"Copier";
            var classDeclaration = SyntaxFactory.ClassDeclaration(className).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword));

            var sourceMembers = GetMembers(sourceType).Select(s => new MemberAccessDeclaration("source", s));
            var sourceMembersCannotBeCopied = GetMembers(sourceType,false).Select(s => new MemberAccessDeclaration("source", s));
            var destinationMembers = GetMembers(destinationType).Select(s => new MemberAccessDeclaration("destination", s)).ToDictionary(d => d.MemberName);
            var destinationMemberssCannotBeCopied = GetMembers(destinationType, false).Select(s => new MemberAccessDeclaration("destination", s)).ToDictionary(d => d.MemberName);

            var assignmentExpressions = new List<SimpleExpressionDeclaration>();
            var invocationExpressions = new List<InvocationExpressionDeclaration>();
            foreach (var sourceMember in sourceMembers)
            {
                if ((!destinationMembers.TryGetValue(sourceMember.MemberName, out var destinationMember)))
                {
                    continue;
                }
                var assignment = new AssignmentDeclaration(destinationMember, sourceMember);
                var expression = new SimpleExpressionDeclaration(assignment);
                assignmentExpressions.Add(expression);
            }

            foreach (var sourceMember in sourceMembersCannotBeCopied)
            {
                if ((!destinationMemberssCannotBeCopied.TryGetValue(sourceMember.MemberName, out var destinationMember)))
                {
                    continue;
                }
                var invocation = new InvocationDeclaration(sourceMember, "CopyTo", destinationMember);
                var invocationExpression = new InvocationExpressionDeclaration(invocation);
                invocationExpressions.Add(invocationExpression);
            }
            var bodyStatements = invocationExpressions.Select(e => e.ExpressionStatement).Concat(assignmentExpressions.Select(e => e.ExpressionStatement)).ToArray();
            var body = new BodyDeclaration(bodyStatements);
            var method = new MethodDeclaration("Copy", AccessModifier.Public | AccessModifier.Static,
                new SmallList<ParameterDeclaration>(
                    new ParameterDeclaration(sourceClassName, "source", true, false),
                    new ParameterDeclaration(destinationClassName, "destination", false, false)
                ),
                new SmallList<ParameterDeclaration>(), body);
            classDeclaration = classDeclaration.AddMembers(method.MethodDeclarationSyntax);

            return classDeclaration;


        }

        private static IEnumerable<string> GetMembers(INamedTypeSymbol sourceType, bool canBeCopied = true)
        {
            var members = sourceType.GetMembers().Where(s => s.Kind == SymbolKind.Property).OfType<IPropertySymbol>();
            foreach (var member in members.Where(m => CanBeCopied(m.Type) == canBeCopied))
            {

                yield return member.Name;

            }
        }

        private static bool CanBeCopied(ITypeSymbol type)
        {
            return type.IsValueType
                || type.SpecialType == SpecialType.System_String;
        }

    }
}
