//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TicketManagementSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class user
    {
        public string user_name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public string user_type { get; set; }
        public string ver_code { get; set; }
        public string status { get; set; }
        public string admin_ver { get; set; }
        public string reset_code { get; set; }
        public string jwt_token { get; set; }
        public int id { get; set; }
    }
}
