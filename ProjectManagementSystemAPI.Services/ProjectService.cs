using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        public async Task<bool> Delete(int id)
        {
            try
            {
                var forDelete = await _context.Set<Project>().FirstOrDefaultAsync(x => x.Id == id);
                if (forDelete == null)
                    return false;
                _context.Set<Project>().Remove(forDelete);
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Project>> GetAll()
        {
            var projects = await _context.Projects
                                        .Include(project => project.ProjectManager)
                                        .Include(project => project.Tasks)
                                            .ThenInclude(t => t.Developer)
                                        .ToListAsync();

            if (projects == null)
            {
                return null;
            }

            foreach(Project p in projects)
            {
                p.ProjectManager = p.ProjectManager.WithoutPassword();
                foreach(Data.Models.Task t in p.Tasks)
                {
                    t.Developer = t.Developer.WithoutPassword();
                }
            }
            return projects;
        }

        public async Task<IEnumerable<Project>> GetAllOfProjectManager(int id)
        {
            var projects = await _context.Projects
                                            .Include(project => project.ProjectManager)
                                            .Include(project => project.Tasks)
                                                .ThenInclude(t => t.Developer)
                                            .Where(project => project.ProjectManager.Id == id)
                                            .ToListAsync();

            if (projects == null)
            {
                return null;
            }

            foreach (Project p in projects)
            {
                p.ProjectManager = p.ProjectManager.WithoutPassword();
                foreach (Data.Models.Task t in p.Tasks)
                {
                    t.Developer = t.Developer.WithoutPassword();
                }
            }
            return projects;
        }

        public Project GetById(int id)
        {
            var project = _context.Projects
                                        .Include(project => project.ProjectManager)
                                        .Include(project => project.Tasks)
                                            .ThenInclude(t => t.Developer)
                                        .FirstOrDefault(project => project.Id == id);

            if(project == null || project.ProjectManager == null)
            {
                return null;
            }
            project.ProjectManager = project.ProjectManager.WithoutPassword();
            return project;
        }

        public async Task<bool> Update(int id, Project project)
        {
            var forUpdate = _context.Projects
                                        .FirstOrDefault(project => project.Id == id);

            if (forUpdate == null)
            {
                return false;
            }

            UpdateProject(ref forUpdate, project);

            try
            {
                _context.Update(forUpdate);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        // Help for update projects
        private void UpdateProject(ref Project forUpdate, Project project)
        {
            forUpdate.Name = project.Name;
            forUpdate.ProjectManager = project.ProjectManager;
        }
    }
}
