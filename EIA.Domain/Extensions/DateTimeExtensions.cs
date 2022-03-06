using System;
using DotNetCommon.Extensions;

namespace EIA.Domain.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ParseQuarter(this string source)
        {
            string year = source.Substring(0, 4);
            string quarter = source.Substring(4);

            int yearInt = int.Parse(year);

            int quarterAsMonth;
            switch (quarter.CapsAndTrim())
            {
                case "Q1": quarterAsMonth = 1; break;
                case "Q2": quarterAsMonth = 4; break;
                case "Q3": quarterAsMonth = 7; break;
                case "Q4": quarterAsMonth = 10; break;
                default: throw new ArgumentOutOfRangeException("Only four quarters in a year");
            }

            return new DateTime(yearInt, quarterAsMonth, 1);
        }
    }
}
