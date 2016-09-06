using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soalize.Transpiler
{
    static class CompilationExtensions
    {
        public static INamedTypeSymbol GetSymbol(this Compilation compilation, Type type)
            => compilation.GetTypeByMetadataName(type.FullName);
    }
}
