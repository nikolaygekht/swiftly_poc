using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Swiftly.User.Api.Data;

namespace Swiftly.User.Api
{
    /// <summary>
    /// The user API entity
    /// </summary>
    public class User
    {
        /// <summary>
        /// The user identifier
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// The user name
        /// </summary>
        [JsonPropertyName("name")]
        public string UserName { get; set; }

        /// <summary>
        /// The hash of a password
        /// 
        /// The hash of the password is sha512 hash of the password encoded into UTF-8
        /// and then encoded into base64
        /// </summary>
        [JsonPropertyName("passwordhash")]
        public string PasswordHash { get; set; }

        /// <summary>
        /// The user name
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>
        /// The user role
        /// </summary>
        [JsonPropertyName("role")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; }

        /// <summary>
        /// The user status
        /// </summary>
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserStatus Status { get; set; }
    }
}
