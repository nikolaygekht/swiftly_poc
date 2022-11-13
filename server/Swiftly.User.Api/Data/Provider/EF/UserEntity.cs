using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Swiftly.User.Api.Data.Provider.EF
{
    /// <summary>
    /// Information about a user
    /// </summary>
    [Table("Swiftly_Users")]
    internal class UserEntity
    {
        /// <summary>
        /// Unique user identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The name of the user
        /// </summary>
        [MaxLength(25)]
        public string UserName { get; set; }

        /// <summary>
        /// The hash of a password
        /// </summary>
        [MaxLength(128)]
        public string PasswordHash { get; set; }

        /// <summary>
        /// The user email
        /// </summary>
        [MaxLength(320)]
        public string Email { get; set; }

        /// <summary>
        /// The status of a user
        /// </summary>
        public UserStatus Status { get; set; }

        /// <summary>
        /// The role of the user
        /// </summary>
        public UserRole Role { get; set; }
    }
}
