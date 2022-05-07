using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using TicketManagementSystem.Schemas;
using TicketManagementSystem.Helper;
using TicketManagementSystem.Models;

namespace TicketManagementSystem.Controllers
{
    [RoutePrefix("project")]
    public class ProjectController : ApiController
    {
        // Create project
        [HttpPost]
        [Route("create")]
        public string Create(CreateProjectData data)
        {
            // Get JWT token from cookie
            CookieHeaderValue user_name = Request.Headers.GetCookies("user_name").FirstOrDefault();
            CookieHeaderValue session_id = Request.Headers.GetCookies("session_id").FirstOrDefault();

            // Verify JWT token
            if (user_name != null && session_id != null)
            {
                bool verified = JwtHandler.VerifyToken(user_name["user_name"].Value, session_id["session_id"].Value);

                if (verified == false)
                {
                    Response auth_response = new Response();
                    auth_response.success = false;
                    auth_response.data = "Authorization failed";
                    return JsonConvert.SerializeObject(auth_response);
                }
            }

            // Create a new ticket in database
            TicketManagementEntities dbContext = new TicketManagementEntities();
            project ProjectData = new project
            {
                name = data.name
            };
            dbContext.projects.Add(ProjectData);
            dbContext.SaveChanges();

            Response response = new Response
            {
                success = true,
                data = "Project Created"
            };
            return JsonConvert.SerializeObject(response);
        }

        // Get all projects from database
        [HttpGet]
        [Route("")]
        public string Get()
        {
            // Get JWT token from database
            CookieHeaderValue user_name = Request.Headers.GetCookies("user_name").FirstOrDefault();
            CookieHeaderValue session_id = Request.Headers.GetCookies("session_id").FirstOrDefault();

            // Verify JWT token
            if (user_name != null && session_id != null)
            {
                bool verified = JwtHandler.VerifyToken(user_name["user_name"].Value, session_id["session_id"].Value);

                if (verified == false)
                {
                    Response auth_response = new Response();
                    auth_response.success = false;
                    auth_response.data = "Authorization failed";
                    return JsonConvert.SerializeObject(auth_response);
                }
            }

            // Get the project details from database
            TicketManagementEntities dbContext = new TicketManagementEntities();
            var result = from project in dbContext.projects
            select new
            {
                _id = project.id,
                name = project.name
            };

            Response response = new Response();
            response.success = true;
            response.data = JsonConvert.SerializeObject(result.ToArray());
            return JsonConvert.SerializeObject(response);
        }

        // Delete project with given id
        [HttpDelete]
        [Route("{id}")]
        public string Delete(string id)
        {
            // Get JWT token from cookie
            CookieHeaderValue user_name = Request.Headers.GetCookies("user_name").FirstOrDefault();
            CookieHeaderValue session_id = Request.Headers.GetCookies("session_id").FirstOrDefault();

            // Verify JWT token
            if (user_name != null && session_id != null)
            {
                bool verified = JwtHandler.VerifyToken(user_name["user_name"].Value, session_id["session_id"].Value);

                if (verified == false)
                {
                    Response auth_response = new Response();
                    auth_response.success = false;
                    auth_response.data = "Authorization failed";
                    return JsonConvert.SerializeObject(auth_response);
                }

            }

            // Delete the project from database
            TicketManagementEntities dbContext = new TicketManagementEntities();
            System.Diagnostics.Debug.WriteLine(id);
            int Id = Int16.Parse(id);
            var project = dbContext.projects.FirstOrDefault(item => item.id == Id);

            Response response = new Response();
            if (project != null)
            {
                dbContext.projects.Remove(project);
                dbContext.SaveChanges();
                response.success = true;
                response.data = "Project deleted";
            }
            else
            {
                response.success = false;
                response.data = "Project not found";
            }
            return JsonConvert.SerializeObject(response);
        }
    }
}