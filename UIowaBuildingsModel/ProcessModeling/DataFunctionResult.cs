using System.Text.Json.Serialization;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class DataFunctionResult
    {
        public string FunctionName { get; set; }

        public double TotalValue { get; set; }

        public string Unit { get; set; }

        public string UnitForm { get; set; }

        public string DefaultValueUnit { get; set; }

        [JsonIgnore]
        public Func<object, double> ValueRenderFunction { get; set; }

        public bool IsLikeResult(DataFunctionResult other)
        {
            return this.Unit == other.Unit && this.UnitForm == other.UnitForm;
        }

        public static DataFunctionResult CombineResults(DataFunctionResult r1, DataFunctionResult r2)
        {
            if (!r1.IsLikeResult(r2)) throw new ArgumentException("Arguments are not of like units");
            DataFunctionResult r3 = new DataFunctionResult();
            r3.FunctionName = r1.FunctionName;
            r3.Unit = r1.Unit;
            r3.UnitForm = r1.UnitForm;
            r3.DefaultValueUnit = r1.DefaultValueUnit;
            r3.ValueRenderFunction = r1.ValueRenderFunction;
            r3.TotalValue = r1.TotalValue + r2.TotalValue;
            return r3;
        }

        public override string ToString()
        {
            return $"{this.UnitForm} {this.Unit} ({this.DefaultValueUnit})";
        }
    }
}
