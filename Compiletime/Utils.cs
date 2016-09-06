using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Soalize
{
    static class Utils
    {
        internal static void WarnConstructNonTranspiled<T>()
        {
            Debug.WriteLineIf(!Debugger.IsAttached, $"WARNING: non-transpiled {typeof(T)} constructed. Expect performance degradation.");
        }

        internal static T ShallowCloneNonNull<T>(this T value)
        {
            Contract.Requires<ArgumentNullException>(null != value, "value must not be null.");
            var memberwiseClone = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)memberwiseClone.Invoke(value, null);
        }
    }
}
