using System.Security.Cryptography;
using System.Text;

namespace Vehicle_Parts_Inventory_Management.Helpers
{
    public static class EmailVerificationToken
    {
        public static string GenerateToken()
        {
            // random 32 bytes => URL-safe token
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                .Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        public static string HashToken(string token)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(bytes);
        }
    }
}