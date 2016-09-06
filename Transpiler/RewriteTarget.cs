using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Soalize.Transpiler
{
    class RewriteInfo
    {
        internal enum FieldAccessMode
        {
            SoAoS, SoA
        }

        internal INamedTypeSymbol Type { get; set; }
        internal ITypeSymbol ElementType { get; set; }
        internal Dictionary<IFieldSymbol, Tuple<string, FieldAccessMode>> ElementFieldColumns { get; set; }
        internal SyntaxNode Specialization { get; set; }
    }

    class TargetResolver : CSharpSyntaxWalker
    {
        TreeContext treeContext;
        Action<ISymbol, INamedTypeSymbol> targetFoundAction;

        internal TargetResolver(TreeContext treeContext, Action<ISymbol, INamedTypeSymbol> targetFoundAction)
        {
            this.treeContext = treeContext;
            this.targetFoundAction = targetFoundAction;
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            base.VisitFieldDeclaration(node);

            var type = treeContext.Model.GetTypeInfo(node.Declaration.Type).Type as INamedTypeSymbol;
            if (type?.ConstructedFrom == treeContext.Context.UnboundValueArrayType)
            {
                foreach (var variable in node.Declaration.Variables)
                {
                    var symbol = treeContext.Model.GetDeclaredSymbol(variable);
                    targetFoundAction(symbol, type);
                }
            }
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            base.VisitPropertyDeclaration(node);

            var type = treeContext.Model.GetTypeInfo(node.Type).Type as INamedTypeSymbol;
            if (type?.ConstructedFrom == treeContext.Context.UnboundValueArrayType)
            {
                if (node.IsAutoProperty())
                {
                    var symbol = treeContext.Model.GetDeclaredSymbol(node);
                    targetFoundAction(symbol, type);
                }
            }
        }
    }
}
