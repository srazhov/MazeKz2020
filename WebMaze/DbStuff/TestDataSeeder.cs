using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WebMaze.DbStuff.Model;
using WebMaze.DbStuff.Model.UserAccount;
using WebMaze.DbStuff.Repository;
using WebMaze.Infrastructure.Enums;
using TaskStatus = WebMaze.Infrastructure.Enums.TaskStatus;

namespace WebMaze.DbStuff
{
    public class TestDataSeeder
    {
        private CitizenUserRepository citizenUserRepository;

        private RoleRepository roleRepository;

        private FriendshipRepository friendshipRepository;

        private UserTaskRepository userTaskRepository;

        private MessageRepository messageRepository;

        public TestDataSeeder(IServiceScope scope)
        {
            citizenUserRepository = scope.ServiceProvider.GetService<CitizenUserRepository>();
            roleRepository = scope.ServiceProvider.GetService<RoleRepository>();
            friendshipRepository = scope.ServiceProvider.GetService<FriendshipRepository>();
            userTaskRepository = scope.ServiceProvider.GetService<UserTaskRepository>();
            messageRepository = scope.ServiceProvider.GetService<MessageRepository>();

            if (citizenUserRepository == null || roleRepository == null || friendshipRepository == null ||
                userTaskRepository == null || messageRepository == null)
            {
                throw new Exception("Cannot get services from ServiceProvider.");
            }
        }

        public void SeedData()
        {
            AddDoctors();
            AddPolicemen();
            AddRegularUsers();

            AddCertificates();
            AddFriendshipsAndMessages();
            AddUserTasksToBill();
        }

        private void AddDoctors()
        {
            var doctors = new List<CitizenUser>()
            {
                new CitizenUser
                {
                    Login = "Tsoi",
                    Password = "123",
                    AvatarUrl = "/image/avatar/tsoi.jpg",
                    Balance = 300000,
                    RegistrationDate = new DateTime(2020, 10, 28),
                    LastLoginDate = new DateTime(2020, 10, 28),
                    FirstName = "Alexey",
                    LastName = "Tsoi",
                    Gender = Gender.Male,
                    Email = "AlexeyTsoi@example.com",
                    PhoneNumber = "3333333333",
                    BirthDate = new DateTime(1977, 4, 2)
                }
            };
            AddIfNotExistUsersWithRole(doctors, roleName: "Doctor");
        }

        private void AddPolicemen()
        {
            var policemen = new List<CitizenUser>()
            {
                new CitizenUser
                {
                    Login = "Chuck",
                    Password = "123",
                    AvatarUrl = "/image/avatar/chuck.jpg",
                    Balance = 6000000,
                    RegistrationDate = new DateTime(2020, 12, 30),
                    LastLoginDate = new DateTime(2020, 12, 30),
                    FirstName = "Chuck",
                    LastName = "Norris",
                    Gender = Gender.Male,
                    Email = "ChuckNorris@example.com",
                    PhoneNumber = "4444444444",
                    BirthDate = new DateTime(1940, 3, 10)
                }
            };
            AddIfNotExistUsersWithRole(policemen, "Policeman");
        }

        private void AddRegularUsers()
        {
            var regularUsers = new List<CitizenUser>()
            {
                new CitizenUser
                {
                    Login = "Ivan",
                    Password = "123",
                    AvatarUrl = "/image/avatar/ivan.jpg",
                    Balance = 1000,
                    RegistrationDate = new DateTime(2021, 1, 12),
                    LastLoginDate = new DateTime(2021, 1, 12),
                    FirstName = "Ivan",
                    LastName = "Sokolov",
                    Gender = Gender.Male,
                    Email = "IvanSokolov@example.com",
                    PhoneNumber = "5555555555",
                    BirthDate = new DateTime(1980, 5, 17)
                },
                new CitizenUser
                {
                    Login = "Anastasia",
                    Password = "123",
                    AvatarUrl = "/image/avatar/anastasia.jpg",
                    Balance = 30000,
                    RegistrationDate = new DateTime(2021, 1, 15),
                    LastLoginDate = new DateTime(2021, 1, 15),
                    FirstName = "Anastasia",
                    LastName = "Kuznecova",
                    Gender = Gender.Female,
                    Email = "AnastasiaKuznecova@example.com",
                    PhoneNumber = "66666666",
                    BirthDate = new DateTime(1990, 11, 22)
                },
                new CitizenUser
                {
                    Login = "Arnold",
                    Password = "123",
                    AvatarUrl = "/image/avatar/arnold.png",
                    Balance = 9000000,
                    RegistrationDate = new DateTime(2021, 1, 16),
                    LastLoginDate = new DateTime(2021, 1, 16),
                    FirstName = "Arnold",
                    LastName = "Schwarzenegger",
                    Gender = Gender.Male,
                    Email = "ArnoldSchwarzenegger@example.com",
                    PhoneNumber = "77777777",
                    BirthDate = new DateTime(1947, 7, 30)
                },
                new CitizenUser
                {
                    Login = "Aigerim",
                    Password = "123",
                    AvatarUrl = "/image/avatar/aigerim.jpg",
                    Balance = 8000,
                    RegistrationDate = new DateTime(2021, 1, 17),
                    LastLoginDate = new DateTime(2021, 1, 17),
                    FirstName = "Aigerim",
                    LastName = "Alieva",
                    Gender = Gender.Female,
                    Email = "AigerimAlieva@example.com",
                    PhoneNumber = "8888888888",
                    BirthDate = new DateTime(1983, 8, 3)
                },
                new CitizenUser
                {
                    Login = "Dias",
                    Password = "123",
                    AvatarUrl = "/image/avatar/dias.png",
                    Balance = 15000,
                    RegistrationDate = new DateTime(2021, 1, 19),
                    LastLoginDate = new DateTime(2021, 1, 19),
                    FirstName = "Dias",
                    LastName = "Karimov",
                    Gender = Gender.Male,
                    Email = "DiasKarimov@example.com",
                    PhoneNumber = "9999999999",
                    BirthDate = new DateTime(2005, 10, 5)
                }
            };

            AddIfNotExistUsersWithRole(regularUsers);
        }

        private void AddIfNotExistUsersWithRole(List<CitizenUser> users, string roleName = null)
        {
            foreach (var user in users.Where(u => !citizenUserRepository.UserExists(u.Login)))
            {
                if (roleName != null)
                {
                    var role = roleRepository.GetRoleByName(roleName);
                    user.Roles.Add(role);
                }

                citizenUserRepository.Save(user);
            }
        }

        private void AddCertificates()
        {
            var allCitizens = citizenUserRepository.GetUsersAsQueryable();

            // Ensure that all citizens have a birth certificate.
            AddIfNotExistCertificateToCitizens(allCitizens, "Birth Certificate");

            // Ensure that 5 citizens have a diploma.
            var citizenLoginsWithDiploma = new List<string> { "Bill", "Musk", "Stroustrup", "Tsoi", "Chuck" };
            var citizenWithDiploma = citizenUserRepository.GetUsersByLogins(citizenLoginsWithDiploma);
            AddIfNotExistCertificateToCitizens(citizenWithDiploma, "Diploma of Higher Education");

            // Ensure that citizens have a policeman certificate.
            var citizenLoginsWithPoliceCertificate = new List<string> { "Chuck" };
            var policemen = citizenUserRepository.GetUsersByLogins(citizenLoginsWithPoliceCertificate);
            AddIfNotExistCertificateToCitizens(policemen, "Policeman Certificate");

            // Ensure that citizens have a doctor certificate.
            var citizenLoginsWithDoctorCertificate = new List<string> { "Tsoi" };
            var doctors = citizenUserRepository.GetUsersByLogins(citizenLoginsWithDoctorCertificate);
            AddIfNotExistCertificateToCitizens(doctors, "Doctor Certificate");
        }

        private void AddIfNotExistCertificateToCitizens(IQueryable<CitizenUser> citizens, string certificateName)
        {
            foreach (var citizen in citizens.Where(user => user.Certificates.All(certificate => certificate.Name != certificateName)).ToList())
            {
                var certificate = GenerateCertificate(certificateName, citizen);
                citizen.Certificates.Add(certificate);
                citizenUserRepository.Save(citizen);
            }
        }

        private Certificate GenerateCertificate(string certificateName, CitizenUser owner)
        {
            var certificate = new Certificate
            {
                Name = certificateName,
                Owner = owner
            };

            switch (certificateName)
            {
                case "Diploma of Higher Education":
                    certificate.Description =
                        "The document certifies that the person completed a course of study in a university";
                    certificate.IssuedBy = "University";
                    certificate.IssueDate = owner.BirthDate + TimeSpan.FromDays(22 * 365);
                    certificate.ExpiryDate = DateTime.MaxValue;
                    break;
                case "Birth Certificate":
                    certificate.Description = "The certificate documents the birth of the person";
                    certificate.IssuedBy = "Hospital";
                    certificate.IssueDate = owner.BirthDate;
                    certificate.ExpiryDate = DateTime.MaxValue;
                    break;
                case "Policeman Certificate":
                    certificate.Description = "The document assure qualification to work as a policeman";
                    certificate.IssuedBy = "Police";
                    certificate.IssueDate = new DateTime(2021, 1, 28);
                    certificate.ExpiryDate = new DateTime(2022, 1, 28);
                    break;
                case "Doctor Certificate":
                    certificate.Description = "The document assure qualification to work as a doctor";
                    certificate.IssuedBy = "Health Department";
                    certificate.IssueDate = new DateTime(2020, 5, 3);
                    certificate.ExpiryDate = new DateTime(2021, 5, 3);
                    break;
            }

            return certificate;
        }

        private void AddFriendshipsAndMessages()
        {
            var friendLogins = new List<string> { "Bill", "Musk", "Stroustrup", "Chuck", "Ivan", "Anastasia", "Arnold" };
            var friends = citizenUserRepository.GetUsersByLogins(friendLogins).ToList();

            var friendships = new List<Friendship>()
            {
                new Friendship
                {
                    RequestDate = DateTime.Now-TimeSpan.FromDays(30),
                    AcceptanceDate = DateTime.Now-TimeSpan.FromDays(29),
                    FriendshipStatus = FriendshipStatus.Accepted,
                    Requester = friends[0],
                    Requested = friends[1]
                },
                new Friendship
                {
                    RequestDate = DateTime.Now-TimeSpan.FromDays(20),
                    AcceptanceDate = DateTime.Now-TimeSpan.FromDays(19),
                    FriendshipStatus = FriendshipStatus.Accepted,
                    Requester = friends[0],
                    Requested = friends[2]
                },
                new Friendship
                {
                    RequestDate = DateTime.Now-TimeSpan.FromDays(10),
                    AcceptanceDate = DateTime.Now-TimeSpan.FromDays(9),
                    FriendshipStatus = FriendshipStatus.Accepted,
                    Requester = friends[0],
                    Requested = friends[3]
                },
                new Friendship
                {
                    RequestDate = DateTime.Now-TimeSpan.FromDays(2),
                    AcceptanceDate = DateTime.Now-TimeSpan.FromDays(1),
                    FriendshipStatus = FriendshipStatus.Accepted,
                    Requester = friends[0],
                    Requested = friends[6]
                },
                new Friendship
                {
                    RequestDate = DateTime.Now-TimeSpan.FromDays(1),
                    FriendshipStatus = FriendshipStatus.Pending,
                    Requester = friends[4],
                    Requested = friends[0]
                },
                new Friendship
                {
                    RequestDate = DateTime.Now,
                    FriendshipStatus = FriendshipStatus.Pending,
                    Requester = friends[5],
                    Requested = friends[0]
                }
            };

            var notExistFriendships = friendships.Where(f =>
                !friendshipRepository.FriendshipBetweenUsersExists(f.Requester.Login, f.Requested.Login));

            foreach (var friendship in notExistFriendships)
            {
                friendshipRepository.Save(friendship);
            }

            var billMessages = new List<Message>()
            {
                new Message
                {
                    Date = DateTime.Now - TimeSpan.FromHours(1),
                    Text = "Hi, how are you doing Bill?",
                    Sender = friends[1],
                    Recipient = friends[0]
                },
                new Message
                {
                    Date = DateTime.Now - TimeSpan.FromMinutes(9),
                    Text = "Hi, Musk. I am OK, thank you. How about you?",
                    Sender = friends[0],
                    Recipient = friends[1]
                },
                new Message
                {
                    Date = DateTime.Now - TimeSpan.FromMinutes(8),
                    Text = "I am good too. Have you fixed the bug with the chat?",
                    Sender = friends[1],
                    Recipient = friends[0]
                },
                new Message
                {
                    Date = DateTime.Now - TimeSpan.FromMinutes(1),
                    Text = "Yes, as you can see now. Works great.",
                    Sender = friends[0],
                    Recipient = friends[1]
                },
                new Message
                {
                    Date = DateTime.Now - TimeSpan.FromMinutes(4),
                    Text = "Hello, Bill.",
                    Sender = friends[2],
                    Recipient = friends[0]
                },
                new Message
                {
                    Date = DateTime.Now - TimeSpan.FromMinutes(3),
                    Text = "Hello, Bjarne.",
                    Sender = friends[0],
                    Recipient = friends[2]
                },
                new Message
                {
                    Date = DateTime.Now - TimeSpan.FromMinutes(2),
                    Text = "Could you please check the administration page of the site?",
                    Sender = friends[2],
                    Recipient = friends[0]
                },
                new Message
                {
                    Date = DateTime.Now - TimeSpan.FromMinutes(64),
                    Text = "Hello, Bill. Are you free tomorrow?",
                    Sender = friends[3],
                    Recipient = friends[0]
                },
                new Message
                {
                    Date = DateTime.Now - TimeSpan.FromMinutes(63),
                    Text = "I have to finish something, but I will be free after 3:30.",
                    Sender = friends[0],
                    Recipient = friends[3]
                },
                new Message
                {
                    Date = DateTime.Now - TimeSpan.FromMinutes(62),
                    Text = "Do you want to get together after you finish work?",
                    Sender = friends[3],
                    Recipient = friends[0]
                },
                new Message
                {
                    Date = DateTime.Now - TimeSpan.FromMinutes(61),
                    Text = "Yes, I do.",
                    Sender = friends[0],
                    Recipient = friends[3]
                },
                new Message
                {
                    Date = DateTime.Now - TimeSpan.FromMinutes(125),
                    Text = "Hey, how have you been?",
                    Sender = friends[6],
                    Recipient = friends[0]
                }
            };

            var notExistMessages =
                billMessages.Where(message => !messageRepository.MessageWithTextExists(message.Text));

            foreach (var message in notExistMessages)
            {
                messageRepository.Save(message);
            }
        }

        private void AddUserTasksToBill()
        {
            var bill = citizenUserRepository.GetUserByLogin("Bill");

            var billTasks = new List<UserTask>()
            {
                new UserTask
                {
                    Name = "Develop task feature",
                    Description = "Design an user interface and implement business logic",
                    StartDate = DateTime.Now - TimeSpan.FromDays(1),
                    Status = TaskStatus.Planned,
                    Priority = TaskPriority.Medium,
                    Owner = bill
                },
                new UserTask
                {
                    Name = "Add test data to database",
                    Description = "Implement extension for database seeding. Ensure that admin account exists",
                    StartDate = DateTime.Now + TimeSpan.FromHours(1),
                    Status = TaskStatus.Planned,
                    Priority = TaskPriority.Low,
                    Owner = bill
                },
                new UserTask
                {
                    Name = "Migrate from Newtonsoft.Json to System.Text.Json",
                    Description = "Remove all dependencies on Newtonsoft.Json and replace it with built-in Json",
                    StartDate = DateTime.Now + TimeSpan.FromHours(2),
                    Status = TaskStatus.Planned,
                    Priority = TaskPriority.Medium,
                    Owner = bill
                },
                new UserTask
                {
                    Name = "Fix bug with .Designer file",
                    Description = "Last migration cause an error with DbContext file",
                    StartDate = DateTime.Now + TimeSpan.FromHours(3),
                    Status = TaskStatus.Planned,
                    Priority = TaskPriority.High,
                    Owner = bill
                },
                new UserTask
                {
                    Name = "Rename CitizenUser properties",
                    Description = "Marriage and HaveChildren should be renamed to IsMarried and HasChildren",
                    StartDate = DateTime.Now + TimeSpan.FromHours(4),
                    Status = TaskStatus.Planned,
                    Priority = TaskPriority.Low,
                    Owner = bill
                },
                new UserTask
                {
                    Name = "Refactor user service",
                    Description = "Make UserService class more readable",
                    StartDate = DateTime.Now + TimeSpan.FromHours(5),
                    Status = TaskStatus.Planned,
                    Priority = TaskPriority.Low,
                    Owner = bill
                },
                new UserTask
                {
                    Name = "Create custom theme for site",
                    Description = "Find good theme in Internet and add it to website",
                    StartDate = DateTime.Now.Date + TimeSpan.FromDays(1),
                    Status = TaskStatus.Planned,
                    Priority = TaskPriority.Low,
                    Owner = bill
                },
                new UserTask
                {
                    Name = "Merge with upstream",
                    Description = "Pavel add new feature to the project. It should be pulled.",
                    StartDate = DateTime.Now.Date + TimeSpan.FromHours(30),
                    Status = TaskStatus.Planned,
                    Priority = TaskPriority.Medium,
                    Owner = bill
                },
                new UserTask
                {
                    Name = "Find the cause of the bug with database updating",
                    Description = "After last migration updates are not working",
                    StartDate = DateTime.Now.Date + TimeSpan.FromHours(31),
                    Status = TaskStatus.Planned,
                    Priority = TaskPriority.High,
                    Owner = bill
                },
                new UserTask
                {
                    Name = "Add unique constraint to login",
                    Description = "Login should be unique for whole database",
                    StartDate = DateTime.Now.Date + TimeSpan.FromHours(32),
                    Status = TaskStatus.Planned,
                    Priority = TaskPriority.High,
                    Owner = bill
                },
                new UserTask
                {
                    Name = "Implement policy-based authorization",
                    Description = "Create 2 policies: one for admins and one for users",
                    StartDate = DateTime.Now.Date + TimeSpan.FromHours(33),
                    Status = TaskStatus.Planned,
                    Priority = TaskPriority.High,
                    Owner = bill
                },
                new UserTask
                {
                    Name = "Remove unnecessary code from project",
                    Description = "Delete code that is unreachable by program",
                    StartDate = DateTime.Now + TimeSpan.FromDays(3),
                    Status = TaskStatus.Planned,
                    Priority = TaskPriority.Low,
                    Owner = bill
                },
                new UserTask
                {
                    Name = "Write unit-tests",
                    Description = "Cover the project with unit-tests",
                    StartDate = DateTime.Now + TimeSpan.FromDays(3),
                    Status = TaskStatus.Planned,
                    Priority = TaskPriority.Medium,
                    Owner = bill
                },
                new UserTask
                {
                    Name = "Add administration page",
                    Description = "It should perform CRUD operations with all database tables",
                    StartDate = DateTime.Now + TimeSpan.FromDays(4),
                    Status = TaskStatus.Planned,
                    Priority = TaskPriority.High,
                    Owner = bill
                },
                new UserTask
                {
                    Name = "Deploy website",
                    Description = "Find free hosting and publish website to it",
                    StartDate = DateTime.Now + TimeSpan.FromDays(5),
                    Status = TaskStatus.Planned,
                    Priority = TaskPriority.High,
                    Owner = bill
                },
                new UserTask
                {
                    Name = "Test deployed website",
                    Description = "Search for bugs and make hot fixes",
                    StartDate = DateTime.Now + TimeSpan.FromDays(10),
                    Status = TaskStatus.Planned,
                    Priority = TaskPriority.High,
                    Owner = bill
                }
            };

            var notExistBillTasks = billTasks.Where(task => !userTaskRepository.TaskWithNameExists(task.Name));

            foreach (var billTask in notExistBillTasks)
            {
                userTaskRepository.Save(billTask);
            }
        }
    }
}

