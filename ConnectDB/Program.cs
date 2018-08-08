using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectDB
{
	class Program
	{
		// Read destination file location from App.Config
		private static string _FileLocation = ConfigurationManager.AppSettings["FileLocation"];
		// Read SQL Query from App.Config
		private static string query1 = ConfigurationManager.AppSettings["Query1"];

		static void Main(string[] args)
		{
			using (SqlConnection conn = new SqlConnection())
			{
				// Read Connection String from App.Config
				conn.ConnectionString = ConfigurationManager.ConnectionStrings["_WindowsAuthenticationConnection"].ConnectionString;
				conn.Open();
				SqlCommand command = new SqlCommand(query1, conn);
				Console.WriteLine("Executing Query");
				using (SqlDataReader reader = command.ExecuteReader())
				{
					DataTable dt = new DataTable();
					dt.Load(reader);

					StringBuilder sb = new StringBuilder();

					IEnumerable<string> columns = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
					sb.AppendLine(string.Join(",", columns));

					foreach (DataRow row in dt.Rows)
					{
						IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
						sb.AppendLine(string.Join(",", fields));
					}

					File.WriteAllText(_FileLocation, sb.ToString());

					Console.WriteLine("Write Completed.");
					Console.WriteLine("File has been created at : " + _FileLocation);
				}
			}
		}
	}
}
