using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace WebServiceDemo.Helper
{
   public static class SQLHelper
    {
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
