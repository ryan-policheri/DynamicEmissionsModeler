namespace DotNetCommon.DynamicCompilation
{
    public class CSharpMethod
    {
        public string MethodName { get; set; }

        public Type ReturnType { get; set; }

        public string ReturnTypeName
        {
            get
            {
                if (this.ReturnType == null) return "void";
                else return this.ReturnType.Name;
            }
        }

        public ICollection<CSharpMethodParameter> MethodParameters { get; set; }

        public string MethodBody { get; set; }
    }
}
