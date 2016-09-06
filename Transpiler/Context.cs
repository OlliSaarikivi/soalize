using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soalize.Transpiler
{
    class Context
    {
        public Compilation Compilation { get; private set; }
        public INamedTypeSymbol UnboundValueArrayType { get; private set; }
        public Dictionary<SyntaxTree, TreeContext> TreeContexts { get; private set; }

        public Dictionary<ISymbol, RewriteInfo> RewriteTargets { get; set; }
        
        public Context(Compilation compilation)
        {
            Compilation = compilation;
            UnboundValueArrayType = compilation.GetSymbol(typeof(ValueArray<>));
            TreeContexts = compilation.SyntaxTrees.ToDictionary(x => x, x => new TreeContext(this, x));
        }
    }

    class TreeContext
    {
        public Context Context { get; private set; }
        public SyntaxTree Tree { get; private set; }
        public SyntaxNode Root { get; private set; }
        public SemanticModel Model { get; private set; }

        public TreeContext(Context context, SyntaxTree tree)
        {
            Context = context;
            Tree = tree;
            Root = tree.GetRoot();
            Model = context.Compilation.GetSemanticModel(tree);
        }
    }
}
