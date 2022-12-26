using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessClient.Services
{
    internal class DataService
    {
        public static SqlConnection DirectConnect()
        {
            string connectionString;
            SqlConnection cnn;
            connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=WKEXP905;Integrated Security=True";
            return new SqlConnection(connectionString);
        }
    }
}
