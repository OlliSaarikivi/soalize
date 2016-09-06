using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Soalize.Transpiler
{
    public static class Compilations
    {
        public static async Task<Compilation> FromCsprojAsync(string path, CancellationToken cancellationToken = default(CancellationToken))
        {
            var workspace = Microsoft.CodeAnalysis.MSBuild.MSBuildWorkspace.Create();
            var project = await workspace.OpenProjectAsync(path, cancellationToken);
            return await project.GetCompilationAsync();
        }
    }
}
