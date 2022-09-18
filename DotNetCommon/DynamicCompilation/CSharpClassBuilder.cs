namespace DotNetCommon.DynamicCompilation
{
    public class CSharpClassBuilder
    {
        public CSharpClassBuilder(string className)
        {
            ClassName = className;
            Methods = new List<CSharpMethod>();
        }

        public string ClassName { get; }

        public ICollection<CSharpMethod> Methods { get; }

        public void AddMethod(CSharpMethod cSharpMethod)
        {
            Methods.Add(cSharpMethod);
        }
    }
}