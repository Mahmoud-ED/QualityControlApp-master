﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QualityControlApp.Classes;
using QualityControlApp.Models.Entities;

namespace QualityControlApp.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder
                                   , IServiceProvider serviceProvider)
        {
            modelBuilder.Entity<SiteInfo>().HasData(
                  new SiteInfo()
                  {
                      Id = Guid.NewGuid(),
                      Name = "LACC",
                      Activity = "LACC site",
                      About = "",
                      Created = DateTime.Now
                  });

            modelBuilder.Entity<Contact>().HasData(
              new Contact()
              {
                  Id = Guid.NewGuid(),
                  Email = "libyanlacc@gmail.com",
                  Phone = "+218913832221",
                  Facebook = "- Facebook",
                  Twitter = "- Twitter",
                  Instagram = "- Instagram",
                  Created = DateTime.Now

              });
            modelBuilder.Entity<SiteState>().HasData(
             new SiteState()
             {
                 Id = Guid.NewGuid(),
                 State = true,
                 ClosingMessage = "The site is temporarily closed for development",
                 Created = DateTime.Now

             });

            //--------------------Roles-----------------------------
            var progRoleId = Guid.NewGuid().ToString();

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = progRoleId,
                    Name = "Prog",
                    NormalizedName = "Prog".ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            );

            //-------------------Programmer-----------------------------

            string programmerId = Guid.NewGuid().ToString();
            //string programmerName = "REEMA2783@GMAIL.COM";

            var programmerSettings = serviceProvider.
                             GetRequiredService<IOptions<ProgrammerSettings>>().Value;
            string programmerName = programmerSettings.ProgrammerName;

            string programmerPassword = "p111";

            var Programmer = new ApplicationUser
            {
                Id = programmerId,
                UserName = programmerName,
                NormalizedUserName = programmerName.ToUpper(),
                Email = programmerName,
                NormalizedEmail = programmerName.ToUpper(),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                LockoutEnabled = true,
                //SecurityStamp = // سيتم تخصيصها تلقائيًا  
                //ConcurrencyStamp = // سيتم تخصيصها تلقائيًا  
                CreatedDate = DateTime.UtcNow,
                Age = 0
            };

            var hasher = new PasswordHasher<ApplicationUser>();
            Programmer.PasswordHash =
                hasher.HashPassword(Programmer, programmerPassword);

            modelBuilder.Entity<ApplicationUser>().HasData(Programmer);


            //-------------Add User to Role--------------------
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = progRoleId,
                UserId = programmerId
            });
        }

    }
}

