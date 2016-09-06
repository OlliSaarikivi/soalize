using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soalize.Transpiler
{
    static class SyntaxExtensions
    {
        internal static bool IsAutoProperty(this PropertyDeclarationSyntax node)
            => node.AccessorList.Accessors.All(x => x.Body == null);
    }
}
