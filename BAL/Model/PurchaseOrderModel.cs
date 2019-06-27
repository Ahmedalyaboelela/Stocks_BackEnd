using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
   public class PurchaseOrderModel
    {
        public int PurchaseOrderID { get; set; }
        
        public string Code { get; set; }

        public int PayWay { get; set; }

        public DateTime? Date { get; set; }

        public int EmployeeID { get; set; }

        public int PortfolioID { get; set; }

        public int PortfolioAccount { get; set; }
        public int Count { get; set; }

        public virtual IEnumerable<PurchaseOrderDetailModel> DetailsModels { get; set; }
        public virtual SettingModel SettingModel { get; set; }
      
    }
}
