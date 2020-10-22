using ProjectManagementSystemAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystemAPI.Data.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetAll();
        Task<IEnumerable<Project>> GetAllOfProjectManager(int id);
        Project GetById(int id);

        Task<Project> Create(Project project);
        Task<bool> Delete(int id);
        Task<bool> Update(int id, Project project);
    }
}
