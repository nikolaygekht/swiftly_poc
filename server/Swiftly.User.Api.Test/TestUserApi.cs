using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Swiftly.DatabaseContext;
using Swiftly.User.Api.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Swiftly.User.Api.Test
{
    public sealed class TestUserApi : IDisposable
    {
        private readonly ServiceProvider mServiceProvider;

        public string DatabasePath
        {
            get
            {
                var fi = new FileInfo(this.GetType().Assembly.Location);
                return Path.Combine(fi.DirectoryName, "TestUserApi.db");
            }
        }

        public TestUserApi()
        {
            var dbfile = DatabasePath;
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddDbContextRegistry()
                //.AddDbContextConfigurer(options => options.UseSqlite("DataSource=myshareddb;mode=memory;cache=shared"))
                .AddDbContextConfigurer(options => options.UseSqlite($"DataSource={dbfile}"))
                .AddUserApi();

            mServiceProvider = serviceCollection.BuildServiceProvider();

            var registry = mServiceProvider.GetRequiredService<SwitlyDbContextRegisty>();
            foreach (var contextType in registry.Contexts)
            {
                var context = mServiceProvider.GetRequiredService(contextType) as DbContext;
                context.Should().NotBeNull();
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }

        public void Dispose()
        {
            mServiceProvider.Dispose();
        }

        [Fact]
        public void EmptyDatabase_Initially()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            api.CountUsers(null, null, null, null).Should().Be(0);
        }

        [Fact]
        public void Create_New()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var user = new User()
            {
                UserName = "user1",
                Email = "email@domain.com",
                PasswordHash = PasswordHashController.GetHash("password"),
                Role = UserRole.Administator,
                Status = UserStatus.Active
            };
            api.Save(user);
            user.Id.Should().BeGreaterThan(0, "Identifier must be update");
            api.CountUsers(null, null, null, null).Should().Be(1);
        }

        [Fact]
        public void Create_ReadBack()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var user = new User()
            {
                UserName = "user1",
                Email = "email@domain.com",
                PasswordHash = PasswordHashController.GetHash("password"),
                Role = UserRole.Administator,
                Status = UserStatus.Active
            };
            api.Save(user);

            var user2 = api.Get(user.Id);
            user2.Should().NotBeNull();

            user2.Id.Should().Be(user.Id);
            user2.UserName.Should().Be(user.UserName);
            user2.Email.Should().Be(user.Email);
            user2.PasswordHash.Should().BeNull();
            user2.Role.Should().Be(user.Role);
            user2.Status.Should().Be(user.Status);

            api.ValidatePassword(user.UserName, PasswordHashController.GetHash("password")).Should().NotBeNull();
        }

        [Fact]
        public void Create_Fail_SameName()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var user = new User()
            {
                UserName = "user1",
                Email = "email@domain.com",
                PasswordHash = PasswordHashController.GetHash("password"),
                Role = UserRole.Administator,
                Status = UserStatus.Active
            };

            api.Save(user);

            user = new User()
            {
                UserName = "user1",
                Email = "email@domain.com",
                PasswordHash = PasswordHashController.GetHash("password"),
                Role = UserRole.Administator,
                Status = UserStatus.Active
            };

            ((Action)(() => api.Save(user))).Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_Success_DifferentName()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var user = new User()
            {
                UserName = "user1",
                Email = "email@domain.com",
                PasswordHash = PasswordHashController.GetHash("password"),
                Role = UserRole.Administator,
                Status = UserStatus.Active
            };

            api.Save(user);

            user = new User()
            {
                UserName = "user2",
                Email = "email@domain.com",
                PasswordHash = PasswordHashController.GetHash("password"),
                Role = UserRole.Administator,
                Status = UserStatus.Active
            };

            ((Action)(() => api.Save(user))).Should().NotThrow();

            api.CountUsers(null, null, null, null).Should().Be(2);
        }
        
        [Fact]
        public void Edit_Success_Rename()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var user = new User()
            {
                UserName = "user1",
                Email = "email@domain.com",
                PasswordHash = PasswordHashController.GetHash("password"),
                Role = UserRole.Administator,
                Status = UserStatus.Active
            };
            
            api.Save(user);

            user.UserName = "user2";
            user.Email = "newemail@domain.com";
            user.Role = UserRole.Driver;
            api.Save(user);

            var user2 = api.Get(user.Id);
            user2.Should().NotBeNull();

            user2.Id.Should().Be(user.Id);
            user2.UserName.Should().Be(user.UserName);
            user2.Email.Should().Be(user.Email);
            user2.PasswordHash.Should().BeNull();
            user2.Role.Should().Be(user.Role);
            user2.Status.Should().Be(user.Status);

            api.ValidatePassword(user.UserName, PasswordHashController.GetHash("password")).Should().NotBeNull();
        }
        
        [Fact]
        public void Edit_Success_NoRename()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var user = new User()
            {
                UserName = "user1",
                Email = "email@domain.com",
                PasswordHash = PasswordHashController.GetHash("password"),
                Role = UserRole.Administator,
                Status = UserStatus.Active
            };

            api.Save(user);

            user.Email = "newemail@domain.com";
            user.Role = UserRole.Driver;

            api.Save(user);

            var user2 = api.Get(user.Id);
            user2.Should().NotBeNull();

            user2.Id.Should().Be(user.Id);
            user2.UserName.Should().Be(user.UserName);
            user2.Email.Should().Be(user.Email);
            user2.PasswordHash.Should().BeNull();
            user2.Role.Should().Be(user.Role);
            user2.Status.Should().Be(user.Status);

            api.ValidatePassword(user.UserName, PasswordHashController.GetHash("password")).Should().NotBeNull();
        }

        [Fact]
        public void Edit_Success_ChangePassword()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var user = new User()
            {
                UserName = "user1",
                Email = "email@domain.com",
                PasswordHash = PasswordHashController.GetHash("password"),
                Role = UserRole.Administator,
                Status = UserStatus.Active
            };

            api.Save(user);

            user.Email = "newemail@domain.com";
            user.PasswordHash = PasswordHashController.GetHash("password1");
            user.Role = UserRole.Driver;

            api.Save(user);

            var user2 = api.Get(user.Id);
            user2.Should().NotBeNull();

            user2.PasswordHash.Should().BeNull();

            api.ValidatePassword(user.UserName, PasswordHashController.GetHash("password1")).Should().NotBeNull();
        }

        [Fact]
        public void Create_Fail_RenameToExistingUser()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var user = new User()
            {
                UserName = "user1",
                Email = "email@domain.com",
                PasswordHash = PasswordHashController.GetHash("password"),
                Role = UserRole.Administator,
                Status = UserStatus.Active
            };

            api.Save(user);

            var user1 = new User()
            {
                UserName = "user2",
                Email = "email@domain.com",
                PasswordHash = PasswordHashController.GetHash("password"),
                Role = UserRole.Administator,
                Status = UserStatus.Active
            };

            api.Save(user1);

            var user2 = api.Get(user.Id);

            user2.UserName = user1.UserName;
            ((Action)(() => api.Save(user2))).Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_Delete()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var user = new User()
            {
                UserName = "user1",
                Email = "email@domain.com",
                PasswordHash = PasswordHashController.GetHash("password"),
                Role = UserRole.Administator,
                Status = UserStatus.Active
            };
            api.Save(user);
            user.Id.Should().BeGreaterThan(0, "Identifier must be update");

            api.Delete(user);
            
            api.CountUsers(null, null, null, null).Should().Be(0);
            api.Get(user.Id).Should().BeNull();
        }

        [Fact]
        public void CheckPassword_OK()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var user = new User()
            {
                UserName = "user1",
                Email = "email@domain.com",
                PasswordHash = PasswordHashController.GetHash("password"),
                Role = UserRole.Administator,
                Status = UserStatus.Active
            };
            api.Save(user);
            api.ValidatePassword("user1", PasswordHashController.GetHash("password")).Should()
                .NotBeNull()
                .And.Subject.As<User>().UserName.Should().Be("user1");
        }

        [Fact]
        public void CheckPassword_Fail_NoUser()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var user = new User()
            {
                UserName = "user1",
                Email = "email@domain.com",
                PasswordHash = PasswordHashController.GetHash("password"),
                Role = UserRole.Administator,
                Status = UserStatus.Active
            };
            api.Save(user);
            api.ValidatePassword("user2", PasswordHashController.GetHash("password")).Should()
                .BeNull();
        }

        [Fact]
        public void CheckPassword_Fail_WrongPassword()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var user = new User()
            {
                UserName = "user1",
                Email = "email@domain.com",
                PasswordHash = PasswordHashController.GetHash("password"),
                Role = UserRole.Administator,
                Status = UserStatus.Active
            };
            api.Save(user);
            api.ValidatePassword("user1", PasswordHashController.GetHash("password1")).Should()
                .BeNull();
        }

        [Fact]
        public void CheckPassword_Fail_UserInactive()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var user = new User()
            {
                UserName = "user1",
                Email = "email@domain.com",
                PasswordHash = PasswordHashController.GetHash("password"),
                Role = UserRole.Administator,
                Status = UserStatus.Inactive
            };
            api.Save(user);
            api.ValidatePassword("user1", PasswordHashController.GetHash("password")).Should()
                .BeNull();
        }

        private static User[] CreateSet(IUserApi api, int size)
        {
            var users = new User[size];
            for (int i = 0; i < size; i++)
            {
                users[i] = new User()
                {
                    UserName = $"{i}user{i}",
                    Email = $"user{i}@domain{i}.com",
                    PasswordHash = PasswordHashController.GetHash($"password{i}"),
                    Role = (i % 5) switch
                    {
                        0 => UserRole.Administator,
                        1 => UserRole.BackofficeManager,
                        2 => UserRole.Driver,
                        3 => UserRole.Customer,
                        4 => UserRole.Provider,
                        _ => throw new InvalidOperationException()
                    },
                    Status = (i % 2) == 0 ? UserStatus.Active : UserStatus.Inactive
                };
            }
            api.MassCreate(users);
            Array.Sort(users, (a, b) => a.UserName.CompareTo(b.UserName));
            return users;
        }

        [Fact]
        public void Read_All()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var users = CreateSet(api, 10);
            

            api.CountUsers(null, null, null, null).Should().Be(10);
            var users1 = api.GetUsers(null, null, null, null, 0, 100).ToArray();
            users1.Should().HaveCount(10);
            foreach (var user in users)
                users1.Should().Contain(u => u.Id == user.Id);
        }

        [Fact]
        public void Read_FromTo()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var users = CreateSet(api, 20);

            api.CountUsers(null, null, null, null).Should().Be(20);
            var users1 = api.GetUsers(null, null, null, null, 0, 100).ToArray();
            users1.Should().HaveCount(20);
            foreach (var user in users)
                users1.Should().Contain(u => u.Id == user.Id);
        }

        [Fact]
        public void Read_TakeSkip()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var users = CreateSet(api, 20);

            var users1 = api.GetUsers(null, null, null, null, 1, 3).ToArray();
            users1.Should().HaveCount(3);
            users1.Should().Contain(u => u.Id == users[1].Id);
            users1.Should().Contain(u => u.Id == users[2].Id);
            users1.Should().Contain(u => u.Id == users[3].Id);
        }

        [Fact]
        public void Read_FilterByFullName ()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var users = CreateSet(api, 20);

            api.CountUsers(users[3].UserName, null, null, null).Should().Be(1);
            var users1 = api.GetUsers(users[3].UserName, null, null, null, 0, 100).ToArray();
            users1.Should().HaveCount(1);
            users1.Should().Contain(u => u.Id == users[3].Id);
        }

        [Fact]
        public void Find_OK()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var users = CreateSet(api, 20);

            var user1 = api.Find(users[3].UserName);
            user1.Should().NotBeNull();
            user1.Id.Should().Be(users[3].Id);
            user1.UserName.Should().Be(users[3].UserName);
            user1.Email.Should().Be(users[3].Email);
            user1.PasswordHash.Should().BeNull();
            user1.Role.Should().Be(users[3].Role);
            user1.Status.Should().Be(users[3].Status);
        }

        [Fact]
        public void Find_Fail()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            CreateSet(api, 20);

            var user1 = api.Find("non-exisiting");
            user1.Should().BeNull();
        }

        [Fact]
        public void Read_FilterByNameStart()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var users = CreateSet(api, 20).Where(u => u.UserName.StartsWith("1")).ToArray();
            users.Should().HaveCountLessThan(20);

            api.CountUsers("1*", null, null, null).Should().Be(users.Length);
            foreach (var user in api.GetUsers("1*", null, null, null, 0, 100))
                users.Should().Contain(u => u.Id == user.Id);
        }

        [Fact]
        public void Read_FilterByNameEnd()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var users = CreateSet(api, 20).Where(u => u.UserName.EndsWith("1")).ToArray();
            users.Should().HaveCountLessThan(20);

            api.CountUsers("*1", null, null, null).Should().Be(users.Length);
            foreach (var user in api.GetUsers("*1", null, null, null, 0, 100))
                users.Should().Contain(u => u.Id == user.Id);
        }

        [Fact]
        public void Read_FilterByNameContains()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var users = CreateSet(api, 20).Where(u => u.UserName.Contains("r1")).ToArray();
            users.Should().HaveCountLessThan(20);

            api.CountUsers("*r1*", null, null, null).Should().Be(users.Length);
            foreach (var user in api.GetUsers("*r1*", null, null, null, 0, 100))
                users.Should().Contain(u => u.Id == user.Id);
        }

        [Fact]
        public void Read_FilterByEmail()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var users = CreateSet(api, 20);

            api.CountUsers(null, users[3].Email, null, null).Should().Be(1);
            var users1 = api.GetUsers(null, users[3].Email, null, null, 0, 100).ToArray();
            users1.Should().HaveCount(1);
            users1.Should().Contain(u => u.Id == users[3].Id);
        }

        [Fact]
        public void Read_FilterByRole()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var users = CreateSet(api, 20).Where(u => u.Role == UserRole.Customer).ToArray();
            users.Should().HaveCountLessThan(20);

            api.CountUsers(null, null, UserRole.Customer, null).Should().Be(users.Length);
            foreach (var user in api.GetUsers(null, null, UserRole.Customer, null, 0, 100))
                users.Should().Contain(u => u.Id == user.Id);
        }

        [Fact]
        public void Read_FilterByStatus()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            var users = CreateSet(api, 20).Where(u => u.Status == UserStatus.Active).ToArray();
            users.Should().HaveCountLessThan(20);

            api.CountUsers(null, null, null, UserStatus.Active).Should().Be(users.Length);
            foreach (var user in api.GetUsers(null, null, null, UserStatus.Active, 0, 100))
                users.Should().Contain(u => u.Id == user.Id);
        }
    }

    public sealed class TestUserApi_Exceptions : IDisposable
    {
        private readonly ServiceProvider mServiceProvider;

        public TestUserApi_Exceptions()
        {
            var mockDao = new Mock<IUserDao>();

            mockDao.Setup(d => d.Get(It.IsAny<int>()))
                .Throws(new Exception("Failed"));
            mockDao.Setup(d => d.Find(It.IsAny<string>()))
                .Throws(new Exception("Failed"));
            mockDao.Setup(d => d.Save(It.IsAny<User>()))
                .Throws(new Exception("Failed"));
            mockDao.Setup(d => d.Delete(It.IsAny<User>()))
                .Throws(new Exception("Failed"));
            mockDao.Setup(d => d.MassCreate(It.IsAny<IEnumerable<User>>()))
                .Throws(new Exception("Failed"));
            mockDao.Setup(d => d.IsAnotherUserWithSameNameExists(It.IsAny<string>(), It.IsAny<int>()))
                .Throws(new Exception("Failed"));
            mockDao.Setup(d => d.CountUsers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole?>(), It.IsAny<UserStatus?>()))
                .Throws(new Exception("Failed"));
            mockDao.Setup(d => d.GetUsers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole?>(), It.IsAny<UserStatus?>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new Exception("Failed"));

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddSingleton<IUserDao>(mockDao.Object)
                .AddUserApi();

            mServiceProvider = serviceCollection.BuildServiceProvider();
        }


        public void Dispose()
        {
            mServiceProvider.Dispose();
        }

        [Fact]
        public void GetFailed()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            ((Action)(() => api.Get(1))).Should().Throw<UserApiException>()
                .Which
                .Should().Match<UserApiException>(e => e.Operation == "Get");
        }

        [Fact]
        public void FindFailed()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            ((Action)(() => api.Find(null))).Should().Throw<ArgumentNullException>();
            ((Action)(() => api.Find("a"))).Should().Throw<UserApiException>()
                .Which
                .Should().Match<UserApiException>(e => e.Operation == "Find");
        }

        [Fact]
        public void SaveFailed()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            ((Action)(() => api.Save(null))).Should().Throw<ArgumentNullException>();
            ((Action)(() => api.Save(new User()))).Should().Throw<ArgumentException>();
            ((Action)(() => api.Save(new User() { UserName = "" }))).Should().Throw<ArgumentException>();
            ((Action)(() => api.Save(new User() { UserName = "abc" }))).Should().Throw<UserApiException>()
                .Which
                .Should().Match<UserApiException>(e => e.Operation == "Save");
        }

        [Fact]
        public void DeleteFailed()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            ((Action)(() => api.Delete(null))).Should().Throw<ArgumentNullException>();
            ((Action)(() => api.Delete(new User() { Id = 0 }))).Should().Throw<ArgumentException>();
            ((Action)(() => api.Delete(new User() { Id = 1 }))).Should().Throw<UserApiException>()
                .Which
                .Should().Match<UserApiException>(e => e.Operation == "Delete");
        }

        [Fact]
        public void CountFailed()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            ((Action)(() => api.CountUsers(null, null, null, null))).Should().Throw<UserApiException>()
                .Which
                .Should().Match<UserApiException>(e => e.Operation == "CountUsers");
        }

        [Fact]
        public void ReadFailed()
        {
            var api = mServiceProvider.GetRequiredService<IUserApi>();
            ((Action)(() => api.GetUsers(null, null, null, null, 0, 0))).Should().Throw<UserApiException>()
                .Which
                .Should().Match<UserApiException>(e => e.Operation == "GetUsers");
        }
    }
}
