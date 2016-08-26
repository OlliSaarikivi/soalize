using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;

namespace Soalize
{
    class CommandLineReferenceResolver : MetadataReferenceResolver
    {
        RelativePathResolver pathResolver;

        public CommandLineReferenceResolver(RelativePathResolver pathResolver)
        {
            Debug.Assert(pathResolver != null);

            this.pathResolver = pathResolver;
        }

        public override ImmutableArray<PortableExecutableReference> ResolveReference(string reference, string baseFilePath, MetadataReferenceProperties properties)
        {
            string fullPath = pathResolver.ResolvePath(reference, baseFilePath);

            if (fullPath != null)
            {
                return ImmutableArray.Create(MetadataReference.CreateFromFile(fullPath, properties));
            }

            return ImmutableArray<PortableExecutableReference>.Empty;
        }

        public override bool Equals(object other)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
