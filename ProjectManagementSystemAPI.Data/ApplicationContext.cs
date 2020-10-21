using System;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemAPI.Data.Models;

namespace ProjectManagementSystemAPI.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {

        }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
                
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
    }
}
