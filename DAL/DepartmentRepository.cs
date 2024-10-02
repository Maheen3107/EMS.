using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Dapper;
using BOL;

namespace DAL
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly string _connectionString;

        public DepartmentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Department> GetAllDepartments()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<Department>("SELECT * FROM Department");
            }
        }

        public Department GetDepartmentById(int departmentId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<Department>(
                    "SELECT * FROM Department WHERE DepartmentID = @DepartmentID",
                    new { DepartmentID = departmentId });
            }
        }

        public void AddDepartment(Department department)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO Department (Name, Budget) VALUES (@Name, @Budget)";
                connection.Execute(sql, department);
            }
        }

        public void UpdateDepartment(Department department)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "UPDATE Department SET Name = @Name, Budget = @Budget WHERE DepartmentID = @DepartmentID";
                connection.Execute(sql, department);
            }
        }

        public void DeleteDepartment(int departmentId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM Department WHERE DepartmentID = @DepartmentID";
                connection.Execute(sql, new { DepartmentID = departmentId });
            }
        }

        public int GetEmployeeCountByDepartment(int departmentId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM Employee WHERE DepartmentID = @DepartmentID";
                return connection.ExecuteScalar<int>(sql, new { DepartmentID = departmentId });
            }
        }

        public decimal GetTotalSalaryByDepartment(int departmentId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT SUM(Salary) FROM Employee WHERE DepartmentID = @DepartmentID";
                return connection.ExecuteScalar<decimal>(sql, new { DepartmentID = departmentId });
            }
        }
        public IEnumerable<DepartmentStatistics> GetDepartmentStatistics()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"
            SELECT 
                d.DepartmentID, 
                d.Budget, 
                d.DepartmentName, 
                COUNT(e.EmployeeID) AS EmployeeCount, 
                SUM(e.Salary) AS TotalSalary
            FROM 
                Department d
            LEFT JOIN 
                Employee e ON d.DepartmentID = e.DepartmentID
            GROUP BY 
                d.DepartmentID, d.Budget, d.DepartmentName";

                return connection.Query<DepartmentStatistics>(sql);
            }
        }
        public void AddEmployeeToDepartment(int departmentId, Employee employee)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = @"
                    INSERT INTO Employee (FirstName, LastName, Position, Salary, DepartmentID)
                    VALUES (@FirstName, @LastName, @Position, @Salary, @DepartmentID)";

                // Add employee and assign to the department
                connection.Execute(sql, new
                {
                    employee.FirstName,
                    employee.LastName,
                    employee.Position,
                    employee.Salary,
                    DepartmentID = departmentId
                });
            }
        }
    }
}
