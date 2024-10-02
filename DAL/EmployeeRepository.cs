using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using Dapper;
using BOL;

namespace DAL
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Retrieve all employees
        public IEnumerable<Employee> GetAllEmployees()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Employee>("SELECT * FROM Employee");
            }
        }

        // Retrieve an employee by ID
        public Employee GetEmployeeById(int employeeId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    return connection.QueryFirstOrDefault<Employee>(
                        "SELECT * FROM Employee WHERE EmployeeID = @EmployeeID",
                        new { EmployeeID = employeeId });
                }
            }
            catch (SqlException ex)
            {
                // Log the exception
                throw new Exception($"An error occurred while retrieving employee with ID {employeeId}.", ex);
            }
        }

        // Add a new employee
        public void AddEmployee(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        INSERT INTO Employee (FirstName, LastName, Position, Salary, DepartmentID)
                        VALUES (@FirstName, @LastName, @Position, @Salary, @DepartmentID)";

                    // Ensure salary and departmentID are properly validated before inserting
                    if (employee.Salary <= 0 || employee.DepartmentID <= 0)
                    {
                        throw new ArgumentException("Salary and DepartmentID must be greater than zero.");
                    }

                    connection.Execute(sql, employee);
                }
            }
            catch (SqlException ex)
            {
                // Log the exception
                throw new Exception("An error occurred while adding the employee.", ex);
            }
        }

        // Update an existing employee
        public void UpdateEmployee(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Check if employee exists
                    var existingEmployee = connection.QuerySingleOrDefault<Employee>(
                        "SELECT * FROM Employee WHERE EmployeeID = @EmployeeID", new { employee.EmployeeID });

                    if (existingEmployee == null)
                    {
                        throw new Exception($"Employee with ID {employee.EmployeeID} does not exist.");
                    }

                    // Validate Salary and DepartmentID
                    if (employee.Salary <= 0 || employee.DepartmentID <= 0)
                    {
                        throw new ArgumentException("Salary and DepartmentID must be greater than zero.");
                    }

                    // Check the current total salary of the department
                    var newDepartmentTotalSalary = connection.QuerySingleOrDefault<decimal>(
                        "SELECT SUM(Salary) FROM Employee WHERE DepartmentID = @DepartmentID",
                        new { DepartmentID = employee.DepartmentID });

                    // Validate the budget
                    var departmentBudget = connection.QuerySingleOrDefault<decimal>(
                        "SELECT Budget FROM Department WHERE DepartmentID = @DepartmentID",
                        new { DepartmentID = employee.DepartmentID });

                    if (newDepartmentTotalSalary + employee.Salary > departmentBudget)
                    {
                        throw new Exception("Insufficient budget in the department for this employee's salary.");
                    }

                    // Proceed with the update
                    string sql = @"
                UPDATE Employee 
                SET FirstName = @FirstName, LastName = @LastName, Position = @Position, Salary = @Salary, DepartmentID = @DepartmentID
                WHERE EmployeeID = @EmployeeID";

                    connection.Execute(sql, employee);
                }
            }
            catch (SqlException ex)
            {
                // Log the exception
                throw new Exception($"An error occurred while updating employee with ID {employee.EmployeeID}.", ex);
            }
        }


        // Delete an employee
        public void DeleteEmployee(int employeeId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM Employee WHERE EmployeeID = @EmployeeID";
                    connection.Execute(sql, new { EmployeeID = employeeId });
                }
            }
            catch (SqlException ex)
            {
                // Log the exception
                throw new Exception($"An error occurred while deleting employee with ID {employeeId}.", ex);
            }
        }

        // Search employees by name, department, or position
        public IEnumerable<Employee> SearchEmployees(string name = null, int? departmentId = null, string position = null)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Base SQL query
                    var sql = "SELECT * FROM Employee WHERE 1=1";

                    // Parameters object to hold dynamic parameters
                    var parameters = new DynamicParameters();

                    // Add conditions based on the provided parameters
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        sql += " AND (FirstName LIKE @Name OR LastName LIKE @Name)";
                        parameters.Add("Name", $"%{name}%");
                    }

                    if (departmentId.HasValue)
                    {
                        sql += " AND DepartmentID = @DepartmentID";
                        parameters.Add("DepartmentID", departmentId.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(position))
                    {
                        sql += " AND Position LIKE @Position";
                        parameters.Add("Position", $"%{position}%");
                    }

                    // Execute the query with the built SQL and parameters
                    return connection.Query<Employee>(sql, parameters);
                }
            }
            catch (SqlException ex)
            {
                // Log the exception
                throw new Exception("An error occurred while searching for employees.", ex);
            }
        }
    }
}
