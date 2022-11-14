using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Swiftly.User.Api.Data.Provider.EF
{
    /// <summary>
    /// EF implementation of user API DAO
    /// </summary>
    internal class UserApiDao : IUserDao
    {
        private readonly UserApiContext mContext;
        private readonly IMapper mMapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public UserApiDao(UserApiContext context, IMapper mapper)
        {
            mContext = context;
            mMapper = mapper;
        }
       
        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="user"></param>
        public void Delete(User user)
        {
            var entity = mContext.Users.Where(u => u.Id == user.Id).FirstOrDefault();
            if (entity != null)
            {
                mContext.Remove(entity);
                mContext.SaveChanges();
            }
        }

        /// <summary>
        /// Finds user by the name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public User Find(string userName)
        {
            var rs = mContext.Users
                .AsNoTracking()
                .Where(m => m.UserName == userName).Take(100);
            
            var e = rs.FirstOrDefault();
            if (e == null)
                return null;
            return mMapper.Map<User>(e);
        }

        /// <summary>
        /// Gets password hash for the specified user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetPasswordHash(int userId)
        {
            var entity = mContext.Users
                .AsNoTracking()
                .Where(m => m.Id == userId).Take(100).FirstOrDefault();
            if (entity == null)
                return null;
            return entity.PasswordHash;
        }


        /// <summary>
        /// Gets user by identifier
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public User Get(int userId)
        {
            var rs = mContext.Users
                .AsNoTracking()
                .Where(m => m.Id == userId).Take(100);
            var e = rs.FirstOrDefault();
            if (e == null)
                return null;
            return mMapper.Map<User>(e);
        }

        /// <summary>
        /// Creates multiple users at a time
        /// </summary>
        /// <param name="users"></param>
        public void MassCreate(IEnumerable<User> users)
        {
            var arr = users.ToArray();
            var arr1 = new UserEntity[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                arr1[i] = New(arr[i], false);
            mContext.SaveChanges();
            for (int i = 0; i < arr.Length; i++)
                arr[i].Id = arr1[i].Id;
        }

        /// <summary>
        /// Saves a user
        /// </summary>
        /// <param name="user"></param>
        public void Save(User user) => Save(user, true);

        /// <summary>
        /// Saves a user
        /// </summary>
        /// <param name="user"></param>
        private void Save(User user, bool save)
        {
            if (user.Id == 0)
            {
                New(user, save);
                return;
            }

            var entity = mContext.Users.Where(m => m.Id == user.Id).FirstOrDefault();
            if (entity == null)
            {
                New(user, save);
                return;
            }
            
            mMapper.Map(user, entity);
            mContext.Update(entity);
            if (save)
                mContext.SaveChanges();
        }

        private UserEntity New(User user, bool save)
        {
            var entity = mMapper.Map<UserEntity>(user);
            mContext.Add(entity);
            if (save)
            {
                mContext.SaveChanges();
                user.Id = entity.Id;
            }
            return entity;
        }

        private static IQueryable<UserEntity> SetupUserListFilters(IQueryable<UserEntity> query, string nameFilter, string email, UserRole? role, UserStatus? status)
        {
            if (!string.IsNullOrEmpty(nameFilter))
            {
                if (nameFilter.StartsWith('*') && nameFilter.EndsWith('*'))
                    query = query.Where(m => m.UserName.Contains(nameFilter.Substring(1, nameFilter.Length - 2)));
                else if (nameFilter.StartsWith('*'))
                    query = query.Where(m => m.UserName.EndsWith(nameFilter.Substring(1)));
                else if (nameFilter.EndsWith('*'))
                    query = query.Where(m => m.UserName.StartsWith(nameFilter.Substring(0, nameFilter.Length - 1)));
                else
                    query = query.Where(m => m.UserName == nameFilter);
            }
            if (!string.IsNullOrEmpty(email))
                query = query.Where(m => m.Email == email);
            if (role.HasValue)
                query = query.Where(m => m.Role == role.Value);
            if (status.HasValue)
                query = query.Where(m => m.Status == status.Value);
            return query;
        }

        /// <summary>
        /// Gets a list of the users
        /// </summary>
        /// <param name="nameFilter"></param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public User[] GetUsers(string nameFilter, string email, UserRole? role, UserStatus? status, int skip, int take)
        {
            var query = mContext.Users
                .AsNoTracking()
                .AsQueryable();
            query = SetupUserListFilters(query, nameFilter, email, role, status)
                .OrderBy(u => u.UserName)
                .Skip(skip).Take(take);
            var entities = query.ToArray();
            return mMapper.Map<User[]>(entities);
        }

        /// <summary>
        /// Gets the number of the users
        /// </summary>
        /// <param name="nameFilter">The beginning of the user name</param>
        /// <param name="email">The complete email</param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int CountUsers(string nameFilter, string email, UserRole? role, UserStatus? status)
        {
            var query = mContext.Users
                .AsNoTracking()
                .AsQueryable();
            query = SetupUserListFilters(query, nameFilter, email, role, status);
            return query.Count();
        }

        /// <summary>
        /// Checks whether the user with such name exists
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public bool IsAnotherUserWithSameNameExists(string userName, int identifier)
        {
            return mContext.Users
                .AsNoTracking()
                .Where(u => u.UserName == userName && u.Id != identifier)
                .Any();
        }
    }
}
