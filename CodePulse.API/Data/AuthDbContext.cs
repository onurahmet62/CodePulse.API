using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Exchange.WebServices.Data;

namespace CodePulse.API.Data
{
    public class AuthDbContext:IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) 
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var readerRoleId = "f275d27e-2a38-40f9-83e0-da92eb9438a3";
            var writerRoleId = "8de2c8a8-f7f9-4774-8eab-694779853c95";

            var roles = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Id = readerRoleId,
                    Name ="Reader",
                    NormalizedName = "Reader".ToUpper(),
                    ConcurrencyStamp = readerRoleId,
                },

                new IdentityRole()
                {
                    Id=writerRoleId,
                    Name= "Writer",
                    NormalizedName="Writer".ToUpper(),
                    ConcurrencyStamp= writerRoleId,
                }
            
            };

            builder.Entity<IdentityRole>().HasData(roles);


            var adminUserId = "8674da05-94c8-4fa6-af36-fa37d2f31a15";
            var admin = new IdentityUser()
            {
                Id= adminUserId,
                UserName= "admin@codepulse.com",
                Email= "admin@codepulse.com",
                NormalizedEmail= "admin@codepulse.com".ToLower(),
                NormalizedUserName = "admin@codepulse.com".ToUpper()
            };

            admin.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(admin, "Admin@123");

            builder.Entity<IdentityUser>().HasData(admin);

            var adminRoles = new List<IdentityUserRole<string>>() 
            {
                new ()
                {
                    UserId=adminUserId,
                    RoleId=readerRoleId,
                },
                 new ()
                {
                    UserId=adminUserId,
                    RoleId=writerRoleId,
                }
            };

            builder.Entity<IdentityUserRole<string>>().HasData(adminRoles);
        }

    }
}
