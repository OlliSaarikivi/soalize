using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soalize.Transpiler
{
    using SF = SyntaxFactory;

    static class ValueArraySpecializer
    {
        internal static void Specialize(Context context, RewriteInfo rewriteInfo)
        {
            rewriteInfo.ElementFieldColumns = rewriteInfo.ElementType.GetFields().Select(field =>
            {
                var declaredSymbol = field.IsImplicitlyDeclared ? field.AssociatedSymbol : field;
                if (null != declaredSymbol)
                    return new { field, groupName = GetGroupName(context, declaredSymbol) };
                else
                    throw new TranspilationException($"Could not find associated symbol for implicitly declared field: {field}");
            }).ToDictionary(x => x.field, x => x.groupName);

            rewriteInfo.Specialization = SF.ClassDeclaration($"{context.UnboundValueArrayType.Name}_{rewriteInfo.ElementType.Name}")
                .WithMembers(SF.List(GenerateMembers(context, rewriteInfo)));
        }

        static IEnumerable<MemberDeclarationSyntax> GenerateMembers(Context context, RewriteInfo rewriteInfo) =>
            rewriteInfo.ElementFieldColumns.GroupBy(x => x.Value).SelectMany(x => ColumnToMembers(x.Key, x.Select(y => y.Key)));

        static string GetGroupName(Context context, ISymbol symbol)
        {
            var column = symbol.GetAttribute(typeof(ColumnAttribute), context.Compilation);
            if (null != column)
            {
                var namedGroupName = column.NamedArguments.Select(x => new { x.Key, x.Value })
                    .FirstOrDefault(x => x.Key == nameof(ColumnAttribute.Group))?.Value.Value as string;
                if (null != namedGroupName)
                    return $"g_{symbol.Name}";
                else
                    return $"_{symbol.Name}";
            }
            else
            {
                return "_";
            }
        }

        static IEnumerable<MemberDeclarationSyntax> ColumnToMembers(string column, IEnumerable<IFieldSymbol> fields)
        {
            var fieldsList = fields.ToList();
            TypeSyntax columnElementType;
            if (fieldsList.Count > 1)
            {
                var fieldGroupTypeName = SF.IdentifierName($"{column}_t");
                yield return SF.StructDeclaration(fieldGroupTypeName.Identifier)
                    .WithMembers(SF.List(fields.Select(x => SF.FieldDeclaration(SF.VariableDeclaration(
                        SF.IdentifierName(x.Type.Name),
                        SF.SingletonSeparatedList(SF.VariableDeclarator(x.Name))))).Cast<MemberDeclarationSyntax>()));
                columnElementType = fieldGroupTypeName;
            }
            else
            {
                columnElementType = SF.IdentifierName(fields.First().Type.Name);
            }

            yield return SF.FieldDeclaration(SF.VariableDeclaration(
                SF.ArrayType(columnElementType, SF.SingletonList(SF.ArrayRankSpecifier())),
                SF.SingletonSeparatedList(SF.VariableDeclarator(column))));
        }
    }
}
