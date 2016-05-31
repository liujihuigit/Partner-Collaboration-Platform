using MySQLDriverCS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;

namespace PaCS.Tools
{
    class ConnForMySQL
    {

        private static string server = ConfigurationManager.ConnectionStrings[server].ConnectionString;
        private static string database = ConfigurationManager.ConnectionStrings[database].ConnectionString;
        private static string login = ConfigurationManager.ConnectionStrings[login].ConnectionString;
        private static string password = ConfigurationManager.ConnectionStrings[password].ConnectionString;


        public static int ExecuteNoQuery(String sql,MySQLParameter[] parameters)
        {

            using (MySQLConnection conn = new MySQLConnection(new MySQLConnectionString(server, database, login, password).AsString))
            {

                conn.Open();

                //防止乱码
                MySQLCommand commn = new MySQLCommand("set names gb2312", conn);
                commn.ExecuteNonQuery();
                //连接语句和SQL
                MySQLCommand cmd = new MySQLCommand(sql, conn);
                //添加参数
                cmd.Parameters.AddRange( parameters);
                //返回执行结果
                return cmd.ExecuteNonQuery();

            }
        
        }

        public static object ExecuteScalar(String sql, MySQLParameter[] parameters)
        {

            using (MySQLConnection conn = new MySQLConnection(new MySQLConnectionString(server, database, login, password).AsString))
            {

                conn.Open();
                //防止乱码
                MySQLCommand commn = new MySQLCommand("set names gb2312", conn);
                commn.ExecuteNonQuery();

                MySQLCommand cmd = new MySQLCommand(sql, conn);
                //添加参数
                cmd.Parameters.AddRange(parameters);
                
                return cmd.ExecuteNonQuery();
            }
        
        }

        //较少的时候
        public static DataTable ExecuteReaderEx(String sql, MySQLParameter[] parameters)
        {

            using (MySQLConnection conn = new MySQLConnection(new MySQLConnectionString(server, database, login, password).AsString))
            {

                conn.Open();
                //防止乱码
                MySQLCommand commn = new MySQLCommand("set names gb2312", conn);
                commn.ExecuteNonQuery();

                MySQLCommand cmd = new MySQLCommand(sql, conn);
                //添加参数
                cmd.Parameters.AddRange(parameters);

                MySQLDataAdapter mda = new MySQLDataAdapter(cmd);

                //查询出的数据是存在DataTable中的，DataTable可以理解成为一个虚拟的表，DataTable中的一行为一条记录，一列为一个数据库字段  


                DataTable dt = new DataTable();
                mda.Fill(dt);  

                return dt;
            }

        }

        public static DataSet ExecuteReaderEx2(String sql, MySQLParameter[] parameters)
        {


            using (MySQLConnection conn = new MySQLConnection(new MySQLConnectionString(server, database, login, password).AsString))
            {

                conn.Open();
                //防止乱码
                MySQLCommand commn = new MySQLCommand("set names gb2312", conn);
                commn.ExecuteNonQuery();

                MySQLCommand cmd = new MySQLCommand(sql, conn);
                //添加参数
                cmd.Parameters.AddRange(parameters);

                MySQLDataAdapter mda = new MySQLDataAdapter(cmd);

                //查询出的数据是存在DataTable中的，DataTable可以理解成为一个虚拟的表，DataTable中的一行为一条记录，一列为一个数据库字段  


                DataSet ds = new DataSet();
                mda.Fill(ds);
                return ds;
            }

        }

    }
}
