using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics.Eventing.Reader;

namespace sqlblkcpyNotifyAfter
{
    class Program
    {
        static void Main(string[] args)
        {
            string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using(SqlConnection sourcecon = new SqlConnection(cs))
            {
                var cmd = new SqlCommand("select * from Products_Source", sourcecon);
                sourcecon.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())                   
                {
                    using(SqlConnection destinationcon = new SqlConnection(cs))
                    {
                        using(SqlBulkCopy sb = new SqlBulkCopy(destinationcon))
                        {                          
                            sb.BatchSize = 10000;
                            sb.NotifyAfter = 5000;
                            sb.SqlRowsCopied += (sender,eventsArgs) =>
                            {
                                Console.WriteLine(eventsArgs.RowsCopied + "  Number of rows processed");
                            };
                            sb.DestinationTableName="Products_Destination";
                            destinationcon.Open();
                            sb.WriteToServer(rdr);

                            Console.Read();

                        }
                    }
                }
            }
        }
    }
}
