using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystemAPI.Data.Models;
using ProjectManagementSystemAPI.Data.Services;
using ProjectManagementSystemAPI.ViewModels;

namespace ProjectManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly ITaskService _taskService;

        public ProjectController(IProjectService projectService, IUserService userService, ITaskService taskService)
        {
            _projectService = projectService;
            _userService = userService;
            _taskService = taskService;
        }

        [HttpPost("create")]
        [Authorize(Roles = Roles.Admin +"," + Roles.ProjectManager)]
        public async Task<IActionResult> CreateProject([FromBody] ProjectModel model)
        {
            var projectManagerId = User.Claims.First(claim => claim.Type == ClaimTypes.Name.ToString()).Value;
            var role = User.Claims.First(claim => claim.Type == ClaimTypes.Role.ToString()).Value;

            var project = (Project)model;

            var projectManager = _userService.GetById(model.ProjectManagerId);

            if (role == Roles.ProjectManager)
            {
                projectManager = _userService.GetById(Convert.ToInt32(projectManagerId));
            }
            
            if (projectManager == null || projectManager.Role != Roles.ProjectManager)
                return NotFound(new
                {
                    message = "Cannot find project manager!"
                });

            project.ProjectManager = projectManager;

            var createdProject = await _projectService.Create(project);
            if (createdProject == null)
                return BadRequest(new
                {
                    message = "Problem with creating project!"
                });

            return Ok(_projectService.GetById(createdProject.Id));
        }

        [HttpGet("all")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ProjectManager)]
        public async Task<IActionResult> GetAllProjects()
        {
            var id = User.Claims.First(claim => claim.Type == ClaimTypes.Name.ToString()).Value;
            var role = User.Claims.First(claim => claim.Type == ClaimTypes.Role.ToString()).Value;

            if (role == Roles.ProjectManager)
            {
                var projectsPm = await _projectService.GetAllOfProjectManager(Convert.ToInt32(id));
                return Ok(projectsPm);
            } else
            {
                var projects = await _projectService.GetAll();
                return Ok(projects);
            }        
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteProject(int id)
        {
            if (await _projectService.Delete(id))
                return Ok();
            return BadRequest(new
            {
                message = "Can't delete project"
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ProjectManager)]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectModel model)
        {
            var projectManagerId = User.Claims.First(claim => claim.Type == ClaimTypes.Name.ToString()).Value;
            var role = User.Claims.First(claim => claim.Type == ClaimTypes.Role.ToString()).Value;

            var project = (Project)model;
            var projectManager = _userService.GetById(model.ProjectManagerId);

            if (role == Roles.ProjectManager)
            {
                projectManager = _userService.GetById(Convert.ToInt32(projectManagerId));
            }

            if (projectManager == null || projectManager.Role != Roles.ProjectManager)
                return NotFound(new
                {
                    message = "Cannot find project manager!"
                });

            project.ProjectManager = projectManager;

            if (await _projectService.Update(id, project))
            {
                var updated = _projectService.GetById(id);
                if (updated == null)
                {
                    return BadRequest();
                }
                return Ok(updated);
            }
            return BadRequest(new
            {
                message = "Project not updated!"
            });
        }

        [HttpPost("{id}/addtask")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ProjectManager)]
        public async Task<IActionResult> AddNewTask(int id, [FromBody] TaskModel model)
        {
            var task = (Data.Models.Task)model;
            Debug.WriteLine(task.Deadline + "" + task.Developer + "" + task.Description + "" + task.Progress);
            var dev = _userService.GetById(model.DeveloperId);
            if (dev != null && dev.Role != Roles.Developer)
            {
                return BadRequest(new
                {
                    message = "You must send user with DEVELOPER role"
                });
            }

            if(dev!=null && _taskService.DeveloperTasks(dev) >= 3)
            {
                return BadRequest(new
                {
                    message = "Developer already have maximum of 3 tasks!"
                });
            }
            
            task.Developer = dev;

            var createdTask = await _taskService.Create(id, task);

            if (createdTask == null)
                return BadRequest(new
                {
                    message = "Can't add task!"
                });

            return Ok(createdTask);
        }

        [HttpPut("{idProject}/tasks/{idTask}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ProjectManager + "," + Roles.Developer)]
        public async Task<IActionResult> UpdateTask(int idProject, [FromBody] TaskModel model, int idTask)
        {
            var role = User.Claims.First(claim => claim.Type == ClaimTypes.Role.ToString()).Value;
            var task = (Data.Models.Task)model;

            var dev = _userService.GetById(model.DeveloperId);
            if (dev != null && dev.Role != Roles.Developer)
            {
                return BadRequest(new
                {
                    message = "You must send user with DEVELOPER role"
                });
            }

            task.Developer = dev;

            var updatedTask = await _taskService.Update(idProject, task, idTask, role);

            if (updatedTask == null)
            {
                if(dev != null && _taskService.DeveloperTasks(task.Developer) >= 3)
                {
                    return BadRequest(new
                    {
                        message = "Developer already have 3 tasks!"
                    });
                }
                return BadRequest(new
                {
                    message = "Can't update task!"
                });
            }

            return Ok(updatedTask);

        }

        [HttpDelete("{idProject}/tasks/{idTask}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ProjectManager)]
        public async Task<IActionResult> DeleteTask(int idProject, int idTask)
        {
            if (await _taskService.Delete(idProject, idTask))
                return Ok();
            return BadRequest(new
            {
                message = "Can't delete task!"
            });
        }
    }
}
