using DotNetCommon;
using DotNetCommon.DynamicCompilation;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;
using Microsoft.CSharp.RuntimeBinder;

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
            CSharpAssemblyBuilder builder = new CSharpAssemblyBuilder();
            builder.AddUsing("UnitsNet");
            builder.AddUsing("EmissionsMonitorModel.Units");
            builder.AddUsing("EmissionsMonitorModel.TimeSeries");
            builder.AddUsing("EmissionsMonitorModel.ConversionMethods");
            builder.SetNamespace(model.ModelName);

            foreach (ProcessNode node in model.ProcessNodes)
            {
                builder.AddClass(node.Name.ToValidClassName(), (builder) =>
                {
                    foreach (DataFunction function in node.GetUserDefinedFunctions())
                    {
                        builder.AddMethod(new CSharpMethod
                        {
                            MethodName = function.FunctionName.ToValidMethodName(),
                            ReturnType = function.GetReturnType(),
                            MethodBody = function.FunctionCode,
                            MethodParameters = function.FunctionFactors.Select(x => new CSharpMethodParameter
                            {
                                ParameterName = x.ParameterName.ToValidVariableName(),
                                ParameterType = typeof(DataPoint)
                            }).ToList()
                        });
                    }
                });
            }

            DynamicCompilationResult result = await _compilerService.CompileCSharpAsync(builder, _compilerService.GetAppAssemblies());
            if (!result.IsValid) throw new RuntimeBinderInternalCompilerException(result.CompilationErrors.ToString());

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
    }
}
