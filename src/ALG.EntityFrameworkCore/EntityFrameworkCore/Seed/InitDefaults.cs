using ALG.Core.Services;
using ALG.Core.Users;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ALG.EntityFrameworkCore.EntityFrameworkCore.Seed
{
    public static class InitDefaults
    {
        public static List<User> GetInitialUsers()
        {
            var users = new List<User>();

            var passwordHasher = new PasswordHasher<User>();

            var user = new User()
            {
                Id = Guid.NewGuid(),
                Name = "John Dow",
                Email = "john.dow@gmail.com",
                Role = "User"
            };
            user.Password = passwordHasher.HashPassword(user, "111");
            users.Add(user);

            user = new User()
            {
                Id = Guid.NewGuid(),
                Name = "Adell W. Sansone",
                Email = "adell.sansone@gmail.com",
                Role = "User"
            };
            user.Password = passwordHasher.HashPassword(user, "222");
            users.Add(user);

            user = new User()
            {
                Id = Guid.NewGuid(),
                Name = "Gordon Brundage",
                Email = "gordon.brundage@gmail.com",
                Role = "Admin"
            };
            user.Password = passwordHasher.HashPassword(user, "333");
            users.Add(user);

            return users;
        }

        public static List<Service> GetInitialServices()
        {
            return new List<Service>
            {
                new Service
                {
                    Id = Guid.NewGuid(),
                    Name = "Sitecostructor.io",
                    Description = "Description Sitecostructor.io",
                    Promocode = "itpromocode"
                },
                new Service
                {
                    Id = Guid.NewGuid(),
                    Name = "Appvision.com",
                    Description = "Description Appvision.com",
                    Promocode = "itpromocode"
                },
                new Service
                {
                    Id = Guid.NewGuid(),
                    Name = "Analytics.com",
                    Description = "Description Analytics.com",
                    Promocode = "itpromocode"
                },
                new Service
                {
                    Id = Guid.NewGuid(),
                    Name = "Logotype",
                    Description = "Description Logotype",
                    Promocode = "itpromocode"
                },
                new Service
                {
                    Id = Guid.NewGuid(),
                    Name = "Google.com",
                    Description = "Description Google.com",
                    Promocode = "itpromocode"
                },
                new Service
                {
                    Id = Guid.NewGuid(),
                    Name = "Microsoft.com",
                    Description = "Description Microsoft.com",
                    Promocode = "itpromocode"
                },
                new Service
                {
                    Id = Guid.NewGuid(),
                    Name = "Amazon.com",
                    Description = "Description Amazon.com",
                    Promocode = "itpromocode"
                }
            };
        }
    }
}
