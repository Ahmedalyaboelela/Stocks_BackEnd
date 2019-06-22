using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    class SellingOrderModel
    {
        public int SellingOrderID { get; set; }

       
        public string Code { get; set; }

        public int PayWay { get; set; }

        public DateTime? Date { get; set; }

        public int EmployeeID { get; set; }

        public int PortfolioID { get; set; }
        public int Count { get; set; }
    }
}
