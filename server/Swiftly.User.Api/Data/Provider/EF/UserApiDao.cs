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
        /// Saves user
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Save(User user)
        {
            if (user.Id == 0)
            {
                New(user);
                return;
            }

            var entity = mContext.Users.Where(m => m.Id == user.Id).FirstOrDefault();
            if (entity == null)
            {
                New(user);
                return;
            }
            
            mMapper.Map(user, entity);
            mContext.Update(entity);
            mContext.SaveChanges();
        }

        private void New(User user)
        {
            var entity = mMapper.Map<UserEntity>(user);
            mContext.Add(entity);
            mContext.SaveChanges();
            user.Id = entity.Id;
        }

        private static void SetupUserListFilters(IQueryable<UserEntity> query, string nameFilter, string email, UserRole? role, UserStatus? status, int? skip, int? take)
        {
            if (!string.IsNullOrEmpty(nameFilter))
            {
                if (nameFilter.StartsWith('*') && nameFilter.EndsWith('*'))
                    query = query.Where(m => m.UserName.Contains(nameFilter.Substring(1, nameFilter.Length - 2)));
                else if (nameFilter.StartsWith('*'))
                    query = query.Where(m => m.UserName.StartsWith(nameFilter.Substring(1)));
                else if (nameFilter.EndsWith('*'))
                    query = query.Where(m => m.UserName.EndsWith(nameFilter.Substring(0, nameFilter.Length - 1)));
                else
                    query = query.Where(m => m.UserName == nameFilter);
            }
            if (!string.IsNullOrEmpty(email))
                query = query.Where(m => m.Email == email);
            if (role.HasValue)
                query = query.Where(m => m.Role == role.Value);
            if (status.HasValue)
                query = query.Where(m => m.Status == status.Value);
            if (skip != null && take != null)
                query.Skip(skip.Value).Take(take.Value);
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
            SetupUserListFilters(query, nameFilter, email, role, status, skip, take);
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
            SetupUserListFilters(query, nameFilter, email, role, status, null, null);
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
