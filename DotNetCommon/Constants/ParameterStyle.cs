using System.ComponentModel;

namespace DotNetCommon.Constants
{
    public enum ParameterStyle
    {
        [Description("--")]
        DoubleDash,
        [Description("-")]
        SingleDash
    }
}
