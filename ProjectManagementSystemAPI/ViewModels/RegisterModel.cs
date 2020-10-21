using ProjectManagementSystemAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagementSystemAPI.ViewModels
{
    public class RegisterModel
    {

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username must have characters and numbers")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Password must have characters and numbers")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Confirm password must be same as password")]
        public string ConfirmPassword { get; set; }

        public static implicit operator User(RegisterModel model)
        {
            User user = new User();
            user.Id = 0;
            user.Username = model.Username;
            user.Password = model.Password;
            user.Email = model.Email;
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.Token = null; ;

            if (model.Role == "Developer")
            {
                user.Role = Roles.Developer;
            } else if (model.Role == "Admin")
            {
                user.Role = Roles.Admin;
            } else if(model.Role == "ProjectManager")
            {
                user.Role = Roles.ProjectManager;
            }

            return user;
        }


    }
}
