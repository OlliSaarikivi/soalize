using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Soalize.Transpiler
{
    using SF = SyntaxFactory;

    class DirectAccessRewriter : CSharpSyntaxRewriter
    {
        TreeContext treeContext;

        public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            node = (MemberAccessExpressionSyntax)base.VisitMemberAccessExpression(node);

            var memberSymbolInfo = treeContext.Model.GetSymbolInfo(node);
            if (null == memberSymbolInfo.Symbol)
                throw new TranspilationException($"Unresolved symbol: {node}");
            var fieldSymbol = memberSymbolInfo.Symbol as IFieldSymbol;

            if (null != fieldSymbol)
            {
                var elementAccess = node.Expression as ElementAccessExpressionSyntax;
                if (null != elementAccess)
                {
                    var objectSymbolInfo = treeContext.Model.GetSymbolInfo(elementAccess.Expression);
                    if (null == objectSymbolInfo.Symbol)
                        throw new TranspilationException($"Unresolved symbol: {elementAccess.Expression}");
                    RewriteInfo rewriteInfo;
                    if (treeContext.Context.RewriteTargets.TryGetValue(objectSymbolInfo.Symbol, out rewriteInfo))
                    {
                        if (node.Kind() == SyntaxKind.PointerMemberAccessExpression)
                            throw new TranspilationException($"Unsupported member access: {node}");

                        return SF.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                            SF.member)
                        rewriteInfo.ElementFieldColumns[fieldSymbol];
                    }
                }
            }
        }
    }
}
