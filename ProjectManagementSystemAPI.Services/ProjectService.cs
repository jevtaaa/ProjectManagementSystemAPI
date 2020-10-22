using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectManagementSystemAPI.Data;
using ProjectManagementSystemAPI.Data.Helpers;
using ProjectManagementSystemAPI.Data.Models;
using ProjectManagementSystemAPI.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystemAPI.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationContext _context;

        public ProjectService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<Project> Create(Project project)
        {
            try
            {
                EntityEntry<Project> createdProject = await _context.Set<Project>().AddAsync(project);
                await _context.SaveChangesAsync();

                return createdProject.Entity;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Project>> GetAll()
        {
            var projects = await _context.Projects
                                        .Include(project => project.ProjectManager)
                                        .Include(project => project.Tasks)
                                        .ToListAsync();

            if (projects == null)
            {
                return null;
            }

            foreach(Project p in projects)
            {
                p.ProjectManager = p.ProjectManager.WithoutPassword();
            }
            return projects;
        }

        public Task<IEnumerable<Project>> GetAllOfProjectManager(int id)
        {
            throw new NotImplementedException();
        }

        public Project GetById(int id)
        {
            var project = _context.Projects
                                        .Include(project => project.ProjectManager)
                                        .Include(project => project.Tasks)
                                        .FirstOrDefault(project => project.Id == id);

            if(project == null || project.ProjectManager == null)
            {
                return null;
            }
            project.ProjectManager = project.ProjectManager.WithoutPassword();
            return project;
        }

        public Task<bool> Update(int id, Project project)
        {
            throw new NotImplementedException();
        }
    }
}
