using System.Data;

namespace UIowaBuildingsServices
{
    public class TableWithMeta
    {
        public int InputSize { get; set; }

        public string Header { get; set; }

        public DataTable Table { get; set; }

        public int ColumnCount => Table.Columns.Count;
    }
}
