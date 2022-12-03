using DotNetCommon;
using DotNetCommon.DynamicCompilation;
using DotNetCommon.Extensions;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;
using Microsoft.CSharp.RuntimeBinder;
using System.Runtime.Intrinsics.X86;

namespace EmissionsMonitorDataAccess
{
    public class ModelInitializationService
    {
        private readonly DynamicCompilerService _compilerService;

        public ModelInitializationService(DynamicCompilerService compilerService)
        {
            _compilerService = compilerService;
        }

        public async Task InitializeModel(ProcessModel model)
        {
            //Compile code
            CSharpAssemblyBuilder builder = SetupAssemblyBuilder(model.ModelName.ToValidNamespace());

            foreach (ProcessNode node in model.ProcessNodes)
            {
                builder.AddClass(node.Name.ToValidClassName(), (builder) =>
                {
                    foreach (DataFunction function in node.GetUserDefinedFunctions())
                    {
                        builder.AddMethod(function.ToCSharpMethod());
                    }
                });
            }

            DynamicCompilationResult result = await _compilerService.CompileCSharpAsync(builder, _compilerService.GetAppAssemblies());
            if (!result.IsValid) throw new RuntimeBinderInternalCompilerException(result.CompilationErrors.ToDelimitedList(Environment.NewLine));

            foreach (ProcessNode node in model.ProcessNodes)
            {
                string hostClass = builder.Classes.First(x => x.ClassName == node.Name.ToValidClassName()).ClassName;
                object hostInstance = result.GetClassInstance(hostClass);
                foreach (DataFunction function in node.GetUserDefinedFunctions())
                {
                    function.FunctionHostObject = hostInstance;
                }
            }
        }

        public async Task InitializeFunction(DataFunction function)
        {
            CSharpAssemblyBuilder builder = SetupAssemblyBuilder("TEMP_ASSEMBLY_FOR_FUNC_TEST");

            builder.AddClass("TEMP_CLASS_FOR_FUNC_TEST", (builder) =>
            {
                builder.AddMethod(function.ToCSharpMethod());
            });

            DynamicCompilationResult result = await _compilerService.CompileCSharpAsync(builder, _compilerService.GetAppAssemblies());
            if (!result.IsValid) throw new RuntimeBinderInternalCompilerException(result.CompilationErrors.ToDelimitedList(Environment.NewLine));

            function.FunctionHostObject = result.GetClassInstance("TEMP_CLASS_FOR_FUNC_TEST");
        }

        private CSharpAssemblyBuilder SetupAssemblyBuilder(string assemblyNamespace)
        {
            CSharpAssemblyBuilder builder = new CSharpAssemblyBuilder();
            builder.AddUsing("UnitsNet");
            builder.AddUsing("EmissionsMonitorModel.Units");
            builder.AddUsing("EmissionsMonitorModel.TimeSeries");
            builder.AddUsing("EmissionsMonitorModel.ConversionMethods");
            builder.SetNamespace(assemblyNamespace);
            return builder;
        }
    }
}
