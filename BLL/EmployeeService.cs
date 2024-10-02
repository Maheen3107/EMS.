using DAL;
using System;
using System.Collections.Generic;
using BOL;

namespace BLL
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public EmployeeService(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
        }

        // Get all employees
        public IEnumerable<Employee> GetEmployees()
        {
            var employees = _employeeRepository.GetAllEmployees();

            foreach (var employee in employees)
            {
                employee.Department = _departmentRepository.GetDepartmentById(employee.DepartmentID);
            }

            return employees;
        }

        // Add a new employee
        public void AddEmployee(Employee employee)
        {
            // Validate salary
            if (employee.Salary <= 0)
                throw new Exception("Salary must be greater than 0.");

            // Validate department existence
            var department = _departmentRepository.GetDepartmentById(employee.DepartmentID);
            if (department == null)
                throw new Exception("Department does not exist.");

            // Check if department budget allows for adding the new employee
            decimal totalSalary = _departmentRepository.GetTotalSalaryByDepartment(employee.DepartmentID);
            if (totalSalary + employee.Salary > department.Budget)
                throw new Exception("Insufficient department budget for adding this employee.");

            _employeeRepository.AddEmployee(employee);
        }

        // Update an existing employee
        public void UpdateEmployee(Employee employee)
        {
            // Check if the employee exists
            var existingEmployee = _employeeRepository.GetEmployeeById(employee.EmployeeID);
            if (existingEmployee == null)
                throw new Exception($"Employee with ID {employee.EmployeeID} does not exist.");

            // Validate salary
            if (employee.Salary <= 0)
                throw new Exception("Salary must be greater than 0.");

            // Check if reassigning to a different department
            if (existingEmployee.DepartmentID != employee.DepartmentID)
            {
                // Validate the new department
                var newDepartment = _departmentRepository.GetDepartmentById(employee.DepartmentID);
                if (newDepartment == null)
                    throw new Exception("New department does not exist.");

                // Check if new department has enough budget for this employee
                decimal newDepartmentTotalSalary = _departmentRepository.GetTotalSalaryByDepartment(employee.DepartmentID);
                if (newDepartmentTotalSalary + employee.Salary > newDepartment.Budget)
                    throw new Exception("Insufficient budget in the new department for this employee.");
            }

            _employeeRepository.UpdateEmployee(employee);
        }

        // Delete an employee
        public void DeleteEmployee(int employeeId)
        {
            // Validate that the employee exists before deletion
            var employee = _employeeRepository.GetEmployeeById(employeeId);
            if (employee == null)
                throw new Exception($"Employee with ID {employeeId} does not exist.");

            _employeeRepository.DeleteEmployee(employeeId);
        }


        public List<Employee> SearchEmployees(string searchTerm)
        {
            // If both searchTerm and departmentId are null or empty, throw an exception
            if (string.IsNullOrWhiteSpace(searchTerm) )
                throw new ArgumentException("At least one search criterion must be provided.");

            // Get all employees
            var employees = _employeeRepository.GetAllEmployees();

            // Perform filtering based on the search term and optional departmentId
            var filteredEmployees = employees.Where(e =>
                // Check if the employee matches the search term in name or position
                (e.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                 e.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                 e.Position.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                 (e.DepartmentID.ToString() == searchTerm)) // Include departmentId match
            ).ToList();

            return filteredEmployees;
        }



        // Search for employees by name, department, or position
        /* public IEnumerable<Employee> SearchEmployees(string name = null, int? departmentId = null, string position = null)
         {
             if (string.IsNullOrWhiteSpace(name) && departmentId == null && string.IsNullOrWhiteSpace(position))
                 throw new Exception("At least one search criterion (name, department, or position) must be provided.");

             return _employeeRepository.SearchEmployees(name, departmentId, position);
         }*/
    }
}
