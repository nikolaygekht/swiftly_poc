using System;
using System.Runtime.Serialization;

namespace Swiftly.User.Api
{
    /// <summary>
    /// Exception in user API
    /// </summary>
    [Serializable]
    public class UserApiException : Exception 
    {
        public string Operation { get; }
        public string OperationMessage { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="message"></param>
        public UserApiException(string operation, string message)
            : base($"User API error {message} in operation {operation}")
        {
            Operation = operation;
            OperationMessage = message;
        }
    

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="message"></param>
        public UserApiException(string operation, Exception innerException)
                : base($"Exception {innerException.GetType().Name}({innerException.Message}) in operation {operation}", innerException)
        {
            Operation = operation;
            OperationMessage = $"Exception {innerException.GetType().Name}({innerException.Message})";
        }

        protected UserApiException(SerializationInfo info, StreamingContext context)
                : base(info, context)
        {
            Operation = info.GetString("Operation");
            OperationMessage = info.GetString("OperationMessage");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Operation", Operation);
            info.AddValue("OperationMessage", OperationMessage);
        }
    }
}
