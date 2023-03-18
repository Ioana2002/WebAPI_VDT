using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAPI_VDT.Models;

namespace WebAPI_VDT.Context
{
    public class ApplicationContext : IdentityDbContext
    {
        public ApplicationContext(DbContextOptions options): base(options)
        {
            
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Profile> Profile { get; set; }
        public DbSet<ProfilePicture> ProfilePicture { get; set; }

    }
}
