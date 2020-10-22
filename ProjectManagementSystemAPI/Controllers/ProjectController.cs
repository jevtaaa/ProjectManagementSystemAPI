﻿using System;
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

        public ProjectController(IProjectService projectService, IUserService userService)
        {
            _projectService = projectService;
            _userService = userService;
        }

        [HttpPost("create")]
        [Authorize(Roles = Roles.Admin +"," + Roles.ProjectManager)]
        public async Task<IActionResult> CreateProject([FromBody] ProjectModel model)
        {
            var id = User.Claims.First(claim => claim.Type == ClaimTypes.Name.ToString()).Value;
            var role = User.Claims.First(claim => claim.Type == ClaimTypes.Role.ToString()).Value;

            Debug.WriteLine(id);
            if (model == null)
            {
                return BadRequest(new
                {
                    message = "Arguments not sent"
                });
            }

            var project = (Project)model;

            var projectManager = _userService.GetById(model.ProjectManagerId);

            if (role == Roles.ProjectManager)
            {
                projectManager = _userService.GetById(Convert.ToInt32(id));
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
    }
}
