using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
            user.Status = UserStatus.Inactive;

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
            user.Status = UserStatus.Inactive;

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
    }
}
