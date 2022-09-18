using System.Reflection;
using DotNetCommon.DynamicCompilation;
using DotNetCommon.Extensions;

namespace DotNetCommon
{
    public class DynamicCompilationResult
    {
        public CSharpAssemblyBuilder SourceCodeBuilder { get; set; }

        public string SourceCode => SourceCodeBuilder.Build();

        public bool IsValid => LoadedAssembly != null && CompilationErrors.Count() == 0;

        public ICollection<string> CompilationErrors { get; set; } = new List<string>();

        public string CompilationErrorString => CompilationErrors.ToDelimitedList(Environment.NewLine);

        public Assembly LoadedAssembly { get; set; }

        public object GetClassInstance(string className)
        {
            if (!this.SourceCodeBuilder.Classes.Any(x => x.ClassName == className)) throw new ArgumentOutOfRangeException($"Class with name {className} not found");
            Type type = this.LoadedAssembly.GetType($"{this.SourceCodeBuilder.Namespace}.{className}");
            object instance = Activator.CreateInstance(type);
            return instance;
        }

        public T GetClassInstance<T>(string className)
        {
            return (T)this.GetClassInstance(className);
        }
        
        public IList<CSharpMethodArgument> GetCSharpMethodArguments(string className, string methodName)
        {
            return this.SourceCodeBuilder.Classes.First(x => x.ClassName == className)
                .Methods.First(x => x.MethodName == methodName)
                .MethodParameters.Select(x => new CSharpMethodArgument(x))
                .ToList();
        }

        public void ExecuteMethod(string className, string methodName, IEnumerable<CSharpMethodArgument> args)
        {
            InternalExecuteMethod(className, methodName, args);
        }

        public T ExecuteMethod<T>(string className, string methodName, IEnumerable<CSharpMethodArgument> args)
        {
            return (T)InternalExecuteMethod(className, methodName, args);
        }

        private object InternalExecuteMethod(string className, string methodName, IEnumerable<CSharpMethodArgument> args)
        {
            object instance = GetClassInstance(className);
            Type instanceType = instance.GetType();
            MethodInfo method = instanceType.GetMethod(methodName);
            if (method == null) throw new ArgumentOutOfRangeException($"Method with name {methodName} not found");

            ParameterInfo[] parameters = method.GetParameters();
            ICollection<object> orderedArgs = new List<object>();
            foreach (ParameterInfo param in parameters.OrderBy(x => x.Position))
            {
                CSharpMethodArgument correspondingArg = args.First(x => x.ParameterName == param.Name);
                orderedArgs.Add(correspondingArg.Argument);
            }

            object returnValue = instanceType.InvokeMember(methodName, BindingFlags.Default | BindingFlags.InvokeMethod, null, instance, orderedArgs.ToArray());
            return returnValue;
        }
    }
}
