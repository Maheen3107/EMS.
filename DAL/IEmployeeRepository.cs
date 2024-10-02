using System;
using System.Collections.Generic;
using BOL;

namespace DAL
{
    public interface IEmployeeRepository
    {
        // Retrieve all employees
        IEnumerable<Employee> GetAllEmployees();

        // Retrieve an employee by ID
        Employee GetEmployeeById(int employeeId);

        // Add a new employee
        void AddEmployee(Employee employee);

        // Update an existing employee
        void UpdateEmployee(Employee employee);

        // Delete an employee
        void DeleteEmployee(int employeeId);

        // Search employees by name, department, or position
        IEnumerable<Employee> SearchEmployees(string name = null, int? departmentId = null, string position = null);
    }
}
