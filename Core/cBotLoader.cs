using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace OptimizationEngine.Core
{
    /// <summary>
    /// Loads cBot assemblies and discovers available cBot types.
    /// </summary>
    public static class cBotLoader
    {
        /// <summary>
        /// Load a .dll or assembly from specified path.
        /// </summary>
        public static Assembly LoadAssembly(string path)
        {
            if (File.Exists(path) && Path.GetExtension(path).Equals(".dll", StringComparison.OrdinalIgnoreCase))
                return Assembly.LoadFrom(path);

            if ((File.Exists(path) && Path.GetExtension(path).Equals(".csproj", StringComparison.OrdinalIgnoreCase)) || Directory.Exists(path))
            {
                var projectDir = File.Exists(path)
                    ? Path.GetDirectoryName(path)!
                    : path;
                var csFiles = Directory.GetFiles(projectDir, "*.cs", SearchOption.AllDirectories);
                return CompileSource(csFiles);
            }

            throw new FileNotFoundException($"Assembly or source not found: {path}");
        }

        private static Assembly CompileSource(string[] sourceFiles)
        {
            var syntaxTrees = sourceFiles.Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file), path: file)).ToList();
            var refs = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location));
            var compilation = CSharpCompilation.Create(
                "cBotTempAssembly",
                syntaxTrees,
                refs,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);
            if (!result.Success)
            {
                var errors = string.Join("\n", result.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .Select(d => d.ToString()));
                throw new InvalidOperationException($"Compilation failed:\n{errors}");
            }
            ms.Seek(0, SeekOrigin.Begin);
            return Assembly.Load(ms.ToArray());
        }

        /// <summary>
        /// Retrieve all types that appear to be cBots (with Parameter attribute usage).
        /// </summary>
        public static IEnumerable<Type> GetcBotTypes(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => t.IsClass && t.GetMembers()
                    .Any(m => m.GetCustomAttributes()
                        .Any(a => a.GetType().Name == "ParameterAttribute")));
        }
    }
}
