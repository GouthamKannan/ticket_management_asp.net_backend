using System;
using System.Linq;
using System.Web.Http;
using System.Net.Http;
using System.Net.Http.Headers;

using TicketManagementSystem.Schemas;
using TicketManagementSystem.Helper;
using TicketManagementSystem.Models;
using Newtonsoft.Json;

namespace TicketManagementSystem.Controllers
{
    [RoutePrefix("ticket")]
    public class TicketController : ApiController
    {
        // Get all tickets from database
        [HttpGet]
        [Route("")]
        public string GetTickets()
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
            
            // Get the ticket details from database
            TicketManagementEntities dbContext = new TicketManagementEntities();
            var result = from ticket in dbContext.tickets
            select new
            {
                _id = ticket.id,
                title = ticket.title,
                description = ticket.description,
                projectName = ticket.projectName,
                assignee = ticket.assignee,
                priority = ticket.priority,
                status = ticket.status,
                type = ticket.type
            };

            Response response = new Response();
            response.success = true;
            response.data = JsonConvert.SerializeObject(result.ToArray());
            return JsonConvert.SerializeObject(response);
        }

        // Create a new ticket
        [HttpPost]
        [Route("create")]
        public string CreateTicket(CreateUpdateTicketData data)
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
            ticket TicketData = new ticket
            {
                title = data.title,
                description = data.description,
                projectName = data.projectName,
                assignee = data.assignee,
                priority = data.priority,
                status = data.status,
                type = data.type
            };
            dbContext.tickets.Add(TicketData);
            dbContext.SaveChanges();

            Response response = new Response
            {
                success = true,
                data = "Ticket Created"
            };
            return JsonConvert.SerializeObject(response);
        }

        // Update ticket with given id
        [HttpPost]
        [Route("update/{id}")]
        public string UpdateTicket(string id, CreateUpdateTicketData data)
        {
            // Get JWT token from cookie
            CookieHeaderValue user_name = Request.Headers.GetCookies("user_name").FirstOrDefault();
            CookieHeaderValue session_id = Request.Headers.GetCookies("session_id").FirstOrDefault();

            // Verify JWT Token
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

            // Get the ticket with given id
            TicketManagementEntities dbContext = new TicketManagementEntities();
            int Id = Int16.Parse(id);
            var ticket = dbContext.tickets.FirstOrDefault(cur_ticket => cur_ticket.id == Id);

            // Update the ticket details
            Response response = new Response();
            if (ticket != null)
            {
                ticket.title = data.title;
                ticket.description = data.description;
                ticket.projectName = data.projectName;
                ticket.assignee = data.assignee;
                ticket.priority = data.priority;
                ticket.status = data.status;
                ticket.type = data.type;

                dbContext.SaveChanges();

                response.success = true;
                response.data = "Updated ticket";
            }
            else
            {
                response.success = false;
                response.data = "Ticket not found";
            }

            return JsonConvert.SerializeObject(response);
        }

        // Get ticket details with given id
        [HttpGet]
        [Route("{id}")]
        public string GetTicket(string id)
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

            // Get the ticket details from database
            TicketManagementEntities dbContext = new TicketManagementEntities();
            int Id = Int16.Parse(id);
            System.Diagnostics.Debug.WriteLine(Id);
            var ticket = dbContext.tickets.FirstOrDefault(item => item.id == Id);

            TicketData Ticket = new TicketData
            {
                _id = ticket.id,
                title = ticket.title,
                description = ticket.description,
                projectName = ticket.projectName,
                assignee = ticket.assignee,
                priority = ticket.priority,
                status = ticket.status,
                type = ticket.type
            };

            Response response = new Response();
            if (ticket != null)
            {
                response.success = true;
                response.data = JsonConvert.SerializeObject(Ticket);
            }
            else
            {
                response.success = false;
                response.data = "Ticket not found";
            }
            return JsonConvert.SerializeObject(response);
        }

        // Delete the ticket with given id
        [HttpDelete]
        [Route("{id}")]
        public string DeleteTicket(string id)
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

            // Get the ticket details from database
            TicketManagementEntities dbContext = new TicketManagementEntities();
            int Id = Int16.Parse(id);
            var ticket = dbContext.tickets.FirstOrDefault(item => item.id == Id);

            // Delete the ticket from database
            Response response = new Response();
            if (ticket != null)
            {
                dbContext.tickets.Remove(ticket);
                dbContext.SaveChanges();
                response.success = true;
                response.data = "Ticket deleted";
            }
            else
            {
                response.success = false;
                response.data = "Ticket not found";
            }
            return JsonConvert.SerializeObject(response);
        }
        
    }
}