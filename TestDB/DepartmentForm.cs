using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestDB
{
    public partial class DepartmentForm : Form
    {
        SqlConnection connect = new SqlConnection(TestDB.Utility.GetConnectionString());

        public DepartmentForm(Department depart)
        {
            InitializeComponent();
            //Флаг указывает форма используется для добавления или редактирования данных

            if (Add.AddOrEditFlag)
            {
                Guid id;
                id = Guid.NewGuid();
                textBox1.Text = id.ToString();

                string sql_query = "SELECT Name FROM Department";
                comboBox1.Items.AddRange(Transform.LoadArray(sql_query).ToArray());
                //comboBox1.Items.Add("");
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                //Задаем параметры в поля

                textBox1.Text = depart.ID;
                textBox2.Text = depart.Name;
                textBox3.Text = depart.Code;
                //comboBox1.Items.Add(Add.Parameters[3]);
                textBox1.Enabled = false;

                //преобразовываем ID в имя
                string sql_query = "SELECT Name FROM Department WHERE Department.ID = " +
                    "\'" + depart.ParentDepartmentID + "\'";
                string name = Transform.ID_Name(sql_query);

                //Формирование списка депаратментов для выпадающего меню, его загрузка...
                //...и устанавка нужного имени департамента по умолчанию
                sql_query = "SELECT Name FROM Department";
                comboBox1.Items.AddRange(Transform.LoadArray(sql_query));
               // comboBox1.Items.Add("");
                comboBox1.SelectedIndex = comboBox1.Items.IndexOf(name);
            }
        }

        private void Add_Note_Load(object sender, EventArgs e)
        {

        }

        //Кнопка Cancel
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Кнопка ОК
        private void button1_Click(object sender, EventArgs e)
        {
            if (Add.AddOrEditFlag)          //Если форма используется для добавления
            {                               //Формируем SQL запрос в БД на добавление.
                if (textBox1.Text != "" && textBox2.Text != "")
                {                           //Проверка на пустоту полей

                    //Получение ID по имени департамента

                    string sql_query = "SELECT ID FROM Department WHERE Department.Name = " 
                        + "\'" + comboBox1.Text + "\'";
                    string department_ID = Transform.ID_Name(sql_query);

                    connect.Open();

                    //Запрос на добавление
                    sql_query = "INSERT INTO [Department] ([ID], [Name], [Code], [ParentDepartmentID])" +
                        " VALUES (@ID, @Name, @Code, @ParentDepartmentID)";
                    using (SqlCommand cmdSQL = new SqlCommand(sql_query, connect))
                    {
                        cmdSQL.Parameters.Add("@ID", textBox1.Text);
                        cmdSQL.Parameters.Add("@Name", textBox2.Text);
                        cmdSQL.Parameters.Add("@Code", textBox3.Text);
                        if (comboBox1.Items[comboBox1.SelectedIndex] == "")
                            cmdSQL.Parameters.Add("@ParentDepartmentID", DBNull.Value);
                        else
                            cmdSQL.Parameters.Add("@ParentDepartmentID", department_ID);
                        cmdSQL.ExecuteNonQuery();
                    }
                    connect.Close();
                    this.Close();
                }
                else { MessageBox.Show("Введите все параметры!"); }
            }
            else            //Если форма используется для редактирования
            {
                if (textBox1.Text != "" && textBox2.Text != "")
                {           //Проверка на пустоту полей
                    //Получение ID по имени департамента
                    string sql_query = "SELECT ID FROM Department WHERE Department.Name = "
                        + "\'" + comboBox1.Text + "\'";
                    string department_ID = Transform.ID_Name(sql_query);

                    connect.Open();
                    sql_query = "UPDATE [Department] SET ID=@ID, Name=@Name, Code=@Code, " +
                        "ParentDepartmentID=@ParentDepartmentID WHERE ID=@ID";

                    using (SqlCommand cmdSQL = new SqlCommand(sql_query, connect))
                    {
                        cmdSQL.Parameters.Add("@ID", textBox1.Text);
                        cmdSQL.Parameters.Add("@Name", textBox2.Text);
                        cmdSQL.Parameters.Add("@Code", textBox3.Text);
                        if(comboBox1.Items[comboBox1.SelectedIndex] == "")
                            cmdSQL.Parameters.Add("@ParentDepartmentID", DBNull.Value);
                        else
                            cmdSQL.Parameters.Add("@ParentDepartmentID", department_ID);
                        cmdSQL.ExecuteNonQuery();
                    }
                    connect.Close();
                    this.Close();
                }
                else { MessageBox.Show("Введите все параметры!"); }
            }
        }
    }
}