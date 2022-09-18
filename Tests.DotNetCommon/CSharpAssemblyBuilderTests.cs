using System;
using System.Collections.Generic;
using DotNetCommon.DynamicCompilation;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.DotNetCommon
{
    [TestClass]
    public class CSharpAssemblyBuilderTests
    {
        [TestMethod]
        public void WhenGivenUsingStatements_AddedAtTop()
        {
            CSharpAssemblyBuilder assemblyBuilder = new CSharpAssemblyBuilder();
            assemblyBuilder.AddUsing("System");
            assemblyBuilder.AddUsing("UnitsNet");
            assemblyBuilder.AddUsing("System.DateTime");

            string expected = $"using System;{Environment.NewLine}using UnitsNet;{Environment.NewLine}using System.DateTime;{Environment.NewLine}";
            string actual = assemblyBuilder.Build();

            actual.Should().Be(expected);
        }

        [TestMethod]
        public void WhenSettingNamespace_ShouldCreateNamespaceContainer()
        {
            CSharpAssemblyBuilder assemblyBuilder = new CSharpAssemblyBuilder();
            assemblyBuilder.SetNamespace("DynamicTesting");

            string code = assemblyBuilder.Build();

            code.Should().Be("namespace DynamicTesting" + Environment.NewLine + "{" + Environment.NewLine + "}");
        }

        [TestMethod]
        public void WhenAddingClass_ShouldCreateClassContainer()
        {
            CSharpAssemblyBuilder assemblyBuilder = new CSharpAssemblyBuilder();
            assemblyBuilder.SetNamespace("DynamicTesting");
            assemblyBuilder.AddClass("DynamicClass", (CSharpClassBuilder builder) => { /*Do nothing*/ });

            string actual = assemblyBuilder.Build();
            string expected = "namespace DynamicTesting" + Environment.NewLine + "{" + Environment.NewLine;
            expected += "\tpublic class DynamicClass" + Environment.NewLine + "\t{" + Environment.NewLine;
            expected += "\t}" + Environment.NewLine;
            expected += "}";

            actual.Should().Be(expected);
        }

        [TestMethod]
        public void WhenAddingMethodToClass_IsCorrect()
        {
            CSharpAssemblyBuilder assemblyBuilder = new CSharpAssemblyBuilder();
            assemblyBuilder.SetNamespace("DynamicTesting");
            assemblyBuilder.AddClass("DynamicClass", (CSharpClassBuilder builder) =>
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

            string expected = "namespace DynamicTesting" + Environment.NewLine + "{" + Environment.NewLine;
            expected += "\tpublic class DynamicClass" + Environment.NewLine + "\t{" + Environment.NewLine;
            expected += "\t\tpublic Int32 Foobar(Int32 value1, Double value2)" + Environment.NewLine;
            expected += "\t\t{" + Environment.NewLine;
            expected += "\t\t\t" + "double value3 = value1 * value2;" + Environment.NewLine;
            expected += "\t\t\t" + "return (int)value3;" + Environment.NewLine;
            expected += "\t\t}" + Environment.NewLine;
            expected += "\t}" + Environment.NewLine;
            expected += "}";
            string actual = assemblyBuilder.Build();

            actual.Should().Be(expected);
        }

        [TestMethod]
        public void WhenAddingMultipleMethodToClass_IsCorrect()
        {
            CSharpAssemblyBuilder assemblyBuilder = new CSharpAssemblyBuilder();
            assemblyBuilder.SetNamespace("DynamicTesting");
            assemblyBuilder.AddClass("DynamicClass", (CSharpClassBuilder builder) =>
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

            string expected = "namespace DynamicTesting" + Environment.NewLine + "{" + Environment.NewLine;
            expected += "\tpublic class DynamicClass" + Environment.NewLine + "\t{" + Environment.NewLine;
            expected += "\t\tpublic Int32 Foobar(Int32 value1, Double value2)" + Environment.NewLine;
            expected += "\t\t{" + Environment.NewLine;
            expected += "\t\t\t" + "double value3 = value1 * value2;" + Environment.NewLine;
            expected += "\t\t\t" + "return (int)value3;" + Environment.NewLine;
            expected += "\t\t}" + Environment.NewLine;
            expected += Environment.NewLine;
            expected += "\t\tpublic Int32 Barfoo(DateTime date1, Int32 int1)" + Environment.NewLine;
            expected += "\t\t{" + Environment.NewLine;
            expected += "\t\t\t" + "double value3 = date1.Ticks * int1;" + Environment.NewLine;
            expected += "\t\t\t" + "return (int)value3;" + Environment.NewLine;
            expected += "\t\t}" + Environment.NewLine;
            expected += "\t}" + Environment.NewLine;
            expected += "}";
            string actual = assemblyBuilder.Build();

            actual.Should().Be(expected);
        }

        [TestMethod]
        public void WhenAddingMultipleClasses_IsCorrect()
        {
            CSharpAssemblyBuilder assemblyBuilder = new CSharpAssemblyBuilder();
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

            string expected = "namespace DynamicTesting" + Environment.NewLine + "{" + Environment.NewLine;
            expected += "\tpublic class FirstClass" + Environment.NewLine + "\t{" + Environment.NewLine;
            expected += "\t\tpublic Int32 Foobar(Int32 value1, Double value2)" + Environment.NewLine;
            expected += "\t\t{" + Environment.NewLine;
            expected += "\t\t\t" + "double value3 = value1 * value2;" + Environment.NewLine;
            expected += "\t\t\t" + "return (int)value3;" + Environment.NewLine;
            expected += "\t\t}" + Environment.NewLine;
            expected += Environment.NewLine;
            expected += "\t\tpublic Int32 Barfoo(DateTime date1, Int32 int1)" + Environment.NewLine;
            expected += "\t\t{" + Environment.NewLine;
            expected += "\t\t\t" + "double value3 = date1.Ticks * int1;" + Environment.NewLine;
            expected += "\t\t\t" + "return (int)value3;" + Environment.NewLine;
            expected += "\t\t}" + Environment.NewLine;
            expected += "\t}" + Environment.NewLine;
            expected += Environment.NewLine;
            expected += "\tpublic class SecondClass" + Environment.NewLine + "\t{" + Environment.NewLine;
            expected += "\t\tpublic Int32 Foobar(Int32 value1, Double value2)" + Environment.NewLine;
            expected += "\t\t{" + Environment.NewLine;
            expected += "\t\t\t" + "double value3 = value1 * value2;" + Environment.NewLine;
            expected += "\t\t\t" + "return (int)value3;" + Environment.NewLine;
            expected += "\t\t}" + Environment.NewLine;
            expected += "\t}" + Environment.NewLine;
            expected += "}";
            string actual = assemblyBuilder.Build();

            actual.Should().Be(expected);
        }
    }
}
