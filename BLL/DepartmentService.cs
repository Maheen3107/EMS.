using DAL;
using BOL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class DepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        // Method to get all departments
        public IEnumerable<Department> GetDepartments()
        {
            return _departmentRepository.GetAllDepartments();
        }

        // Method to get a single department by ID
        public Department GetDepartmentById(int departmentId)
        {
            var department = _departmentRepository.GetDepartmentById(departmentId);
            if (department == null)
                throw new Exception($"Department with ID {departmentId} does not exist.");

            return department;
        }

        // Method to add a new department
        public void AddDepartment(Department department)
        {
            if (string.IsNullOrWhiteSpace(department.DepartmentName))
                throw new Exception("Department Name cannot be empty.");

            if (department.Budget <= 0)
                throw new Exception("Department budget must be greater than 0.");

            _departmentRepository.AddDepartment(department);
        }

        // Method to update an existing department
        public void UpdateDepartment(Department department)
        {
            var existingDepartment = _departmentRepository.GetDepartmentById(department.DepartmentID);
            if (existingDepartment == null)
                throw new Exception($"Department with ID {department.DepartmentID} does not exist.");

            if (string.IsNullOrWhiteSpace(department.DepartmentName))
                throw new Exception("Department Name cannot be empty.");

            if (department.Budget <= 0)
                throw new Exception("Department budget must be greater than 0.");

            _departmentRepository.UpdateDepartment(department);
        }

        // Method to delete a department by ID
        public void DeleteDepartment(int departmentId)
        {
            var department = _departmentRepository.GetDepartmentById(departmentId);
            if (department == null)
                throw new Exception($"Department with ID {departmentId} does not exist.");

            _departmentRepository.DeleteDepartment(departmentId);
        }

        // Method to get total employee count and salary per department
        public DepartmentStatistics GetDepartmentStatistics(int departmentId)
        {
            var department = _departmentRepository.GetDepartmentById(departmentId);
            if (department == null)
                throw new Exception($"Department with ID {departmentId} does not exist.");

            // Assuming the repository has methods to fetch employee count and salary
            var employeeCount = _departmentRepository.GetEmployeeCountByDepartment(departmentId);
            var totalSalary = _departmentRepository.GetTotalSalaryByDepartment(departmentId);

            return new DepartmentStatistics
            {
                DepartmentName = department.DepartmentName,
                EmployeeCount = employeeCount,
                TotalSalary = totalSalary
            };
        }
        public IEnumerable<DepartmentStatistics> GetDepartmentStatistics()
        {
            return _departmentRepository.GetDepartmentStatistics();
        }

        // Method to hire a new employee while ensuring the budget does not exceed
        public void HireEmployee(int departmentId, Employee newEmployee)
        {
            var department = _departmentRepository.GetDepartmentById(departmentId);
            if (department == null)
                throw new Exception($"Department with ID {departmentId} does not exist.");

            var totalSalary = _departmentRepository.GetTotalSalaryByDepartment(departmentId);

            // Calculate the new total salary after hiring the new employee
            var newTotalSalary = totalSalary + newEmployee.Salary;

            // Check if the new total salary exceeds the department's budget
            if (newTotalSalary > department.Budget)
                throw new Exception("Cannot hire the employee. Total salary exceeds department budget.");

            // Proceed with hiring if within budget
            _departmentRepository.AddEmployeeToDepartment(departmentId, newEmployee);
        }
    }

}
