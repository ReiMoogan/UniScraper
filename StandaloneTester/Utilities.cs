using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace StandaloneTester;

public static class Utilities
{
    public static void MapSql(DataTable table, SqlBulkCopy copier)
    {
        copier.ColumnMappings.Clear();
        foreach (var col in table.Columns)
        {
            var name = ((DataColumn) col).ColumnName;
            var snake = string.Concat(name.Select((x, i) =>
                i > 0 && char.IsUpper(x) && !char.IsUpper(name[i - 1]) ? $"_{x}" : x.ToString())).ToLower();
            copier.ColumnMappings.Add(name, snake);
        }
    }
}