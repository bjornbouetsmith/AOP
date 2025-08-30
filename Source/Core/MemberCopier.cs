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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
    public class MemberCopierAttribute : Attribute
    {
        public Type DestinationType { get; set; }
    }

    public class MemberCopier
    {
        public static ClassDeclarationSyntax CreateCopierClass(INamedTypeSymbol sourceType, AttributeData attribute)
        {
            var sourceClassName = $"{sourceType.ContainingNamespace.Name}.{sourceType.Name}";

            var typeSymbol = attribute.NamedArguments[0].Value;
            var destinationType = (INamedTypeSymbol)typeSymbol.Value;


            var destinationClassName = $"{destinationType.ContainingNamespace.Name}.{destinationType.Name}";
            var className = $"Copier";
            var classDeclaration = SyntaxFactory.ClassDeclaration(className).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword));

            var sourceMembers = GetMembers(sourceType);

            var destinationMembers = GetMembers(destinationType).ToDictionary(d => d.Name);


            var assignmentExpressions = new List<SimpleExpressionDeclaration>();
            var invocationExpressions = new List<InvocationExpressionDeclaration>();
            foreach (var sourceMember in sourceMembers)
            {
                if ((!destinationMembers.TryGetValue(sourceMember.Name, out var destinationMember)))
                {
                    continue;
                }
                var destinationAccess = new MemberAccessDeclaration("destination", destinationMember.Name);
                var sourceAccess = new MemberAccessDeclaration("source", sourceMember.Name);
                if (sourceMember.CanBeCopied && destinationMember.CanBeCopied)
                {
                    // simple assignment
                    var assignment = new AssignmentDeclaration(destinationAccess, sourceAccess);
                    var expression = new SimpleExpressionDeclaration(assignment);
                    assignmentExpressions.Add(expression);
                }
                else
                {
                    // need to call CopyTo method
                    var invocation = new InvocationDeclaration(sourceAccess, "CopyTo", destinationAccess);
                    var invocationExpression = new InvocationExpressionDeclaration(invocation);
                    invocationExpressions.Add(invocationExpression);
                }

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

        private static IEnumerable<MemberData> GetMembers(INamedTypeSymbol sourceType)
        {
            var members = sourceType.GetMembers().Where(s => s.Kind == SymbolKind.Property).OfType<IPropertySymbol>();
            foreach (var member in members)
            {
                var canBeCopied = CanBeCopied(member.Type);
                var isCollection = IsCollection(member.Type);
                yield return new(member.Name, canBeCopied, isCollection);
            }
        }

        private static bool CanBeCopied(ITypeSymbol type)
        {
            return type.IsValueType
                || type.SpecialType == SpecialType.System_String;
        }

        private static bool IsCollection(ITypeSymbol type)
        {
            return type.AllInterfaces.Any(i => i.SpecialType == SpecialType.System_Collections_Generic_ICollection_T);
        }

        private readonly record struct MemberData(string Name, bool CanBeCopied, bool IsCollection);

    }
}
