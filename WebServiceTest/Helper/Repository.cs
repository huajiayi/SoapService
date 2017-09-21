using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceDemo.Helper
{
   public static class Repository
    {
        static Repository()
        {
            SqlMapper.AddTypeMap(typeof(DateTime), DbType.DateTime2);
            SqlMapper.AddTypeMap(typeof(DateTime?), DbType.DateTime2);
        }

        public static IEnumerable<T> Query<T>(string sql, object parameter = null)
        {
            var @string = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

            using (var conn = new SqlConnection(@string))
            {
                conn.Open();

                return conn.Query<T>(sql, parameter);
            }
        }

        public static void Execute(string sql, object parameter = null)
        {
            var @string = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

            using (var conn = new SqlConnection(@string))
            {
                conn.Open();

                 conn.Execute(sql, parameter);
            }
        }
    }
}
