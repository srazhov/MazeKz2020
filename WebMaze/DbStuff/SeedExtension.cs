using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using WebMaze.DbStuff.Model;
using WebMaze.DbStuff.Model.UserAccount;
using WebMaze.DbStuff.Repository;
using WebMaze.Infrastructure.Enums;
using WebMaze.Services;

namespace WebMaze.DbStuff
{
    public static class SeedExtension
    {
        private const string AdminRoleName = "Admin";

        public static IHost Seed(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                AddIfNotExistRoles(scope);
                AddIfNotExistAdmins(scope);
                new TestDataSeeder(scope).SeedData();
            }

            return host;
        }

        private static void AddIfNotExistRoles(IServiceScope scope)
        {
            var roleRepository = scope.ServiceProvider.GetService<RoleRepository>();

            if (roleRepository == null)
            {
                throw new Exception("Cannot get RoleRepository from ServiceProvider.");
            }

            var roleNames = new List<string> { AdminRoleName, "Policeman", "Doctor" };

            foreach (var roleName in roleNames.Where(roleName => !roleRepository.RoleExists(roleName)))
            {
                var newRole = new Role() { Name = roleName };
                roleRepository.Save(newRole);
            }
        }

        private static void AddIfNotExistAdmins(IServiceScope scope)
        {
            var citizenUserRepository = scope.ServiceProvider.GetService<CitizenUserRepository>();
            var roleRepository = scope.ServiceProvider.GetService<RoleRepository>();

            if (citizenUserRepository == null || roleRepository == null)
            {
                throw new Exception("Cannot get services from ServiceProvider.");
            }

            var admins = new List<CitizenUser> {
                new CitizenUser
                {
                    Login = "Bill",
                    Password = "123456Qq",
                    AvatarUrl = "/image/avatar/bill.jpg",
                    Balance = 120000000000,
                    RegistrationDate = new DateTime(2020, 10, 1),
                    LastLoginDate = new DateTime(2020, 10, 1),
                    FirstName = "Bill",
                    LastName = "Gates",
                    Gender = Gender.Male,
                    Email = "BillGates@example.com",
                    PhoneNumber = "0000000000",
                    BirthDate = new DateTime(1955, 10, 28)
                },
                new CitizenUser
                {
                    Login = "Musk",
                    Password = "123",
                    AvatarUrl = "/image/avatar/musk.jpg",
                    Balance = 200000000000,
                    RegistrationDate = new DateTime(2020, 12, 15),
                    LastLoginDate = new DateTime(2020, 12, 15),
                    FirstName = "Elon",
                    LastName = "Musk",
                    Gender = Gender.Male,
                    Email = "ElonMusk@example.com",
                    PhoneNumber = "1111111111",
                    BirthDate = new DateTime(1971, 7, 28)
                },
                new CitizenUser
                {
                    Login = "Stroustrup",
                    Password = "123",
                    AvatarUrl = "/image/avatar/stroustrup.png",
                    Balance = 5000000,
                    RegistrationDate = new DateTime(2020, 11, 5),
                    LastLoginDate = new DateTime(2020, 11, 5),
                    FirstName = "Bjarne",
                    LastName = "Stroustrup",
                    Gender = Gender.Male,
                    Email = "BjarneStroustrup@example.com",
                    PhoneNumber = "2222222222",
                    BirthDate = new DateTime(1950, 12, 30)
                }, };

            foreach (var admin in admins.Where(a => !citizenUserRepository.UserExists(a.Login)))
            {
                var role = roleRepository.GetRoleByName(AdminRoleName);
                admin.Roles.Add(role);
                citizenUserRepository.Save(admin);
            }
        }
    }
}