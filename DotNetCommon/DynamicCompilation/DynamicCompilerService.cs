using System.Reflection;
using DotNetCommon.DynamicCompilation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace DotNetCommon
{
    public class DynamicCompilerService
    {
        public IEnumerable<Assembly> GetAppAssemblies(IEnumerable<string> desiredSubset = null)
        {
            IEnumerable<Assembly> assemblyReferences = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && !String.IsNullOrWhiteSpace(a.Location));
            if (desiredSubset != null) assemblyReferences = assemblyReferences.Where(x => desiredSubset.Contains(x.GetName().Name));
            return assemblyReferences;
        }

        public DynamicCompilationResult CompileCSharp(CSharpAssemblyBuilder codeBuilder, IEnumerable<Assembly> referencedAssemblies)
        {
            DynamicCompilationResult compilationResult = new DynamicCompilationResult();
            compilationResult.SourceCodeBuilder = codeBuilder;
            string sourceCode = codeBuilder.Build();

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            IEnumerable<MetadataReference> references = ConvertAssembliesToMetadataReferences(referencedAssemblies);

            CSharpCompilation compilation = CSharpCompilation.Create(Path.GetRandomFileName(), syntaxTrees: new[] { syntaxTree }, references: references, options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        compilationResult.CompilationErrors.Add($"{diagnostic.Id}: {diagnostic.GetMessage()}");
                    }
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    Assembly assembly = Assembly.Load(ms.ToArray());
                    compilationResult.LoadedAssembly = assembly;
                }
            }

            return compilationResult;
        }

        public async Task<DynamicCompilationResult> CompileCSharpAsync(CSharpAssemblyBuilder codeBuilder, IEnumerable<Assembly> referencedAssemblies)
        {
            return await Task.Run(() => CompileCSharp(codeBuilder, referencedAssemblies));
        }

        private IEnumerable<MetadataReference> ConvertAssembliesToMetadataReferences(IEnumerable<Assembly> assemblies)
        {
            ICollection<MetadataReference> references = new List<MetadataReference>();
            IEnumerable<string> assemblyPaths = assemblies.Select(a => a.Location);
            foreach (string path in assemblyPaths) references.Add(MetadataReference.CreateFromFile(path));
            return references;
        }
    }
}
