using System.Collections.Generic;

namespace EIA.Domain.Model
{
    public class SeriesData : List<object>, ICollection<object>
    {

        public string ColumnHeader => this[0].ToString();

        public double ColumnValue => double.Parse(this[1].ToString());


    }
}