
using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
public class UserLogModel
    {
        public int UserLogID { get; set; }

       
        public string PageName { get; set; }
        
        public string OperationName { get; set; }

        public bool MobileView { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string OperationDate { get; set; }
        public string time { get; set; }
      


    }
}
