using Microsoft.EntityFrameworkCore;

using TaskManagementApi.Models;

namespace TaskManagementApi.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<TaskModel> tasks { get; set; }
    }
}
