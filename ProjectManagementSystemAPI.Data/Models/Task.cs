using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagementSystemAPI.Data.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public double Progress { get; set; }
        public DateTime Deadline { get; set; }
        public string Description { get; set; }
        public User Developer { get; set; }
    }
}
