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

            //Add Split Nodes
            foreach (var ssn in model.ProcessNodes.Where(x => x.GetType() == typeof(StreamSplitterNode)).Select(x => x as StreamSplitterNode).ToList())
            {
                model.ProcessNodes.Add(ssn.SplitResultNode);
                model.ProcessNodes.Add(ssn.RemainderResultNode);
            }

            //Populate Preceding Nodes
            foreach (var node in model.ProcessNodes)
            {
                if (node is LikeTermsAggregatorNode)
                {
                    LikeTermsAggregatorNode ltn = node as LikeTermsAggregatorNode;
                    foreach (int id in ltn.PrecedingNodeIds)
                    {
                        ltn.PrecedingNodes.Add(model.ProcessNodes.First(x => x.Id == id) as ExchangeNode);
                    }
                }
                if (node is StreamSplitterNode)
                {
                    StreamSplitterNode ssn = node as StreamSplitterNode;
                    ssn.PrecedingNode = model.ProcessNodes.First(x => x.Id == ssn.PrecedingNodeId);
                }
                if (node is ProductConversionNode)
                {
                    ProductConversionNode pcn = node as ProductConversionNode;
                    pcn.PrecedingNode = model.ProcessNodes.First(x => x.Id == pcn.PrecedingNodeId);
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
