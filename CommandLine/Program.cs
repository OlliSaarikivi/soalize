using Soalize.Transpiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soalize.CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            var original = Compilations.FromCsprojAsync(args[0]).Result;
            var transpiled = Driver.Transpile(original);
        }
    }
}
