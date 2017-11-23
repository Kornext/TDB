using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestDB
{
    public partial class Form1 : Form
    {
        string name_database_table = "Test DB";
        string path_database = "Test DB";
        string catalog = "Test DB";
        string current_SQL_query;

        SqlConnection connect = new SqlConnection(TestDB.Utility.GetConnectionString());
        SqlDataAdapter dataAdapter;
        DataSet dataSet;

        public Form1()
        {
            InitializeComponent();
            string sql_query = "SELECT TABLE_NAME FROM information_schema.tables";
            current_SQL_query = sql_query;
            RefreshTable(sql_query);

            treeView1.Nodes.Add("Test DB");
            treeView1.Nodes[0].Nodes.Add("Department");
            treeView1.Nodes[0].Nodes.Add("Empoyee");
        }

        //Кнопка возврата в предыдущую таблицу
        private void testDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Если путь мы находимося в таблице сотрудников определенного отдела
            //То есть где есть путь TestDB -> Department -> Employee
            if (path_database.Contains("Department -> "))
            {
                string sql_query = "SELECT * FROM Department";
                current_SQL_query = sql_query;
                RefreshTable(sql_query);
                path_database = name_database_table + " -> Department";
                testDBToolStripMenuItem.Text = path_database;
                catalog = "Department";
            }
            //Если мы не находимся в каталоге таблиц, т.е. просматриваю одну из таблиц
            else if (testDBToolStripMenuItem.Text != name_database_table)
            {
                string sql_query = "SELECT TABLE_NAME FROM information_schema.tables";
                current_SQL_query = sql_query;
                RefreshTable(sql_query);
                path_database = name_database_table;
                testDBToolStripMenuItem.Text = path_database;
                catalog = name_database_table;
            } 
            //Если таблица имеет колонку AGE сформированную в сотрудник, удаляем ее
            if (dataGridView1.Columns[0].HeaderText == "Age")
            {
                dataGridView1.Columns.Remove(dataGridView1.Columns[0]);
            }


        }

        //Событие двойного щелчка по ячейке 
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //Если двойной щелчек произошел в главном каталоге со списком таблиц БД
            if (path_database == name_database_table)
            {
                path_database += " -> " + dataGridView1.CurrentCell.Value.ToString();
                testDBToolStripMenuItem.Text = path_database;
                string sql_query = "SELECT * FROM "
                    + dataGridView1.CurrentCell.Value.ToString();
                current_SQL_query = sql_query;
                catalog = dataGridView1.CurrentCell.Value.ToString();
                RefreshTable(sql_query);

                //Добавление столбца "Возраст"
                if (catalog=="Empoyee")
                {
                    Age();
                }
                if(catalog=="Department")
                {
                    DepartmentName();
                }
            }
            //Если двойной щелчек произошел по ячейке с именем департамента
            //Выводим всех сотрудников этого департамента
            else if(dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].HeaderText == "Name")
            {
                path_database += " -> " + dataGridView1.CurrentCell.Value.ToString();
                testDBToolStripMenuItem.Text = path_database;
                string ds = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString();
                string sql_query = "SELECT * FROM Empoyee WHERE Empoyee.DepartmentID = " + "\'" + ds + "\'";
                current_SQL_query = sql_query;
                RefreshTable(sql_query);
                catalog = "sql_query";
            }
        }

        //Метод обновления по Таблицы БД по SQL запросу
        public void RefreshTable(string sql_query) 
        {
            try
            {
                dataGridView1.Columns.Clear();
                connect.Open();
                dataAdapter = new SqlDataAdapter(sql_query, connect);
                dataSet = new DataSet();
                dataAdapter.Fill(dataSet, "Tables");
                connect.Close();
                dataGridView1.DataSource = dataSet.Tables[0];

                dataGridView1.ReadOnly = true;
                dataAdapter.Dispose();
                dataSet.Dispose();
            }
            catch
            {
                MessageBox.Show("Ошибка!");
            }
        }

        //Кнопка удаления записи
        private void button2_Click(object sender, EventArgs e)
        {
            if(catalog=="Department" || catalog == "Empoyee")
                Delete(catalog);
            RefreshTable(current_SQL_query);
        }

        //Метод удаления записи
        private void Delete(string catalog)
        {
            try
            {
                string sql_query = "DELETE FROM ";
                sql_query += catalog + " WHERE " + catalog + ".ID = \'" +
                     dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString() +
                     "\'";
                connect.Open();
                SqlCommand cmd = new SqlCommand(sql_query, connect);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка удалениея!");
            }
            finally
            {
                connect.Close();
            }
        }

        //Метод добавления колонки Age
        private void Age()
        {
            dataGridView1.Columns.Add("Age", "Age");
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                DateTime dt = (DateTime)dataGridView1.Rows[i].Cells[4].Value;
                int age = DateTime.Now.Year - dt.Year;
                dt = new DateTime(DateTime.Now.Year, dt.Month, dt.Day);
                if(DateTime.Now < dt)
                {
                    age--;
                }
                dataGridView1.Rows[i].Cells[dataGridView1.Columns.Count - 1].Value =
                    Convert.ToString(age);
            }
        }

        //Формирование столбца с именем родительского департамента
        private void DepartmentName()
        {
            dataGridView1.Columns.Add("ParentDepartment", "ParentDepartment");
            for(int i=0; i< dataGridView1.Rows.Count-1; i++)
            {
                string sql_query = "SELECT Name FROM Department WHERE Department.ID = " +
                    "\'" + dataGridView1.Rows[i].Cells[dataGridView1.Columns.Count - 2].Value.ToString() + "\'";
                dataGridView1.Rows[i].Cells[dataGridView1.Columns.Count - 1].Value = Transform.ID_Name(sql_query);
            }
        }

        //Кнопка добавления новой записи
        private void button3_Click(object sender, EventArgs e)
        {
            //В зависимости от каталога вызывается соответствующая форма
            if (catalog == "Department")
            {
                Department depart = new Department();
                Add.AddOrEditFlag = true;
                DepartmentForm add = new DepartmentForm(depart);
                add.ShowDialog();
                Refresh();
            }
            if(catalog == "Empoyee")
            {
                Add.AddOrEditFlag = true;
                Employee empl = new Employee();
                EmployeeForm add = new EmployeeForm(empl, "");
                add.ShowDialog();
                Refresh();
            }
        }

        //Кнопка редактирования записи
        private void button5_Click(object sender, EventArgs e)
        {
            Add.AddOrEditFlag = false;

            if(catalog == "Department")
            {
                Department depart = new Department();
                depart.ID = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString();
                depart.Name = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1].Value.ToString();
                depart.Code = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[2].Value.ToString();
                depart.ParentDepartmentID = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[3].Value.ToString();

                DepartmentForm dp = new DepartmentForm(depart);
                dp.ShowDialog();
                Refresh();
            }
            else if(catalog == "Empoyee")
            {
                Employee empl = new Employee();
                empl.FirstName = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1].Value.ToString();
                empl.SurName = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[2].Value.ToString();
                empl.Patronymic = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[3].Value.ToString();
                empl.DataOfBirth = (DateTime)dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[4].Value;
                empl.DocSeries = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[5].Value.ToString();
                empl.DocNumber = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[6].Value.ToString();
                empl.Position = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[7].Value.ToString();
                empl.ParentDepartment = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[8].Value.ToString();

                string ID = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString();
                EmployeeForm emp = new EmployeeForm(empl, ID);
                emp.ShowDialog();
                Refresh();
            }
        }

        //Кнопка обновления
        private void button1_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            RefreshTable(current_SQL_query);
            if (catalog == "Empoyee")
            {
                Age();
            }
            if(catalog == "Department")
            {
                DepartmentName();
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                TreeNode node = treeView1.SelectedNode;
                switch (node.Text)
                {
                    case "Department":
                        {
                            path_database = name_database_table + " -> " + node.Text;
                            testDBToolStripMenuItem.Text = path_database;
                            current_SQL_query = "SELECT * FROM " + node.Text;
                            catalog = node.Text;
                            Refresh();
                            break;
                        }
                    case "Empoyee":
                        {
                            path_database = name_database_table + " -> " + node.Text;
                            testDBToolStripMenuItem.Text = path_database;
                            current_SQL_query = "SELECT * FROM " + node.Text;
                            catalog = node.Text;
                            Refresh();
                            break;
                        }
                    case "Test DB":
                        {
                            testDBToolStripMenuItem.Text = name_database_table;
                            string sql_query = "SELECT TABLE_NAME FROM information_schema.tables";
                            current_SQL_query = sql_query;
                            catalog = node.Text;
                            path_database = name_database_table;
                            RefreshTable(sql_query);
                            break;
                        }
                }
            }
            catch { }
        }
    }
}