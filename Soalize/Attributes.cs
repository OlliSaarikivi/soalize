using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soalize
{
    [System.AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    sealed class SoalizeAttribute : Attribute { }
}
