

using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Web.Helpers;
using System.Linq;
using System.Web.Http;
using System.Net.Http;
using System.Net.Http.Headers;

using TicketManagementSystem.Schemas;
using TicketManagementSystem.Helper;
using TicketManagementSystem.Models;
using System.Net;

namespace TicketManagementSystem.Controllers
{
    [RoutePrefix("user")]
    public class UserController : ApiController
    {
        [HttpPost]
        [Route("login")]
        public string Login(LoginData data)
        {

            // Check if username exists in database
            TicketManagementEntities dbContext = new TicketManagementEntities();
            var user = dbContext.users.FirstOrDefault(cur_user => cur_user.email == data.email);

            if (user != null)
            {
                // Check for password
                if (!Crypto.VerifyHashedPassword(user.password, data.password))
                {
                    Response response = new Response();
                    response.success = false;
                    response.data = "Invalid password";
                    return JsonConvert.SerializeObject(response);
                }
                // Check for account status
                else if (user.status == "inactive")
                {
                    Response response = new Response();
                    response.success = false;
                    response.data = "Email ID not verified";
                    return JsonConvert.SerializeObject(response);
                }
                // Check for admin verification
                else if (user.admin_ver == "false")
                {
                    Response response = new Response();
                    response.success = false;
                    response.data = "Account not verified by admin";
                    return JsonConvert.SerializeObject(response);
                }
                // Create jwt
                else
                {
                    string Token = JwtHandler.CreateToken(data.email);
                    
                    LoginResponse response = new LoginResponse();
                    response.success = true;
                    response.user_name = user.user_name;
                    response.user_type = user.user_type;
                    response.session_id = Token;
                    
                    return JsonConvert.SerializeObject(response);
                }
            }
            else
            {
                Response response = new Response();
                response.success = false;
                response.data = "Invalid Email ID";
                return JsonConvert.SerializeObject(response);
            }
        }

        [HttpPost]
        [Route("signup")]
        public string Signup(SignupData data)
        {
            Response response = new Response();
            // Check if username and email already exists
            TicketManagementEntities dbContext = new TicketManagementEntities();
            var user = dbContext.users.FirstOrDefault(cur_user => cur_user.user_name == data.user_name && cur_user.email == data.email);
            if (user != null)
            {
                response.success = false;
                response.data = "User name or Email Id already exists";
                return JsonConvert.SerializeObject(response);
            }

            // Generate verification code
            var HashedPassword = Crypto.HashPassword(data.password);

            // Send verification code to user's email
            string AbsolutePath = Request.RequestUri.OriginalString;
            string LocalPath = Request.RequestUri.LocalPath;
            string ApiHost = AbsolutePath.Replace(LocalPath, "");
            bool sent = MailHandler.SendVerificationCode(data.user_name, data.email, HashedPassword, data.role, data.user_type,
                                                         ApiHost);

            if (sent)
            {
                response.success = true;
                response.data = "Signup successful";
                return JsonConvert.SerializeObject(response);
            }
            else
            {
                response.success = false;
                response.data = "Cannot send verification email";
                return JsonConvert.SerializeObject(response);
            }
        }

        [HttpGet]
        [Route("verify_email/{ver_code}")]
        public HttpResponseMessage Verify(string ver_code)
        {
            // Get user details using verification code
            TicketManagementEntities dbContext = new TicketManagementEntities();
            var user = dbContext.users.FirstOrDefault(cur_user => cur_user.ver_code == ver_code);

            var redir_response = Request.CreateResponse();
            if (user != null)
            {
                // Update database
                user.ver_code = "";
                user.status = "verified";
                dbContext.SaveChanges();
                string RedirectUrl = ConfigurationManager.AppSettings["UiHost"] + "/login/email_verified";

                // Redirect to login page
                redir_response = Request.CreateResponse(HttpStatusCode.Moved);
                redir_response.Headers.Location = new Uri(RedirectUrl);
                return redir_response;
            }
            Response response = new Response();
            response.success = false;
            response.data = "Invalid Link";

            redir_response = Request.CreateResponse(HttpStatusCode.Found, JsonConvert.SerializeObject(response));
            return redir_response;
            
        }

        [HttpGet]
        [Route("logout")]
        public string Logout()
        {
            // Clear JWT token from database
            CookieHeaderValue user_name = Request.Headers.GetCookies("user_name").FirstOrDefault();
            if (user_name != null)
            {
                JwtHandler.ClearToken(user_name["user_name"].Value);
            }
            Response response = new Response();
            response.success = true;
            response.data = "Logout";
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        [Route("send_reset_link")]
        public string ForgetPassword(ForgetPasswordData data)
        {
            // Get user details based on email
            TicketManagementEntities dbContext = new TicketManagementEntities();
            var user = dbContext.users.FirstOrDefault(cur_user => cur_user.email == data.email);

            if (user != null)
            {
                // Send reset code to user's email
                string AbsolutePath = Request.RequestUri.OriginalString;
                string LocalPath = Request.RequestUri.LocalPath;
                string ApiHost = AbsolutePath.Replace(LocalPath, "");
                bool sent = MailHandler.SendResetCode(data.email, ApiHost);
                if (sent)
                {
                    Response response = new Response();
                    response.success = true;
                    response.data = "Password reset link is sent to registered email ID";
                    return JsonConvert.SerializeObject(response);
                }
                else
                {
                    Response response = new Response();
                    response.success = false;
                    response.data = "Invalid email ID";
                    return JsonConvert.SerializeObject(response);
                }
            }
            else
            {
                Response response = new Response();
                response.success = false;
                response.data = "Email ID not found";
                return JsonConvert.SerializeObject(response);
            }
        }

        [HttpGet]
        [Route("get_reset_form/{reset_code}")]
        public HttpResponseMessage GetResetForm(string reset_code)
        {
            // Check if reset_code exists in database
            TicketManagementEntities dbContext = new TicketManagementEntities();
            var user = dbContext.users.FirstOrDefault(cur_user => cur_user.reset_code == reset_code);

            Response response = new Response();
            if (user != null)
            {
                // Redirect to reset password page
                string RedirectUrl = ConfigurationManager.AppSettings["UiHost"] + "/reset-password/" + reset_code;
                var redir_response = Request.CreateResponse(HttpStatusCode.Moved);
                redir_response.Headers.Location = new Uri(RedirectUrl);
                return redir_response;
            }

            else 
            {
                response.success = false;
                response.data = "Invalid Link";
                var redir_response = Request.CreateResponse(HttpStatusCode.Found, JsonConvert.SerializeObject(response));
                return redir_response;
            }
        }

        [HttpPost]
        [Route("reset_password")]
        public string ResetPassword(ResetPasswordData data)
        {
            Response response = new Response();

            // Get the user details from database
            TicketManagementEntities dbContext = new TicketManagementEntities();
            var user = dbContext.users.FirstOrDefault(cur_user => cur_user.reset_code == data.reset_code && cur_user.email == data.email);

            if (user != null)
            {
                // Save the new password in database
                user.password = Crypto.HashPassword(data.password);
                dbContext.SaveChanges();
                response.success = true;
                response.data = "Password Reset";
                return JsonConvert.SerializeObject(response);
            }
            else
            {
                response.success = false;
                response.data = "Invalid Link";
                return JsonConvert.SerializeObject(response);
            }
        }

        [HttpGet]
        [Route("")]
        public string GetUsers()
        {
            // Get user details from the database
            TicketManagementEntities dbContext = new TicketManagementEntities();
            var result = from user in dbContext.users
            select new
            {
                _id = user.id,
                user_name = user.user_name,
                email = user.email,
                role = user.role,
                user_type = user.user_type,
                status = user.status
            };

            Response response = new Response();
            response.success = true;
            response.data = JsonConvert.SerializeObject(result);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        [Route("approve")]
        public string Approve(ApproveDelete data)
        {
            // Get user details based on id
            TicketManagementEntities dbContext = new TicketManagementEntities();
            int Id = Int16.Parse(data.id);
            var user = dbContext.users.FirstOrDefault(cur_user => cur_user.id == Id);

            if (user != null)
            {
                // Update verification status in database
                user.admin_ver = "true";
                dbContext.SaveChanges();
                return GetUsers();
            }
            else
            {
                Response response = new Response();
                response.success = false;
                response.data = "User not found";
                return JsonConvert.SerializeObject(response);
            }
        }

        [HttpPost]
        [Route("delete")]
        public string Delete(ApproveDelete data)
        {
            // Get user details based on id            
            TicketManagementEntities dbContext = new TicketManagementEntities();
            int Id = Int16.Parse(data.id);
            var user = dbContext.users.FirstOrDefault(cur_user => cur_user.id == Id);

            if (user != null)
            {
                // Remove the user from database
                dbContext.users.Remove(user);
                dbContext.SaveChanges();
                return GetUsers();
            }
            else
            {
                Response response = new Response();
                response.success = false;
                response.data = "User not found";
                return JsonConvert.SerializeObject(response);
            }
        }

    }
}
