using DotNetCommon.Extensions;
using System.Data;

namespace EmissionsMonitorModel.TimeSeries
{
    public class NodeSeries
    {
        public int NodeId { get; set; }

        public string NodeName { get; set; }

        public ICollection<NodeOutputPoint> NodeOutputPoints { get; set; }

        public string ProductName => this.NodeOutputPoints.FirstOrDefault()?.Values.Product.FunctionName;

        public string CostNames => this.NodeOutputPoints.FirstOrDefault()?.Values.Costs?.Select(x => x.FunctionName).ToDelimitedList(',');

        public DataTable TransformToDataTable()
        {
            DataTable table = new DataTable();

            string timestampColumn = "Timestamp";
            var product = this.NodeOutputPoints.First().Values.Product;
            string productColumn = product.ToString();
            var costColumns = this.NodeOutputPoints.First().Values.Costs.Select(x => x.ToString()).ToList();

            table.Columns.Add(timestampColumn);
            table.Columns.Add(productColumn);
            table.Columns.AddRange(costColumns.Select(x => new DataColumn(x)).ToArray());

            foreach (var point in NodeOutputPoints.OrderBy(x => x.Timestamp))
            {
                DataRow row = table.NewRow();
                row[timestampColumn] = point.Timestamp.LocalDateTime.ToString();
                row[productColumn] = point.Values.Product.TotalValue;
                foreach(var cost in point.Values.Costs) { row[cost.ToString()] = cost.TotalValue; }
                table.Rows.Add(row);
            }

            return table;
        }
    }
}
