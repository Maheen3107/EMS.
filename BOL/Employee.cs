using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL
{
    public class Employee
    {
        public int EmployeeID { get; set; }  // Primary Key
        public string FirstName { get; set; }  // First name of the employee
        public string LastName { get; set; }  // Last name of the employee
        public string Position { get; set; }  // Employee's position in the company
        public decimal Salary { get; set; }  // Employee's salary

        // Foreign key relation to the Department
        public int DepartmentID { get; set; }
        public Department Department { get; set; }  // Department reference for the employee
    }
}
