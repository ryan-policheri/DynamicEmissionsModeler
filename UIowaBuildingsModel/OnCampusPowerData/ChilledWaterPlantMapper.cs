using DotNetCommon.Extensions;
using System.Data;

namespace EmissionsMonitorModel
{
    public class ChilledWaterPlantMapper
    {
        public string Name { get; set; }

        public IEnumerable<TaggedDataSet> ChilledWaterPlantInputDataSets { get; set; }

        public IEnumerable<TaggedDataSet> ChilledWaterOutputDataSets { get; set; }

        public DataTable BuildDataTable(DateTimeOffset start, DateTimeOffset end)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Hour");

            foreach (var item in ChilledWaterPlantInputDataSets.Where(x => x.Units.CapsAndTrim() == "KW")) { table.Columns.Add(item.Tag + " (KW)"); }
            foreach (var item in ChilledWaterPlantInputDataSets.Where(x => x.Units.CapsAndTrim() == "LBS/HR")) { table.Columns.Add(item.Tag + " (LBS/HR)"); }
            foreach (var item in ChilledWaterOutputDataSets.Where(x => x.Units.CapsAndTrim() == "GPM")) { table.Columns.Add(item.Tag + " (GPM)"); }

            foreach (DateTimeOffset hour in start.EnumerateHoursUntil(end))
            {
                DataRow row = table.NewRow();
                row["Hour"] = hour.LocalDateTime.ToString();

                foreach (var item in ChilledWaterPlantInputDataSets.Where(x => x.Units.CapsAndTrim() == "KW"))
                {
                    var point = item.DataPoints.TryFindMatchingHour(hour);
                    row[item.Tag + " (KW)"] = point != null ? point.Value : "off";
                }

                foreach (var item in ChilledWaterPlantInputDataSets.Where(x => x.Units.CapsAndTrim() == "LBS/HR"))
                {
                    var point = item.DataPoints.TryFindMatchingHour(hour);
                    row[item.Tag + " (LBS/HR)"] = point != null ? point.Value : "off";
                }

                foreach (var item in ChilledWaterOutputDataSets.Where(x => x.Units.CapsAndTrim() == "GPM"))
                {
                    var point = item.DataPoints.TryFindMatchingHour(hour);
                    row[item.Tag + " (GPM)"] = point != null ? point.Value : "off";
                }

                table.Rows.Add(row);
            }

            return table;
        }
    }
}
