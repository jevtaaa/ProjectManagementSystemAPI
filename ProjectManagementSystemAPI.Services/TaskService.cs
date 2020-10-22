using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemAPI.Data;
using ProjectManagementSystemAPI.Data.Helpers;
using ProjectManagementSystemAPI.Data.Models;
using ProjectManagementSystemAPI.Data.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystemAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationContext _context;

        public TaskService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<Data.Models.Task> Create(int id, Data.Models.Task task)
        {
            try
            {
                var project = await _context.Projects
                                    .Include(p => p.Tasks)
                                    .SingleOrDefaultAsync(p => p.Id == id);

                var tasks = project.Tasks.ToList();
                tasks.Add(task);
                project.Tasks = tasks;

                _context.Set<Project>().Update(project);
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }

            if (task.Developer != null)
            {
                task.Developer = task.Developer.WithoutPassword();
            }
            return task;
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Data.Models.Task>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Data.Models.Task>> GetAllOfUser(int id)
        {
            throw new NotImplementedException();
        }

        public Data.Models.Task GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(int id, Data.Models.Task task)
        {
            throw new NotImplementedException();
        }
    }
}
