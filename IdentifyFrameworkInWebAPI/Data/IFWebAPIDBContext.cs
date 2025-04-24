using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IdentifyFrameworkInWebAPI.Data
{
    public class IFWebAPIDBContext : IdentityDbContext<IdentityUser>
    {

        public IFWebAPIDBContext(DbContextOptions<IFWebAPIDBContext> options) :base(options)
        {
            
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedRoles(builder);
        }


        //seeder for role
        private static void SeedRoles(ModelBuilder builder) { 
        
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Name = "Admin",
                    ConcurrencyStamp ="1",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Name = "Student",
                    ConcurrencyStamp = "2",
                    NormalizedName = "STUDENT"
                },
                   new IdentityRole
                   {
                       Name = "Instructor",
                       ConcurrencyStamp = "3",
                       NormalizedName = "INSTRUCTOR"
                   }
            );
        }
    }
    
}
