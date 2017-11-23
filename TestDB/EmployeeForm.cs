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
    public partial class EmployeeForm : Form
    {
        SqlConnection connect = new SqlConnection(TestDB.Utility.GetConnectionString());
        string ID;

        public EmployeeForm(Employee empl, string ID)
        {
            InitializeComponent();
            //Флаг указывает форма используется для добавления или редактирования данных

            this.ID = ID;

            if (Add.AddOrEditFlag)
            {
                string sql_query = "SELECT Name FROM Department";

                comboBox1.Items.AddRange(Transform.LoadArray(sql_query).ToArray());
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                //Задаем параметры в поля
                textBox2.Text = empl.FirstName;
                textBox3.Text = empl.SurName;
                textBox4.Text = empl.Patronymic;
                textBox6.Text = empl.DocSeries;
                dateTimePicker1.Value = empl.DataOfBirth;
                textBox7.Text = empl.DocNumber;
                textBox8.Text = empl.Position;
                //comboBox1.Items.Add(Add.Parameters[8]);

                //преобразовываем ID в имя
                string sql_query = "SELECT Name FROM Department WHERE Department.ID = " +
                    "\'" + empl.ParentDepartment + "\'";
                string name = Transform.ID_Name(sql_query);

                //Формирование списка депаратментов для выпадающего меню, его загрузка...
                //...и устанавка нужного имени департамента по умолчанию
                sql_query = "SELECT Name FROM Department";
                comboBox1.Items.AddRange(Transform.LoadArray(sql_query));
                comboBox1.SelectedIndex = comboBox1.Items.IndexOf(name);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        //Кнопка ОК
        private void button1_Click(object sender, EventArgs e)
        {
            if (Add.AddOrEditFlag)      //Если форма используется для добавления
            {                           //Формируем SQL запрос в БД на добавление.
                if (textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && textBox8.Text != "")
                { //Проверка на пустоту полей

                    //Получение ID по имени департамента
                    string sql_query = "SELECT ID FROM Department WHERE Department.Name = "
                        + "\'" + comboBox1.Text + "\'";
                    string department_ID = Transform.ID_Name(sql_query);

                    connect.Open();

                    //Запрос на добавление
                    sql_query = "INSERT INTO [Empoyee] ([FirstName], [SurName], [Patronymic], [DateofBirth], " +
                        "[DocSeries], [DocNumber], [Position], [DepartmentID])" + " VALUES " +
                        "(@FirstName, @SurName, @Patronymic, @DateofBirth, @DocSeries, @DocNumber, @Position, " +
                        "@DepartmentID)";
                    using (SqlCommand cmdSQL = new SqlCommand(sql_query, connect))
                    {
                        cmdSQL.Parameters.Add("@FirstName", textBox2.Text);
                        cmdSQL.Parameters.Add("@SurName", textBox3.Text);
                        cmdSQL.Parameters.Add("@Patronymic", textBox4.Text);
                        cmdSQL.Parameters.Add("@DateofBirth", dateTimePicker1.Value.ToShortDateString().ToString());
                        cmdSQL.Parameters.Add("@DocSeries", textBox6.Text);
                        cmdSQL.Parameters.Add("@DocNumber", textBox7.Text);
                        cmdSQL.Parameters.Add("@Position", textBox8.Text);
                        cmdSQL.Parameters.Add("@DepartmentID", department_ID);
                        cmdSQL.ExecuteNonQuery();
                    }
                    connect.Close();
                    this.Close();
                }
                else { MessageBox.Show("Введите все параметры!"); }
            }
            else         //Если форма используется для редактирования
            { 
                if (textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && textBox8.Text != "")
                {//Проверка на пустоту полей

                    //Получение ID по имени департамента
                    string sql_query = "SELECT ID FROM Department WHERE Department.Name = "
                        + "\'" + comboBox1.Text + "\'";
                    string department_ID = Transform.ID_Name(sql_query);

                    connect.Open();
                    sql_query = "UPDATE [Empoyee] SET FirstName=@FirstName, SurName=@SurName," +
                        " Patronymic=@Patronymic, DateofBirth=@DateofBirth, DocSeries=@DocSeries," +
                        " DocNumber=@DocNumber, Position=@Position, DepartmentID=@DepartmentID WHERE ID = \'"
                        + ID + "\'";
                    using (SqlCommand cmdSQL = new SqlCommand(sql_query, connect))
                    {
                        cmdSQL.Parameters.Add("@FirstName", textBox2.Text);
                        cmdSQL.Parameters.Add("@SurName", textBox3.Text);
                        cmdSQL.Parameters.Add("@Patronymic", textBox4.Text);
                        cmdSQL.Parameters.Add("@DateofBirth", dateTimePicker1.Value.ToString());
                        cmdSQL.Parameters.Add("@DocSeries", textBox6.Text);
                        cmdSQL.Parameters.Add("@DocNumber", textBox7.Text);
                        cmdSQL.Parameters.Add("@Position", textBox8.Text);
                        if (comboBox1.Items[comboBox1.SelectedIndex] == "")
                            cmdSQL.Parameters.Add("@DepartmentID", DBNull.Value);
                        else
                            cmdSQL.Parameters.Add("@DepartmentID", department_ID);
                        cmdSQL.ExecuteNonQuery();
                    }
                    connect.Close();
                    this.Close();
                }
                else { MessageBox.Show("Введите все параметры!"); }
            }
        }

        //Кнопка Cancel
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void Employee_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
