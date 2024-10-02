using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL
{
    public  class DepartmentStatistics
    {
        public string DepartmentName { get; set; }  // The name of the department
        public int EmployeeCount { get; set; }  // Total number of employees in the department
        public decimal TotalSalary { get; set; }  // Total salary of all employees in the department
    }
}
