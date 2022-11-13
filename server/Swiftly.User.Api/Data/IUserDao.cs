using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swiftly.User.Api.Data.Provider.EF;

namespace Swiftly.User.Api.Data
{
    /// <summary>
    /// The interface to the user DAO
    /// </summary>
    internal interface IUserDao
    {
        /// <summary>
        /// Gets user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        User Get(int userId);
        
        /// <summary>
        /// Finds user by the name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        User Find(string userName);
        
        /// <summary>
        /// Checks whether the user with such name exists
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        bool IsAnotherUserWithSameNameExists(string userName, int identifier);
        
        /// <summary>
        /// Updates user information
        /// </summary>
        /// <param name="user"></param>
        void Save(User user);
        
        /// <summary>
        /// Deletes user
        /// </summary>
        /// <param name="user"></param>
        void Delete(User user);

        /// <summary>
        /// Gets password hash for the specified user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        string GetPasswordHash(int userId);

        /// <summary>
        /// Gets a list of the users
        /// </summary>
        /// <param name="nameFilter">The beginning of the user name</param>
        /// <param name="email">The complete email</param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        User[] GetUsers(string nameFilter, string email, UserRole? role, UserStatus? status, int skip, int take);

        /// <summary>
        /// Gets the number of the users
        /// </summary>
        /// <param name="nameFilter">The beginning of the user name</param>
        /// <param name="email">The complete email</param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        int CountUsers(string nameFilter, string email, UserRole? role, UserStatus? status);
    }
}
