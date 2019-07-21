using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    public class ApplicationUserModel
    {

        public string Id { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string FullName { get; set; }

        public DateTime CreationDate { get; set; }

        public int Count { get; set; }
    }
}
