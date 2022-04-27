using DotNetCommon.Extensions;
using System.Data;
using UIowaBuildingsModel;

namespace UIowaBuildingsServices
{
    internal class ChilledWaterDataRender
    {
        private readonly ChilledWaterProductionMapper _mapper;
        private readonly DateTimeOffset _start;
        private readonly DateTimeOffset _end;

        public ChilledWaterDataRender(ChilledWaterProductionMapper mapper, DateTimeOffset start, DateTimeOffset end)
        {
            _mapper = mapper;
            _start = start;
            _end = end;
        }

        public TableWithMeta BuildHourlySummary(string waterPlantName)
        {
            ChilledWaterPlantMapper plantMapper = _mapper.PlantMappers.First(x => x.Name.CapsAndTrim() == waterPlantName.CapsAndTrim());

            DataTable table = new DataTable();
            table.Columns.Add("Hour");

            foreach (var item in plantMapper.ChilledWaterPlantInputDataSets.Where(x => x.Units.CapsAndTrim() == "KW")) { table.Columns.Add(item.Tag + " (KW)"); }
            foreach (var item in plantMapper.ChilledWaterPlantInputDataSets.Where(x => x.Units.CapsAndTrim() == "LBS/HR")) { table.Columns.Add(item.Tag + " (LBS/HR)"); }
            foreach (var item in plantMapper.ChilledWaterOutputDataSets.Where(x => x.Units.CapsAndTrim() == "GPM")) { table.Columns.Add(item.Tag + " (GPM)"); }

            foreach (DateTimeOffset hour in _start.EnumerateHoursUntil(_end))
            {
                DataRow row = table.NewRow();
                row["Hour"] = hour.LocalDateTime.ToString();

                foreach (var item in plantMapper.ChilledWaterPlantInputDataSets.Where(x => x.Units.CapsAndTrim() == "KW"))
                {
                    var point = item.DataPoints.TryFindMatchingHour(hour);
                    row[item.Tag + " (KW)"] = point != null ? point.Value : "off";
                }

                foreach (var item in plantMapper.ChilledWaterPlantInputDataSets.Where(x => x.Units.CapsAndTrim() == "LBS/HR"))
                {
                    var point = item.DataPoints.TryFindMatchingHour(hour);
                    row[item.Tag + " (LBS/HR)"] = point != null ? point.Value : "off";
                }

                foreach (var item in plantMapper.ChilledWaterOutputDataSets.Where(x => x.Units.CapsAndTrim() == "GPM"))
                {
                    var point = item.DataPoints.TryFindMatchingHour(hour);
                    row[item.Tag + " (GPM)"] = point != null ? point.Value : "off";
                }

                table.Rows.Add(row);
            }

            TableWithMeta tableWithMeta = new TableWithMeta();
            tableWithMeta.Table = table;
            tableWithMeta.Header = "Chilled Water Production - " + _start.LocalDateTime.Date.ToShortDateString();
            if (_start.EnumerateHoursUntil(_end).Count() > 24) tableWithMeta.Header += " - " + _end.LocalDateTime.Date.ToShortTimeString();

            return tableWithMeta;
        }
    }
}