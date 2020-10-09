using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
 public   class SellingOrderModel
    {
        public int SellingOrderID { get; set; }

      
        public string Code { get; set; }

        //  public DateTime OrderDate { get; set; }
        public string OrderDateGorg { get; set; }
        public string OrderDateHigri { get; set; }

        public bool OrderType { get; set; }
        public int EmployeeID { get; set; }

        public int PortfolioID { get; set; }
        public string PortfolioNameAR { get; set; }
        public string Portfoliocode { get; set; }
        public string FromDateGorg { get; set; }
        public string ToDateGorg { get; set; }
        public string FromDateHigri { get; set; }
        public string ToDateHigri { get; set; }

        public int? OrderPeriod { get; set; }
        public string OrderRemarks { get; set; }

        public int Count { get; set; }
        public string LastCode { get; set; }
        public int PortfolioAccount { get; set; }
        public string PortfolioAccountName { get; set; }
        public IEnumerable<SellingOrderDetailModel> sellingOrderDetailModels { get; set; }


    }
    public class partenerCodeModel {
        public string Code { get; set; }
        public string NameAr { get; set; }

    }
}
