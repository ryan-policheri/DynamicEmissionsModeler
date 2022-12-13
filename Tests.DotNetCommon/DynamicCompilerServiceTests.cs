using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DotNetCommon;
using DotNetCommon.DynamicCompilation;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.DotNetCommon
{
    [TestClass]
    public class DynamicCompilerServiceTests
    {
        [TestMethod]
        public void WhenCompilingValidCode_NoErrorsOccur()
        {
            CSharpAssemblyBuilder assemblyBuilder = GetValidCodeExample();
            DynamicCompilerService compilerService = new DynamicCompilerService();
            DynamicCompilationResult result = compilerService.CompileCSharp(assemblyBuilder, compilerService.GetAppAssemblies());

            result.CompilationErrors.Count.Should().Be(0);
            result.IsValid.Should().Be(true);
        }

        [TestMethod]
        public void WhenCompilingValidCode_CanExecuteInstanceMethod()
        {
            CSharpAssemblyBuilder assemblyBuilder = GetValidCodeExample();
            DynamicCompilerService compilerService = new DynamicCompilerService();
            DynamicCompilationResult compileResult = compilerService.CompileCSharp(assemblyBuilder, compilerService.GetAppAssemblies());
            
            IList<CSharpMethodArgument> methodArgs = compileResult.GetCSharpMethodArguments("FirstClass", "Foobar");
            methodArgs.First(x => x.ParameterName == "value1").Argument = 4;
            methodArgs.First(x => x.ParameterName == "value2").Argument = 7.5;
            int methodResult = compileResult.ExecuteMethod<int>("FirstClass", "Foobar", methodArgs);
            methodResult.Should().Be(30);
        }

        private CSharpAssemblyBuilder GetValidCodeExample()
        {
            CSharpAssemblyBuilder assemblyBuilder = new CSharpAssemblyBuilder();
            assemblyBuilder.AddUsing("System");
            assemblyBuilder.SetNamespace("DynamicTesting");
            assemblyBuilder.AddClass("FirstClass", (CSharpClassBuilder builder) =>
            {
                builder.AddMethod(new CSharpMethod
                {
                    MethodName = "Foobar",
                    ReturnType = typeof(int),
                    MethodParameters = new List<CSharpMethodParameter>()
                    {
                        new CSharpMethodParameter { ParameterName = "value1", ParameterType = typeof(int) },
                        new CSharpMethodParameter { ParameterName = "value2", ParameterType = typeof(double) },
                    },
                    MethodBody = $"double value3 = value1 * value2;{Environment.NewLine}return (int)value3;"
                });
                builder.AddMethod(new CSharpMethod
                {
                    MethodName = "Barfoo",
                    ReturnType = typeof(int),
                    MethodParameters = new List<CSharpMethodParameter>()
                    {
                        new CSharpMethodParameter { ParameterName = "date1", ParameterType = typeof(DateTime) },
                        new CSharpMethodParameter { ParameterName = "int1", ParameterType = typeof(int) }
                    },
                    MethodBody = $"double value3 = date1.Ticks * int1;{Environment.NewLine}return (int)value3;"
                });
            });

            assemblyBuilder.AddClass("SecondClass", (CSharpClassBuilder builder) =>
            {
                builder.AddMethod(new CSharpMethod
                {
                    MethodName = "Foobar",
                    ReturnType = typeof(int),
                    MethodParameters = new List<CSharpMethodParameter>()
                    {
                        new CSharpMethodParameter { ParameterName = "value1", ParameterType = typeof(int) },
                        new CSharpMethodParameter { ParameterName = "value2", ParameterType = typeof(double) },
                    },
                    MethodBody = $"double value3 = value1 * value2;{Environment.NewLine}return (int)value3;"
                });
            });

            return assemblyBuilder;
        }
    }
}
