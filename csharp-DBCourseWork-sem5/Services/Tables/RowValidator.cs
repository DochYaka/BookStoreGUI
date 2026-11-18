using csharp_DBCourseWork_sem5.Models;
using csharp_DBCourseWork_sem5.Models.Interfaces;
using csharp_DBCourseWork_sem5.Services.Tables;
using System.Data;

namespace csharp_DBCourseWork_sem5.Services
{
    public class RowValidator : IRowValidator
    {
        public IReadOnlyList<string> Validate(DataTable table, IEnumerable<DataRow> rows, ISet<string> notNullColumns)
        {
            var errors = new List<string>();

            foreach (var row in rows)
            {
                int rowNum = table.Rows.IndexOf(row) + 1;

                foreach (var col in notNullColumns)
                {
                    if (col.Equals("id", StringComparison.OrdinalIgnoreCase) && row.RowState == DataRowState.Added)
                        continue;

                    if (table.Columns.Contains(col) && SqlHelper.IsEmpty(row[col]))
                        errors.Add($"Строка {rowNum}: поле «{ColumnTranslator.GetDisplayName(col)}» не может быть пустым");
                }

                foreach (DataColumn col in table.Columns)
                {
                    var value = row[col];
                    if (SqlHelper.IsEmpty(value)) continue;

                    string colName = ColumnTranslator.GetDisplayName(col.ColumnName);
                    string strValue = value.ToString()!.Trim();

                    if (col.DataType == typeof(string) && col.MaxLength > 0 && strValue.Length > col.MaxLength)
                        errors.Add($"Строка {rowNum}: поле «{colName}» превышает {col.MaxLength} символов");

                    if (col.DataType == typeof(int) || col.DataType == typeof(decimal))
                    {
                        if (!decimal.TryParse(strValue, out decimal numValue))
                        {
                            errors.Add($"Строка {rowNum}: в поле «{colName}» должно быть число");
                        }
                        else
                        {
                            if (col.ColumnName == "year" && numValue < 868)
                                errors.Add($"Строка {rowNum}: год не может быть меньше 868");
                            if (col.ColumnName == "sale_price" && numValue <= 0)
                                errors.Add($"Строка {rowNum}: цена должна быть больше 0");
                            if (col.ColumnName == "purchase_price" && numValue <= 0)
                                errors.Add($"Строка {rowNum}: цена должна быть больше 0");
                            if (col.ColumnName == "quantity" && numValue < 0)
                                errors.Add($"Строка {rowNum}: количество не может быть отрицательным");
                        }
                    }

                    if (col.DataType == typeof(DateTime) || col.DataType == typeof(DateOnly))
                    {
                        if (!DateTime.TryParse(strValue, out _) && !DateOnly.TryParse(strValue, out _))
                            errors.Add($"Строка {rowNum}: в поле «{colName}» должна быть дата");
                    }
                }
            }

            return errors;
        }
    }
}