using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystemAPI.Data.Models;
using ProjectManagementSystemAPI.Data.Services;

namespace ProjectManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly ITaskService _taskService;

        public TaskController(IProjectService projectService, IUserService userService, ITaskService taskService)
        {
            _projectService = projectService;
            _userService = userService;
            _taskService = taskService;
        }

        [HttpGet("all")]
        [Authorize(Roles = Roles.ProjectManager)]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _taskService.GetAll();
            return Ok(tasks);           
        }

        [HttpGet]
        [Authorize(Roles = Roles.Developer)]
        public async Task<IActionResult> GetDeveloperTasks()
        {
            var id = User.Claims.First(claim => claim.Type == ClaimTypes.Name.ToString()).Value;
            var tasks = await _taskService.GetAllOfUser(Convert.ToInt32(id));
            return Ok(tasks);
        }
    }
}
