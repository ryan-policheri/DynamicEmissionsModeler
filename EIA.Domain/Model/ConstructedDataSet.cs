using EIA.Domain.Enums;
using DotNetCommon.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace EIA.Domain.Model
{
    public class ConstructedDataSet
    {
        private readonly Series _series;

        public ConstructedDataSet(Series series)
        {
            if (series == null) throw new ArgumentNullException(nameof(series));
            _series = series;
            Table = BuildDataTable();
        }

        public string Name => _series.Name;

        public string Description => _series.Description;

        public Frequency Frequency
        {
            get
            {
                switch (_series.Frequency.CapsAndTrim())
                {
                    case "M":
                        return Frequency.Monthly;
                    case "Q":
                        return Frequency.Quarterly;
                    case "A":
                        return Frequency.Yearly;
                    default:
                        throw new NotImplementedException("Frequency text to enum conversion not implemented");
                }
            }
        }

        public string Units => _series.Units;

        public DataTable Table { get; }

        private DataTable BuildDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("PERIOD", typeof(DateTime));
            table.Columns.Add("VALUE", typeof(double));
            CultureInfo us = new CultureInfo("en-US");

            foreach (SeriesData dataPoint in _series.Data)
            {
                DataRow row = table.NewRow();
                switch(_series.Frequency)
                {
                    case "M":
                        row["PERIOD"] = DateTime.ParseExact(dataPoint.ColumnHeader, "yyyyMM", us);
                        break;
                    case "A":
                        row["PERIOD"] = DateTime.ParseExact(dataPoint.ColumnHeader, "yyyy", us);
                        break;
                    case "Q":
                        row["PERIOD"] = dataPoint.ColumnHeader.ParseQuarter();
                        break;
                    case "HL":
                        row["PERIOD"] = DateTime.ParseExact(dataPoint.ColumnHeader, "yyyyMMddTHH-mm", us);
                        break;
                    default:
                        throw new NotImplementedException($"Fequency {_series.Frequency} is not implemented");
                }    
                row["VALUE"] = dataPoint.ColumnValue;
                table.Rows.Add(row);
            }

            return table;
        }
    }
}
