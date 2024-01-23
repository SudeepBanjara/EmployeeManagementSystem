using EmployeeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Data
{
    public class AppDbContext :DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options) : base(options) 
        { 
        }

        public DbSet<ApplicationUser> Users { get; set; }
    }
}
