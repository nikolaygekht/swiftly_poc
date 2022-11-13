using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Swiftly.DatabaseContext.Test.DbContextRegistryTest;

namespace Swiftly.DatabaseContext.Test
{
    public class DbContextRegistryTest
    {
        [Fact]
        public void RegisterInCollection()
        {
            var collection = new ServiceCollection();
            collection.AddDbContextRegistry();
            collection.Should()
                .Contain(sd => sd.ServiceType == typeof(SwitlyDbContextRegisty) &&
                               sd.Lifetime == ServiceLifetime.Singleton &&
                               sd.ImplementationInstance != null);
        }

        [Fact]
        public void FindInCollection()
        {
            var collection = new ServiceCollection();
            collection.AddDbContextRegistry();
            collection.FindDbContextRegistry().Should().NotBeNull();
        }

        [Fact]
        public void FindInCollection_WhenNotInstalled()
        {
            var collection = new ServiceCollection();
            collection.FindDbContextRegistry().Should().BeNull();
        }

        public class DummyDbContext : DbContext
        {
        }
        
        public class DummyDbContext2 : DbContext
        {
        }

        public class NoDbContext
        {
        }

        [Fact]
        public void AddService_Successful_ViaType()
        {
            var registry = new SwitlyDbContextRegisty();
            ((Action)(() => registry.AddContext(typeof(DummyDbContext)))).Should().NotThrow();
            registry.Contexts.Should().Contain(typeof(DummyDbContext));
        }

        [Fact]
        public void AddService_Successful_ViaGeneric()
        {
            var registry = new SwitlyDbContextRegisty();
            ((Action)(() => registry.AddContext<DummyDbContext>())).Should().NotThrow();
            registry.Contexts.Should().Contain(typeof(DummyDbContext));
        }


        [Fact]
        public void AddService_Successful_Two()
        {
            var registry = new SwitlyDbContextRegisty();
            registry.AddContext<DummyDbContext>();
            registry.AddContext<DummyDbContext2>();
            registry.Contexts.Should().Contain(typeof(DummyDbContext))
                                  .And.Contain(typeof(DummyDbContext2));
        }

        [Fact]
        public void AddService_Fail_NoContext()
        {
            var registry = new SwitlyDbContextRegisty();
            ((Action)(() => registry.AddContext(typeof(NoDbContext)))).Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AddService_Fail_AddTwice()
        {
            var registry = new SwitlyDbContextRegisty();
            ((Action)(() => registry.AddContext<DummyDbContext>())).Should().NotThrow();
            ((Action)(() => registry.AddContext<DummyDbContext>())).Should().Throw<ArgumentException>();

        }
    }
}
