using System.Security.Cryptography;
using System.Text;

namespace Common.Application
{
    public static class SystemIdGenerator
    {
        /// <summary>
        /// This method generates a unique system id based on the input string
        /// using the MD5 hashing algorithm.
        /// </summary>
        public static Guid GenerateId(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                
                return new Guid(hash);
            }
        }
    }
}
