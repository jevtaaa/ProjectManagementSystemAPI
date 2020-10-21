using ProjectManagementSystemAPI.Data.Services;
using System;
using System.Security.Cryptography;
using System.Text;

namespace ProjectManagementSystemAPI.Services
{
    public class PasswordService : IPasswordService
    {
        public string ComputePasswordHash(string password)
        {
            using (var sha1 = SHA1.Create())
            {
                byte[] passwordRaw = Encoding.ASCII.GetBytes(password);

                var sha1Data = sha1.ComputeHash(passwordRaw);

                return Encoding.ASCII.GetString(sha1Data);
            }
        }
    }
}
