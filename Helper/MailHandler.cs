using System;
using System.Linq;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using TicketManagementSystem.Models;


namespace TicketManagementSystem.Helper
{
    public class MailHandler
    {
        static string AllChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";

        // Create an SMTP client to sent email
        private static SmtpClient CreateSmtpClient()
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["FromMail"],
                                                       ConfigurationManager.AppSettings["FromPassword"]);
            client.Host = "smtp.gmail.com";

            return client;
        }

        static SmtpClient client = CreateSmtpClient();

        // Generate random string of given length
        private static string GenerateRandomString(int Length)
        {
            var random = new Random();
            string Token = new string(
                Enumerable.Repeat(AllChars, Length)
                .Select(token => token[random.Next(token.Length)]).ToArray());

            return Token.ToString();
        }

        // Send mail with given subject and mail body to given mail address
        private static bool SendMail(string ToMail, string Subject, string MailBody)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(ConfigurationManager.AppSettings["FromMail"]);
            message.To.Add(new MailAddress(ToMail));
            message.IsBodyHtml = true;
            message.Body = MailBody;
            message.Subject = Subject;

            try
            {
                // Send mail
                client.Send(message);
                System.Diagnostics.Debug.WriteLine("Sent Mail");
                return true;
            }

            // Handle Smtp Exception
            catch(System.Net.Mail.SmtpException ex)
            {
                System.Diagnostics.Debug.WriteLine("Smtp exception");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return false;
            }

            // Handle other exceptions
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return false;
            }

        }

        // Generate and send verification code
        public static bool SendVerificationCode(string UserName, string Email, string Password, string Role, string UserType, 
                                                string RequestUrl)
        {
            // Initialize Email contents
            string ver_code = GenerateRandomString(8);
            string link = RequestUrl + "/user/verify_email/" + ver_code;
            string body = "Hello, Please Click on the link to verify your email. <a href=" + link + ">Click here to verify</a>";
            string subject = "Email verification link";

            // Send email
            bool sent = SendMail(Email, subject, body);

            if(sent)
            {
                // Store the user details in database
                TicketManagementEntities dbContext = new TicketManagementEntities();
                user User = new user
                {
                    user_name = UserName,
                    email = Email,
                    password = Password,
                    role = Role,
                    user_type = UserType,
                    ver_code = ver_code,
                    status = "inactive",
                    admin_ver = "false",
                    reset_code = null,
                    jwt_token = null
                };
                dbContext.users.Add(User);
                dbContext.SaveChanges();

                return true;
            }
            else
            {
                return false;
            }
        }

        // Generate and send password reset code to user's email
        public static bool SendResetCode(string email, string request_url)
        {
            // Initalize Email details
            string reset_code = GenerateRandomString(8);
            string link = request_url + "/user/get_reset_form/" + reset_code;
            string body = "Hello, Please Click on the link to reset your password. <a href=" + link + ">Click here to reset</a>";
            string subject = "Password reset link";
            
            // Send email
            bool sent = SendMail(email, subject, body);

            if(sent)
            {
                // Save reset_code in database
                TicketManagementEntities dbContext = new TicketManagementEntities();
                var user = dbContext.users.FirstOrDefault(cur_user => cur_user.email == email);
                if (user != null)
                {
                    user.reset_code = reset_code;
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}