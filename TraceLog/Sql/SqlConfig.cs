using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace MyToolkit.SQL
{
    public static class SqlConfig
    {
        //Data Source = 服务器名称\数据库实例名 ; Initial Catalog = 数据库名称 ; User ID = 用户名 ; Password = 密码;
        private static string sql_connStr = "Data source =.; Initial Catalog = Host_Test; User ID = sa; Password=Aa123456";
        public static void Start()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(sql_connStr))
                {
                    connection.Open();
                    SqlTransaction tx = connection.BeginTransaction("transName");
                    Console.WriteLine("数据库连接成功");

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.Transaction = tx;
                        command.CommandText = "SELECT * FROM [OrderStatus]";
                        //command.ExecuteNonQuery();  //用于增删改
                        //command.ExecuteScalar();  //用于增删改
                        //command.ExecuteReader();  //用于查

                        //using(SqlDataReader reader = command.ExecuteReader())
                        //{
                        //    while(reader.Read())
                        //    {
                        //        Console.WriteLine(reader.ToString());
                        //    }
                        //}

                        //string str = (string)command.ExecuteScalar();
                        //Console.WriteLine(str);

                        SqlDataAdapter cmd = new SqlDataAdapter(command);
                        DataSet data = new DataSet();
                        cmd.Fill(data);

                    }


                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail to Start the SQL:"+ ex.ToString()) ;
            }
        }


        public static DataSet Query()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(sql_connStr))
                {
                    connection.Open();
                    SqlTransaction tx = connection.BeginTransaction("transName");
                    Console.WriteLine("数据库连接成功");

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.Transaction = tx;
                        command.CommandText = "SELECT * FROM [OrderStatus]";

                        SqlDataAdapter cmd = new SqlDataAdapter(command);
                        DataSet data = new DataSet();
                        cmd.Fill(data);
                        return data;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail to Start the SQL:" + ex.ToString());
                return null;
            }
        }





        public static void CreateCommand(string queryString, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();

            }
        }

        public static void ReadOrderData()
        {
            string queryString =
                "SELECT * FROM [OrderStatus];";
            using (SqlConnection connection = new SqlConnection(sql_connStr))
            {
                SqlCommand command = new SqlCommand(
                    queryString, connection);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(String.Format("{0}, {1}", reader[0], reader[1]));
                    }
                }
            }
        }
    }
}
