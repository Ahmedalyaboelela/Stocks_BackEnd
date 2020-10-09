using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
   public class PurchaseOrderModel
    {
        public int PurchaseOrderID { get; set; }
        
        public string Code { get; set; }

        public string Portfoliocode { get; set; }
        public string OrderDate { get; set; }
        public string OrderDateHijri { get; set; }

        public bool OrderType { get; set; }

        public int EmployeeID { get; set; }
        public string EmpNameAR { get; set; }
        public string EmpNameEN { get; set; }
        public string EmpCode { get; set; }
        public string FromDateGorg { get; set; }
        public string ToDateGorg { get; set; }
        public string FromDateHigri { get; set; }
        public string ToDateHigri { get; set; }

        public int? OrderPeriod { get; set; }
        public string OrderRemarks { get; set; }

        public int PortfolioID { get; set; }
        public string PortfolioNameAR { get; set; }
        public string PortfolioNameEN { get; set; }
        public string PortfolioCode { get; set; }

        public int PortfolioAccount { get; set; }
        public int Count { get; set; }
        public string LastCode { get; set; }
        public string PortfolioAccountName { get; set; }

        public  IEnumerable<PurchaseOrderDetailModel> purchaseordersDetailsModels { get; set; }
   
    }
}
