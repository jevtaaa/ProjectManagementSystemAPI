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
        Task<IEnumerable<Project>> GetAllOfUser(int id);
        Data.Models.Task GetById(int id);


        Task<Data.Models.Task> Create(int id, Data.Models.Task task);
        Task<bool> Delete(int idProject, int idTask);
        Task<Data.Models.Task> Update(int idProject, Data.Models.Task task, int idTask, string role);
        int DeveloperTasks(User dev);
    }
}
