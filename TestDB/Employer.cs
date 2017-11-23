using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDB
{
    public class Employee
    {
        public int ID;
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string Patronymic { get; set; }
        public DateTime DataOfBirth { get; set; }
        public string DocSeries { get; set; }
        public string DocNumber { get; set; }
        public string Position { get; set; }
        public string ParentDepartment { get; set; }

        public Employee()
        { }

        public Employee(int ID, string FirstName, string Surname, string Patronymic, 
            DateTime DateOfBirth, string DocSeries, string DocNumber, string Position,
            string ParentDepartment)
        {
            this.ID = ID;
            this.FirstName = FirstName;
            this.SurName = SurName;
            this.Patronymic = Patronymic;
            this.DataOfBirth = DataOfBirth;
            this.DocSeries = DocSeries;
            this.DocNumber = DocNumber;
            this.Position = Position;
            this.ParentDepartment = ParentDepartment;
        }
    }
}
