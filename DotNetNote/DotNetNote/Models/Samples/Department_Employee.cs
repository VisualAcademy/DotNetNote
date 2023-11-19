using System.Linq;

namespace DotNetNote.Models.Samples
{
    public class Department_Employee
    {
        // Empty
    }

    public class Department
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Employee> Type { get; set; }
    }

    public class Employee
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }
    }

    public interface IDepartmentDataSource
    {
        IQueryable<Employee> Employees { get; }

        IQueryable<Department> Departments { get; }
    }
}
