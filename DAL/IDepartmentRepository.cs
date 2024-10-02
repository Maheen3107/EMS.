using System.Collections.Generic;
using BOL; // Assuming BOL contains the Department and Employee models

namespace DAL
{
    public interface IDepartmentRepository
    {
        // Retrieve all departments
        IEnumerable<Department> GetAllDepartments();

        // Retrieve a department by ID
        Department GetDepartmentById(int departmentId);

        // Add a new department
        void AddDepartment(Department department);

        // Update an existing department
        void UpdateDepartment(Department department);

        // Delete a department by ID
        void DeleteDepartment(int departmentId);

        // Get the number of employees in a specific department
        int GetEmployeeCountByDepartment(int departmentId);

        // Get the total salary of all employees in a specific department
        decimal GetTotalSalaryByDepartment(int departmentId);

        // Add an employee to a specific department
        void AddEmployeeToDepartment(int departmentId, Employee employee);
        IEnumerable<DepartmentStatistics> GetDepartmentStatistics();
    }
}
