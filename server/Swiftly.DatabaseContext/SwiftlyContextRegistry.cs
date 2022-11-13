using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swiftly.DatabaseContext
{
    /// <summary>
    /// The registry of all DB context
    /// </summary>
    public class SwitlyDbContextRegisty
    {
        private readonly List<Type> mContexts = new();

        /// <summary>
        /// Returns all contexts registered
        /// </summary>
        public IReadOnlyList<Type> Contexts => mContexts;

        /// <summary>
        /// Adds context type to the registry
        /// </summary>
        /// <param name="contextType"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddContext(Type contextType)
        {
            if (contextType == null)
                throw new ArgumentNullException(nameof(contextType));

            if (!typeof(DbContext).IsAssignableFrom(contextType))
                throw new ArgumentException("The type must be derived from a DbContext", nameof(contextType));

            if (mContexts.Find(t => t == contextType) != null)
                throw new ArgumentException($"The context {contextType.Name} is already registered", nameof(contextType));

            mContexts.Add(contextType);
        }

        /// <summary>
        /// Adds context type to the registry (generic method)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddContext<T>()
            where T : DbContext => AddContext(typeof(T));
    }
}
