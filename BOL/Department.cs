using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL
{
    public class Department
    {
        public int DepartmentID { get; set; }  // Primary Key
        public string DepartmentName { get; set; }  // Name of the department
        public decimal Budget { get; set; }  // Budget allocated for the department
    }
}
