using Day9.Models;

namespace Day9.Repositories
{
    public interface IEmployeeRepository
    {
        Employee? GetById(int id);
    }
}