using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagementSystemAPI.Data.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Task> Tasks { get; set; }
        public User ProjectManager { get; set; }
    }
}
