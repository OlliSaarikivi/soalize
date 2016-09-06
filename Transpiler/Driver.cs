using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soalize.Transpiler
{
    public static class Driver
    {
        public static Compilation Transpile(Compilation compilation)
        {
            var context = new Context(compilation);

            ResolveTargets(context);
            AddSpecializations(context);

            return compilation;
        }
        
        internal static void ResolveTargets(Context context)
        {
            var symbols = new Dictionary<ISymbol, RewriteInfo>();

            foreach (var tree in context.Compilation.SyntaxTrees)
            {
                var treeContext = context.TreeContexts[tree];
                new TargetResolver(treeContext, (symbol, type) =>
                {
                    if (!symbols.ContainsKey(symbol))
                    {
                        var elementType = type.TypeArguments[0] as INamedTypeSymbol;
                        if (elementType != null)
                            symbols.Add(symbol, new RewriteInfo
                            {
                                Type = type,
                                ElementType = elementType,
                            });
                    }
                }).Visit(treeContext.Root);
            }

            context.RewriteTargets = symbols;
        }

        internal static void AddSpecializations(Context context)
        {
            foreach (var target in context.RewriteTargets.Values)
            {
                ValueArraySpecializer.Specialize(context, target);
            }
        }
    }
}
