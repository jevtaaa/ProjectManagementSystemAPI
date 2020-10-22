using ProjectManagementSystemAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystemAPI.Data.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<Data.Models.Task>> GetAll();
        Task<IEnumerable<Data.Models.Task>> GetAllOfUser(int id);
        Data.Models.Task GetById(int id);


        Task<Data.Models.Task> Create(int id, Data.Models.Task task);
        Task<bool> Delete(int id);
        Task<bool> Update(int id, Data.Models.Task task);
    }
}
