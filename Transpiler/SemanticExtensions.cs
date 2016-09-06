using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soalize.Transpiler
{
    static class SemanticExtensions
    {
        public static IEnumerable<IFieldSymbol> GetFields(this ITypeSymbol type) =>
            type.GetMembers().Where(x => x.Kind == SymbolKind.Field).Cast<IFieldSymbol>();
        
        public static IEnumerable<IPropertySymbol> GetProperties(this ITypeSymbol type) =>
            type.GetMembers().Where(x => x.Kind == SymbolKind.Property).Cast<IPropertySymbol>();

        public static IEnumerable<IMethodSymbol> GetMethods(this ITypeSymbol type) =>
            type.GetMembers().Where(x => x.Kind == SymbolKind.Method).Cast<IMethodSymbol>();

        public static bool HasAttribute(this ISymbol symbol, Type attribute, Compilation compilation) =>
            null != symbol.GetAttribute(attribute, compilation);

        public static AttributeData GetAttribute(this ISymbol symbol, Type attribute, Compilation compilation) =>
            symbol.GetAttributes(attribute, compilation).FirstOrDefault();

        public static IEnumerable<AttributeData> GetAttributes(this ISymbol symbol, Type attribute, Compilation compilation) =>
            symbol.GetAttributes().Where(x => x.AttributeClass == compilation.GetSymbol(attribute));
    }
}
