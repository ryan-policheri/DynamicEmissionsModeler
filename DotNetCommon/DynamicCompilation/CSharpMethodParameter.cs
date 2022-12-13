namespace DotNetCommon.DynamicCompilation
{
    public class CSharpMethodParameter
    {
        public Type ParameterType { get; set; }

        public string ParameterTypeName => ParameterType.Name;

        public string ParameterName { get; set; }
    }

    public class CSharpMethodArgument : CSharpMethodParameter
    {
        public CSharpMethodArgument(CSharpMethodParameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            this.ParameterType = parameter.ParameterType;
            this.ParameterName = parameter.ParameterName;
        }

        private object _argument;
        public object Argument
        {
            get { return _argument; }
            set
            {
                if (value.GetType() != this.ParameterType) throw new ArgumentException($"Argument needs to be of type {ParameterTypeName} not {value.GetType().Name}");
                else _argument = value;
            }
        }
    }
}
