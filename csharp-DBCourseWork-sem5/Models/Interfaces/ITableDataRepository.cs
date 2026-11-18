using System.Data;

namespace csharp_DBCourseWork_sem5.Models.Interfaces
{
    public interface ITableDataRepository
    {
        DataTable Load(string tableName, int limit = 1000);
        DataTable ExecuteRawSql(string sql);
        int Save(string tableName, IReadOnlyList<string> primaryKey, IReadOnlyCollection<DataRow> addedRows, IReadOnlyCollection<DataRow> modifiedRows);
        int Delete(string tableName, IReadOnlyList<string> primaryKey, IReadOnlyCollection<DataRow> rows);
    }
}