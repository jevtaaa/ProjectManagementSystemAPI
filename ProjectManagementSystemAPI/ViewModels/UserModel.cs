using ProjectManagementSystemAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagementSystemAPI.ViewModels
{
    public class UserModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }

        public static implicit operator User(UserModel model)
        {
            User user = new User();
            user.Email = model.Email;
            user.Name = model.Name;
            user.Surname = model.Surname;

            if (model.Role == "Developer")
            {
                user.Role = Roles.Developer;
            }
            else if (model.Role == "Admin")
            {
                user.Role = Roles.Admin;
            }
            else if (model.Role == "Project Manager")
            {
                user.Role = Roles.ProjectManager;
            }

            return user;
        }
    }
}
