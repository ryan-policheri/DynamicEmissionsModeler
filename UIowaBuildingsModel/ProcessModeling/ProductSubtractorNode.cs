﻿using EmissionsMonitorModel.TimeSeries;
using System.Text.Json.Serialization;

namespace EmissionsMonitorModel.ProcessModeling
{
    public class ProductSubtractorNode : ProcessNode, ISinglePredecessor
    {
        public ProductSubtractorNode()
        {
            PrecedingNodeId = -1;
            PrecedingNode = null;
        }

        public int PrecedingNodeId { get; set; }

        [JsonIgnore]
        public ProcessNode PrecedingNode { get; set; }

        public DataFunction ProductDeductionFunction { get; set; }

        public override ICollection<DataFunction> GetUserDefinedFunctions()
        {
            return new List<DataFunction>() { ProductDeductionFunction };
        }

        public override ProductCostResults RenderProductAndCosts(ICollection<DataPoint> dataPoints)
        {
            ProductCostResults preceedingStream = PrecedingNode.RenderProductAndCosts(dataPoints);
            if (preceedingStream.Product.UnitType != ProductDeductionFunction.FunctionUnit
                || preceedingStream.Product.UnitForm != ProductDeductionFunction.FunctionUnitForm
                || preceedingStream.Product.DefaultValueUnit != ProductDeductionFunction.FunctionDefaultReturnUnit)
            {
                throw new InvalidOperationException("The preceeding node and the product deduction function must have the same type of product");
            }

            var input = dataPoints.Where(x => ProductDeductionFunction.FunctionFactors.Any(y => y.FactorUri.Uri == x.Series.SeriesUri.Uri));
            DataFunctionResult productDeduction = ProductDeductionFunction.ExecuteFunction(input);
            if (preceedingStream.Product.TotalValue < productDeduction.TotalValue) { throw new InvalidOperationException("Deduction value cannot be more than total value"); }

            preceedingStream.Product.TotalValue -= productDeduction.TotalValue;
            return preceedingStream;
        }
    }
}
