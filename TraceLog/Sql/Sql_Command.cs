using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToolkit.SQL
{
    /// <summary>
    /// ExecuteNonQuery	执行 Transact-sql INSERT、DELETE、UPDATE 和 SET 语句等命令。
    /// ExecuteScalar 检索单个值(例如，从数据库) 聚合值。
    /// ExecuteXmlReader 将 CommandText 发送到 Connection，并生成一个 XmlReader 对象。
    /// </summary>
    public static class Sql_Command
    {
        //Data Source = 服务器名称\数据库实例名 ; Initial Catalog = 数据库名称 ; User ID = 用户名 ; Password = 密码;
        private static string sql_connStr = "Data source =.; Initial Catalog = Host_Test; User ID = sa; Password=Aa123456";

        public static bool Insert(string volumeName, object value, string tableName)
        {
            bool flag = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(sql_connStr))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"INSERT INTO [{tableName}]([{volumeName}]) VALUES({value})";
                        int count = command.ExecuteNonQuery();  //用于增删改
                        if (count > 0) { flag = true; }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail to Start the SQL:" + ex.ToString());
            }
            return flag;
        }

        public static bool Delete(string volumeName, object value, string tableName)
        {
            bool flag = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(sql_connStr))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"DELETE FROM [{tableName}] WHERE([{volumeName}]) = ({value})";
                        int count = command.ExecuteNonQuery();  //用于增删改
                        if (count > 0) { flag = true; }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail to Start the SQL:" + ex.ToString());
            }
            return flag;
        }

        public static bool Update(string index, string volumeName, object value, string tableName)
        {
            bool flag = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(sql_connStr))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"UPDATE [{tableName}] SET [{volumeName}] = {value} WHERE [Index] = {index} ";
                        int count = command.ExecuteNonQuery();  //用于增删改
                        if (count > 0) { flag = true; }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail to Start the SQL:" + ex.ToString());
            }


            return flag;
        }


        public static object Select(string index,string volumeName,string tableName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(sql_connStr))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT [{volumeName}] FROM [{tableName}] WHERE [Index] = {index}";
                        object obj = command.ExecuteScalar();  //用于查找
                        connection.Close();
                        return obj;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail to Start the SQL:" + ex.ToString());
            }

            return null;
        }
    }
}
