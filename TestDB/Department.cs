using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDB
{
    public class Department
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ParentDepartmentID { get; set; }

        public Department()
        {
 
        }
        public Department(string ID, string Name, string Code, string ParentDepartmentID)
        {
            this.ID = ID;
            this.Name = Name;
            this.Code = Code;
            this.ParentDepartmentID = ParentDepartmentID;
        }
    }
}
