
namespace TicketManagementSystem.Schemas
{
    public class CreateProjectData
    {
        public string name { get; set; }
    }

    public class TicketData
    {
        public int _id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string projectName { get; set; }
        public string assignee { get; set; }
        public string priority { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }

    public class CreateUpdateTicketData
    {
        public string title { get; set; }
        public string description { get; set; }
        public string projectName { get; set; }
        public string assignee { get; set; }
        public string priority { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }

    public class SignupData
    {
        public string user_name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public string user_type { get; set; }
    }

    public class LoginData
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class ForgetPasswordData
    {
        public string email { get; set; }
    }

    public class ResetPasswordData
    {
        public string email { get; set; }
        public string password { get; set; }
        public string reset_code { get; set; }
    }

    public class ApproveDelete
    {
        public string id { get; set; }
    }

    public class Response
    {
        public bool success { get; set; }
        public string data { get; set; }
    }

    public class LoginResponse
    {
        public bool success { get; set; }
        public string user_name { get; set; }
        public string user_type { get; set; }
        public string session_id { get; set; }
    }
}