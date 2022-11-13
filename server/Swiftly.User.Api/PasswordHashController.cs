using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Swiftly.User.Api
{
    /// <summary>
    /// The controller to create a hash value
    /// </summary>
    public static class PasswordHashController
    {
        /// <summary>
        /// Gets hash value
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetHash(string password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));
            
            var rawPassword = Encoding.UTF8.GetBytes(password);
            var rawHash = SHA512.HashData(rawPassword);
            return Convert.ToBase64String(rawHash);
        }
    }
}
