using DotNetCommon.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmissionsMonitorModel.TimeSeries
{
    public class MonitorSeries
    {
        public string SeriesName { get; set; }

        public ICollection<MonitorDataPoint> DataPoints { get; set; }

        public string ProductName => this.DataPoints.FirstOrDefault()?.Values.Product.FunctionName;

        public string CostNames => this.DataPoints.FirstOrDefault()?.Values.Costs?.Select(x => x.FunctionName).ToDelimitedList(',');

        public DataTable TransformToDataTable()
        {
            DataTable table = new DataTable();

            string timestampColumn = "Timestamp";
            var product = this.DataPoints.First().Values.Product;
            string productColumn = product.ToString();
            var costColumns = this.DataPoints.First().Values.Costs.Select(x => x.ToString()).ToList();

            table.Columns.Add(timestampColumn);
            table.Columns.Add(productColumn);
            table.Columns.AddRange(costColumns.Select(x => new DataColumn(x)).ToArray());

            foreach (var point in DataPoints.OrderBy(x => x.Timestamp))
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
