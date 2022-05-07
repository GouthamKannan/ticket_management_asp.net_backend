using System;
using System.Linq;
using TicketManagementSystem.Models;

namespace TicketManagementSystem.Helper
{
    public class JwtHandler
    {
        static readonly string AllChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";

        // Generate random string of given length
        private static string GenerateRandomString(int Length)
        {
            var random = new Random();
            string Token = new string(
                Enumerable.Repeat(AllChars, Length)
                .Select(token => token[random.Next(token.Length)]).ToArray());

            return Token.ToString();
        }

        // Create random string for JWT token
        public static string CreateToken(string email)
        {
            string token = null;
            TicketManagementEntities dbContext = new TicketManagementEntities();
            var user = dbContext.users.FirstOrDefault();
            while (true)
            {
                token = GenerateRandomString(64);
                user = dbContext.users.FirstOrDefault(cur_user => cur_user.jwt_token == token);
                if(user == null)
                {
                    break;
                }
            }

            // Store the JWT token in database
            user = dbContext.users.FirstOrDefault(cur_user => cur_user.email == email);
            user.jwt_token = token;
            dbContext.SaveChanges();

            return token; 
        }

        // Verify JWT token
        public static bool VerifyToken(string user_name, string token)
        {
            if (user_name == null || token == null)
            {
                return true;
            }

            // Get the stored token in database
            TicketManagementEntities dbContext = new TicketManagementEntities();
            var user = dbContext.users.FirstOrDefault(cur_user => cur_user.user_name == user_name);
            if (user != null)
            {
                if (user.jwt_token == token)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        // Clear JWT token from database (for logout)
        public static void ClearToken(string user_name)
        {
            TicketManagementEntities dbContext = new TicketManagementEntities();
            var user = dbContext.users.FirstOrDefault(cur_user => cur_user.user_name == user_name);
            if (user != null)
            {
                user.jwt_token = null;
                dbContext.SaveChanges();
            }    
        }
    }
}