using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.MVC.DAL.Models;

namespace Company.MVC.BLL.Interfaces
{
    public interface IDepartmentRepository : IGenericRepository<Department>
    {
        Task<List<Department>> GetByNameAsync(string name);

        //IEnumerable<Department> GetAll();
        //Department? Get (int id);
        //int Add (Department model);
        //int Update (Department model);
        //int Delete (Department model);
    }
}
