using ProjectManagementSystemAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagementSystemAPI.ViewModels
{
    public class ProjectModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public int ProjectManagerId { get; set; }

        public static implicit operator Project(ProjectModel model)
        {
            return new Project
            {
                Id = 0,
                Name = model.Name,
                ProjectManager = null
            };
        }
    }
}
