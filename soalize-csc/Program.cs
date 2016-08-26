using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Soalize
{
    class Program
    {
        const string ResponseFileName = "csc.rsp";

        static void Main(string[] args)
        {
            var cscType = typeof(CSharpCompilation).Assembly.GetType("Microsoft.CodeAnalysis.CSharp.CSharpCompiler");
            var assemblyLoader = Activator.CreateInstance(typeof(Compilation).Assembly.GetType("Microsoft.CodeAnalysis.DesktopAnalyzerAssemblyLoader"));
            var constructorParameterTypes = new Type[] {
                typeof(CSharpCommandLineParser), // parser
                typeof(string), // responseFile
                typeof(string[]), // args
                typeof(string), // clientDirectory
                typeof(string), // baseDirectory
                typeof(string), // sdkDirectoryOpt
                typeof(string), // additionalReferenceDirectories
                typeof(IAnalyzerAssemblyLoader), // analyzerLoader
            };
            cscType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic, null, constructorParameterTypes, null);
            
            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Assembly"), AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule("Module");
            var type = module.DefineType("Class", TypeAttributes.Public, cscType);
            
            var constructor = type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, constructorParameterTypes);
            var generator = constructor.GetILGenerator();

            var instance = Activator.CreateInstance(type.CreateType(),
                CSharpCommandLineParser.Default,
                Path.Combine(AppContext.BaseDirectory, ResponseFileName),
                args,
                AppContext.BaseDirectory,
                null,
                Environment.GetEnvironmentVariable("LIB"),
                assemblyLoader
                );
            
            return;

            //IEnumerable<string> allArgs = args;
            //var responseFile = Path.Combine(AppContext.BaseDirectory, ResponseFileName);
            //Debug.Assert(null == responseFile || Path.IsPathRooted(responseFile));
            //if (!SuppressDefaultResponseFile(args) && File.Exists(responseFile))
            //{
            //    allArgs = new[] { "@" + responseFile }.Concat(allArgs);
            //}

            //var arguments = CSharpCommandLineParser.Default.Parse(allArgs, Directory.GetCurrentDirectory(), null, Environment.GetEnvironmentVariable("LIB"));

            //var parseOptions = arguments.ParseOptions;
            //var scriptParseOptions = parseOptions.WithKind(SourceCodeKind.Script);

            //var sourceFiles = arguments.SourceFiles;
            //var trees = new SyntaxTree[sourceFiles.Length];

            //for (int i = 0; i < sourceFiles.Length; i++)
            //{
            //    var file = sourceFiles[i];
            //    var content = SourceText.From(new FileStream(file.Path, FileMode.Open), arguments.Encoding, arguments.ChecksumAlgorithm);
            //    if (content == null)
            //    {
            //        RunNormalCompilation(args);
            //        return;
            //    }
            //    //NOTE: order of trees is important!!
            //    trees[i] = SyntaxFactory.ParseSyntaxTree(content, file.IsScript ? scriptParseOptions : parseOptions, file.Path);
            //}

            //var assemblyIdentityComparer = DesktopAssemblyIdentityComparer.Default;
            //var appConfigPath = arguments.AppConfigPath;
            //if (appConfigPath != null)
            //{
            //    try
            //    {
            //        using (var appConfigStream = new FileStream(appConfigPath, FileMode.Open, FileAccess.Read))
            //        {
            //            assemblyIdentityComparer = DesktopAssemblyIdentityComparer.LoadFromXml(appConfigStream);
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        RunNormalCompilation(args);
            //        return;
            //    }
            //}

            //MetadataReferenceResolver referenceDirectiveResolver;
            //bool hadError = false;
            //var resolvedReferences = ResolveMetadataReferences(arguments, out referenceDirectiveResolver);
            //if (hadError)
            //{
            //    RunNormalCompilation(args);
            //    return;
            //}

            //var compilation = CSharpCompilation.Create(
            //     arguments.CompilationName,
            //     trees.Where(x => null != x),
            //     resolvedReferences,
            //     arguments.CompilationOptions.
            //         WithMetadataReferenceResolver(referenceDirectiveResolver).
            //         WithAssemblyIdentityComparer(assemblyIdentityComparer).
            //         WithStrongNameProvider(new DesktopStrongNameProvider(arguments.KeyFileSearchPaths)).
            //         WithXmlReferenceResolver(new XmlFileResolver(arguments.BaseDirectory)).
            //         WithSourceReferenceResolver(new SourceFileResolver(ImmutableArray<string>.Empty, arguments.BaseDirectory, arguments.PathMap)));

        }

        //static bool SuppressDefaultResponseFile(IEnumerable<string> args)
        //{
        //    return args.Any(arg => new[] { "/noconfig", "-noconfig" }.Contains(arg.ToLowerInvariant()));
        //}


        ///// <summary>
        ///// Resolves metadata references stored in command line arguments and reports errors for those that can't be resolved.
        ///// </summary>
        //static internal IEnumerable<MetadataReference> ResolveMetadataReferences(
        //    CSharpCommandLineArguments arguments,
        //    out MetadataReferenceResolver referenceDirectiveResolver)
        //{
        //    var pathResolver = new RelativePathResolver(arguments.ReferencePaths, arguments.BaseDirectory);
        //    var commandLineReferenceResolver = new CommandLineReferenceResolver(pathResolver);

        //    var resolved = arguments.ResolveMetadataReferences(commandLineReferenceResolver);
            
        //    // when compiling into an assembly (csc/vbc) we only allow #r that match references given on command line:
        //    referenceDirectiveResolver = new ExistingReferencesResolver(commandLineReferenceResolver, resolved.ToImmutableArray());

        //    return resolved;
        //}

        ///// <summary>
        ///// Looks for metadata references among the assembly file references given to the compilation when constructed.
        ///// When scripts are included into a project we don't want #r's to reference other assemblies than those 
        ///// specified explicitly in the project references.
        ///// </summary>
        //internal sealed class ExistingReferencesResolver : MetadataReferenceResolver, IEquatable<ExistingReferencesResolver>
        //{
        //    private readonly MetadataReferenceResolver _resolver;
        //    private readonly ImmutableArray<MetadataReference> _availableReferences;
        //    private readonly Lazy<HashSet<MetadataId>> _lazyAvailableReferences;

        //    public ExistingReferencesResolver(MetadataReferenceResolver resolver, ImmutableArray<MetadataReference> availableReferences)
        //    {
        //        Debug.Assert(resolver != null);
        //        Debug.Assert(availableReferences != null);

        //        _resolver = resolver;
        //        _availableReferences = availableReferences;

        //        // Delay reading assembly identities until they are actually needed (only when #r is encountered).
        //        _lazyAvailableReferences = new Lazy<HashSet<MetadataId>>(() => new HashSet<MetadataId>(
        //            from reference in _availableReferences
        //            let identity = TryGetIdentity(reference)
        //            where identity != null
        //            select identity));
        //    }

        //    public override ImmutableArray<PortableExecutableReference> ResolveReference(string reference, string baseFilePath, MetadataReferenceProperties properties)
        //    {
        //        var resolvedReferences = _resolver.ResolveReference(reference, baseFilePath, properties);
        //        return ImmutableArray.CreateRange(resolvedReferences.Where(r => _lazyAvailableReferences.Value.Contains(TryGetIdentity(r))));
        //    }

        //    private static MetadataId TryGetIdentity(MetadataReference metadataReference)
        //    {
        //        var peReference = metadataReference as PortableExecutableReference;
        //        if (peReference == null || peReference.Properties.Kind != MetadataImageKind.Assembly)
        //        {
        //            return null;
        //        }

        //        try
        //        {
        //            return ((AssemblyMetadata)peReference.GetMetadata()).Id;
        //        }
        //        catch (Exception e) when (e is BadImageFormatException || e is IOException)
        //        {
        //            // ignore, metadata reading errors are reported by the complier for the existing references
        //            return null;
        //        }
        //    }

        //    public override int GetHashCode()
        //    {
        //        return _resolver.GetHashCode();
        //    }

        //    public bool Equals(ExistingReferencesResolver other)
        //    {
        //        return _resolver.Equals(other._resolver) &&
        //               _availableReferences.SequenceEqual(other._availableReferences);
        //    }

        //    public override bool Equals(object other) => Equals(other as ExistingReferencesResolver);
        //}

        //static void RunNormalCompilation(string[] args)
        //{

        //}
    }
}