using Swiftly.User.Api.Data;
using System;

namespace Swiftly.User.Api
{
    /// <summary>
    /// Implementation of the user API
    /// </summary>
    internal class UserApi : IUserApi
    {
        private readonly IUserDao mDao;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dao"></param>
        public UserApi(IUserDao dao)
        {
            mDao = dao;
        }

        /// <summary>
        /// Gets user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public User Get(int userId)
        {
            try
            {
                var user = mDao.Get(userId);
                return user;
            }
            catch (Exception e)
            {
                throw new UserApiException("Get", e);
            }

        }

        /// <summary>
        /// Finds user by the name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public User Find(string userName)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            try
            {
                var user = mDao.Find(userName);
                return user;
            }
            catch (Exception e)
            {
                throw new UserApiException("Find", e);
            }
        }

        /// <summary>
        /// Finds user by the name and validate the password hash
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passwordHash"></param>
        /// <returns>The method returns a user if the user is authorized or null if user does not exist or password hash does not match</returns>
        public User ValidatePassword(string userName, string passwordHash)
        {
            try
            {
                var user = mDao.Find(userName);
                if (user == null)
                    return null;
                var hash = mDao.GetPasswordHash(user.Id);
                
                if (hash == null)
                    return null;
                
                if (string.Compare(hash, passwordHash, StringComparison.OrdinalIgnoreCase) != 0)
                    return null;
                
                return user;
            }
            catch (Exception e)
            {
                throw new UserApiException("ValidatePassword", e);
            }
        }

        /// <summary>
        /// Updates user information
        /// </summary>
        /// <param name="user"></param>
        public void Save(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(user.UserName))
                throw new ArgumentException("The name must be set", nameof(user));

            if (mDao.IsAnotherUserWithSameNameExists(user.UserName, user.Id))
                throw new ArgumentException("The name must be unique", nameof(user));

            try
            {
                mDao.Save(user);
            }
            catch (Exception e)
            {
                throw new UserApiException("Save", e);
            }

        }

        /// <summary>
        /// Deletes user
        /// </summary>
        /// <param name="user"></param>
        public void Delete(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                mDao.Delete(user);
            }
            catch (Exception e)
            {
                throw new UserApiException("Save", e);
            }

        }

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
        public User[] GetUsers(string nameFilter, string email, UserRole? role, UserStatus? status, int skip, int take)
        {
            if (take > 500)
                throw new UserApiException("GetUsers", "The maximum number of users to return is 500");
            try
            {
                return mDao.GetUsers(nameFilter, email, role, status, skip, take);
            }
            catch (Exception e)
            {
                throw new UserApiException("GetUsers", e);
            }
        }

        /// <summary>
        /// Gets a number of the users
        /// </summary>
        /// <param name="nameFilter">The beginning of the user name</param>
        /// <param name="email">The complete email</param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int CountUsers(string nameFilter, string email, UserRole? role, UserStatus? status)
        {
            try
            {
                return mDao.CountUsers(nameFilter, email, role, status);
            }
            catch (Exception e)
            {
                throw new UserApiException("GetUsers", e);
            }
        }
    }
}
