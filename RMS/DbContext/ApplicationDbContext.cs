using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RMS.Model;
using RMS.Models;

namespace RMS.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Student> Student { get; set; }
        public DbSet<Porter> Porter { get; set; }
        public DbSet<ExeatRecords> ExeatRecords { get; set; }


    }

}
