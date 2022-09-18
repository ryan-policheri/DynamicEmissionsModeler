using System.Text;

namespace DotNetCommon.DynamicCompilation
{
    public class CSharpAssemblyBuilder
    {
        private ICollection<string> _usingStatements = new List<string>();

        public string Namespace { get; private set; }
        public ICollection<CSharpClassBuilder> Classes = new List<CSharpClassBuilder>(); 

        public CSharpAssemblyBuilder AddUsing(string usingStatement)
        {
            if (string.IsNullOrWhiteSpace(usingStatement)) throw new ArgumentNullException(nameof(usingStatement));
            if (usingStatement.ToUpper().Contains("USING ")) throw new ArgumentException("Do not include the word using in your statement. It will be added automatically.");
            _usingStatements.Add(usingStatement);
            return this;
        }

        public CSharpAssemblyBuilder SetNamespace(string namespaceName)
        {
            if (String.IsNullOrWhiteSpace(namespaceName)) throw new ArgumentNullException(nameof(namespaceName));
            if (namespaceName.ToUpper().Contains("NAMESPACE ")) throw new ArgumentException("Do not include the word namespace in your statement. It will be added automatically.");
            if (!String.IsNullOrWhiteSpace(Namespace)) throw new InvalidOperationException($"A namespace {Namespace} has already been created. Using only 1 namespace is supported");
            Namespace = namespaceName;
            return this;
        }

        public void AddClass(string className, Action<CSharpClassBuilder> action)
        {
            CSharpClassBuilder classBuilder = new CSharpClassBuilder(className);
            action(classBuilder);
            Classes.Add(classBuilder);
        }

        public string Build()
        {
            StringBuilder builder = new StringBuilder();
            foreach (string statement in _usingStatements) builder.AppendLine($"using {statement};");

            if (!String.IsNullOrWhiteSpace(Namespace))
            {
                if (_usingStatements.Count() > 0) builder.AppendLine();
                builder.AppendLine($"namespace {Namespace}");
                builder.AppendLine("{");
                foreach (CSharpClassBuilder clas in this.Classes)
                {
                    builder.AppendLine($"\tpublic class {clas.ClassName}");
                    builder.AppendLine("\t{");
                    foreach (CSharpMethod method in clas.Methods)
                    {
                        builder.Append($"\t\tpublic {method.ReturnTypeName} {method.MethodName}(");
                        if (method.MethodParameters != null)
                        {
                            foreach (CSharpMethodParameter param in method.MethodParameters)
                            {
                                builder.Append($"{param.ParameterTypeName} {param.ParameterName}, ");
                            }
                            builder.TrimEnd(' '); builder.TrimEnd(',');
                        }
                        builder.AppendLine(")");

                        builder.AppendLine("\t\t{");
                        string[] bodyLines = method.MethodBody.Split(Environment.NewLine);
                        foreach (string line in bodyLines)
                        {
                            builder.AppendLine($"\t\t\t{line}");
                        }
                        builder.AppendLine("\t\t}");
                        builder.AppendLine();
                    }
                    if(clas.Methods.Count > 0) builder.TrimNewline();
                    builder.AppendLine("\t}");
                    builder.AppendLine();
                }
                if (this.Classes.Count > 0) builder.TrimNewline();
                builder.Append("}");
            }

            return builder.ToString();
        }
    }
}
