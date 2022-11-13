using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swiftly.DatabaseContext
{
    /// <summary>
    /// The interface for configuration of all database context
    /// </summary>
    public interface ISwiftlyDbConfigurer
    {
        void OnConfiguring(DbContextOptionsBuilder optionsBuilder);
    }
}

