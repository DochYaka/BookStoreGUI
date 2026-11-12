using csharp_DBCourseWork_sem5.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace csharp_DBCourseWork_sem5.Services
{
    public static class ParseRawSqlService
    {
        public static DataTable SqlQuerry(string rawSql)
        {
            using var db = new BookStoreDbContext();
            db.Database.OpenConnection();

            using var command = db.Database.GetDbConnection().CreateCommand();
            command.CommandText = rawSql;
            command.CommandTimeout = 30;

            using var reader = command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

    }
}
