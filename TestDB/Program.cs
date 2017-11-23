using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestDB
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    static class Add //Класс для передачи параметров в формы
    {
        //public static string[] Parameters { get; set; }
        public static bool AddOrEditFlag { get; set; } //true - Add / false - Edit

    }

    internal class Utility
    {
        internal static string GetConnectionString()
        {
            string returnValue = null;

            ConnectionStringSettings settings =
            ConfigurationManager.ConnectionStrings["TestDB.Properties.Settings.connString"];

            if (settings != null)
                returnValue = settings.ConnectionString;

            return returnValue;
        }
    }

    class Transform
    {
        //преобразование имени в ID или ID в имя
        private static readonly SqlConnection connect = new SqlConnection(TestDB.Utility.GetConnectionString());

        public static string ID_Name(string sql_query)
        {
            string department_ID = "";
            connect.Open();
            SqlCommand cmdSQL = new SqlCommand(sql_query, connect);
            SqlDataReader reader = cmdSQL.ExecuteReader();
            try
            {

                while (reader.Read())
                {
                    department_ID = reader[0].ToString();
                }
            }
            catch(Exception e)
            {
                //MessageBox.Show(e.ToString());
                department_ID = "";
            }
            connect.Close();
            return department_ID;
        }

        //загрузка параметров в массив
        public static string[] LoadArray(string sql_query)
        {
            List<string> list = new List<string>();
            SqlCommand cmd = new SqlCommand(sql_query, connect);
            connect.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(reader[0].ToString());
            }
            connect.Close();

            return list.ToArray();
        }
    }
}
