using Microsoft.EntityFrameworkCore;
using System;

namespace Swiftly.DatabaseContext
{
    internal class SwiftlyDbActionConfigurer : ISwiftlyDbConfigurer
    {
        private readonly Action<DbContextOptionsBuilder> mAction;

        public SwiftlyDbActionConfigurer(Action<DbContextOptionsBuilder> action)
        {
            mAction = action;
        }

        public void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            mAction(optionsBuilder);
        }
    }
}

