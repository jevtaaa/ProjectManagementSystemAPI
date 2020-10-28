using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectManagementSystemAPI.Controllers;
using ProjectManagementSystemAPI.Data;
using ProjectManagementSystemAPI.Data.Services;
using ProjectManagementSystemAPI.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace ProjectManagementSystemAPI.Test
{
    public class ProjectContorllerTests
    {
        //Using Mock
        private readonly Mock<IProjectService> _projectServiceMock = new Mock<IProjectService>();
        private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
        private readonly Mock<ITaskService> _taskServiceMock = new Mock<ITaskService>();

        //Not working as expected because ApplicationContext dont Inherite any Interface

        [Fact]
        public async Task DeleteReturnsBadRequest()
        {
            //Arrange
            var controller = new ProjectController(_projectServiceMock.Object, _userServiceMock.Object, _taskServiceMock.Object);
            int projectId = 98;

            //Act
            IActionResult actionResult = await controller.DeleteProject(projectId);

            //Assert
            Assert.IsType<OkResult>(actionResult);
        }

        [Fact]
        public async Task DeleteTaskReturnsBadRequest()
        {
            //Arrange
            var controller = new ProjectController(_projectServiceMock.Object, _userServiceMock.Object, _taskServiceMock.Object);

            //Act
            IActionResult actionResult = await controller.DeleteTask(850, 98);

            //Assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        
    }
}
