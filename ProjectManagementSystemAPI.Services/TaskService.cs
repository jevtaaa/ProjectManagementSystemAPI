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
            catch(Exception)
            {
                return null;
            }

            if (task.Developer != null)
            {
                task.Developer = task.Developer.WithoutPassword();
            }
            return task;
        }

        public async Task<Data.Models.Task> Update(int idProject, Data.Models.Task task, int idTask, string role)
        {
            try
            {
                var project = await _context.Projects
                                    .Include(p => p.Tasks)
                                    .SingleOrDefaultAsync(p => p.Id == idProject);

                var tasks = project.Tasks.ToList();
                var taskForUpdate = tasks.SingleOrDefault(t => t.Id == idTask);

                if(taskForUpdate == null)
                {
                    Debug.WriteLine("Wrong task for project!");
                    return null;
                }

                if(task.Developer != null && taskForUpdate.Developer != task.Developer)
                {
                    if (DeveloperTasks(task.Developer) >= 3)
                    {
                        Debug.WriteLine("Developer already have 3 tasks!");
                        return null;
                    }
                }

                UpdateTask(ref taskForUpdate, task, role);
                project.Tasks = tasks;
                _context.Set<Project>().Update(project);
                await _context.SaveChangesAsync();

                task.Id = idTask;
                if(task.Developer != null)
                {
                    task.Developer = task.Developer.WithoutPassword();
                }
                return task;
            }
            catch(Exception)
            {
                return null;
            }
        }   

        public async Task<bool> Delete(int idProject, int idTask)
        {
            try
            {
                var project = await _context.Projects
                                    .Include(p => p.Tasks)
                                    .SingleOrDefaultAsync(p => p.Id == idProject);

                var tasks = project.Tasks.ToList();
                var taskForDelete = tasks.SingleOrDefault(t => t.Id == idTask);

                if (taskForDelete == null)
                {
                    return false;
                }

                // If we want to keep task in DB we can use code bellow. This code will free task of project

                /*tasks.Remove(taskForDelete);
                project.Tasks = tasks;

                _context.Set<Project>().Update(project);
                await _context.SaveChangesAsync();*/

                _context.Tasks.Remove(taskForDelete);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
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

        // Helper for update task
        private void UpdateTask(ref Data.Models.Task taskForUpdate, Data.Models.Task task, string role)
        {
            if(role == Roles.Developer)
            {
                taskForUpdate.Progress = task.Progress;
                taskForUpdate.Status = task.Status;
                taskForUpdate.Description = task.Description;
            }
            else
            {
                taskForUpdate.Progress = task.Progress;
                taskForUpdate.Status = task.Status;
                taskForUpdate.Description = task.Description;
                taskForUpdate.Deadline = task.Deadline;
                taskForUpdate.Developer = task.Developer;
            }  
        }

        public int DeveloperTasks(User dev)
        {
            return _context.Tasks.Count(task => task.Deadline > DateTime.Now && task.Developer == dev);
        }

    }
}
