using Swiftly.User.Api.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swiftly.User.Api
{
    /// <summary>
    /// The interface to a user API
    /// </summary>
    public interface IUserApi
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
        /// Finds user by the name and validate the password hash
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>The method returns a user if the user is authorized or null if user does not exist or password hash does not match</returns>
        User ValidatePassword(string userName, string passwordHash);

        /// <summary>
        /// Creates multiple users at a time
        /// </summary>
        /// <param name="users"></param>
        void MassCreate(IEnumerable<User> users);

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
        /// Gets a list of the users
        /// </summary>
        /// <param name="nameFilter">The name filter. 
        /// 
        /// The filter may be either the exact name or has * character in the beginning or in the ending to find partial value.
        /// 
        /// Avoid using * in the beginning, it affects search performance.
        /// </param>
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
        /// <param name="nameFilter">The name filter. 
        /// 
        /// The filter may be either the exact name or has * character in the beginning or in the ending to find partial value.
        /// 
        /// Avoid using * in the beginning, it affects search performance.
        /// </param>
        /// <param name="email">The complete email</param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        int CountUsers(string nameFilter, string email, UserRole? role, UserStatus? status);
    }
}
