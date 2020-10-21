using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystemAPI.Data.Helpers;
using ProjectManagementSystemAPI.Data.Models;
using ProjectManagementSystemAPI.Data.Services;
using ProjectManagementSystemAPI.ViewModels;

namespace ProjectManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPasswordService _passwordService;

        public AccountController(IUserService userService, IPasswordService passwordService)
        {
            _userService = userService;
            _passwordService = passwordService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var hashedPassword = _passwordService.ComputePasswordHash(model.Password);

            var user = _userService.Login(model.Username, hashedPassword);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect!" });

            return Ok(user);
        }

        [HttpPost("register")]

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (_userService.DoesUserExist(model.Username))
                return BadRequest(new
                {
                    message = "Username already exists!"
                });

            var user = (User)model;
            user.Password = _passwordService.ComputePasswordHash(model.Password);

            var savedUser = _userService.Create(user);

            if (savedUser.Result == null)
                return BadRequest();

            return Ok(savedUser.Result);
        }

        [HttpGet("all")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetAllUsers()
        {
            // var id = User.Claims.First(claim => claim.Type == ClaimTypes.Name.ToString()).Value;
            // var role = User.Claims.First(claim => claim.Type == ClaimTypes.Role.ToString()).Value;
            // Debug.WriteLine(id+"    " + role);
            var users = await _userService.GetAll();

            return Ok(users);
        }

        [HttpGet("with_role")]
        [Authorize(Roles = Roles.Admin + "," + Roles.ProjectManager)]
        public async Task<IActionResult> GetWithRole([FromBody] RoleModel role)
        {
            if(role.role != Roles.Admin && role.role != Roles.Developer && role.role != Roles.ProjectManager)
            {
                return BadRequest(new {
                    message = "Role " + role.role + " doesnt exits!"
                });
            }
            var users = await _userService.GetUserWithRole(role.role);
            return Ok(users);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (await _userService.Delete(id))
            {
                return Ok(new
                {
                    message = "User account successfully deleted!"
                });
            }
            return BadRequest(new
            {
                message = "User account unsuccessfully deleted!"
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserModel model)
        {
            var user = (User)model;

            if (await _userService.Update(id, user))
            {
                var updated = _userService.GetById(id);
                if (updated == null)
                {
                    return BadRequest();
                }
                return Ok(updated.WithoutPassword());    
            }
            return BadRequest(new
            {
                message = "User not updated!"
            });
        }
    }
}
