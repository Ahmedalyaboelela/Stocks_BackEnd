﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    public class PurchaseInvoiceModel
    {
        public int PurchaseInvoiceID { get; set; }

        public string Code { get; set; }


        public string PurchaseDate { get; set; }
        public string PurchaseDateHijri { get; set; }


        public int EmployeeID { get; set; }
        public string EmpNameAR { get; set; }
        public string EmpNameEN { get; set; }
        public string EmpCode { get; set; }

        
        public int PortfolioID { get; set; }
        public string PortfolioNameAR { get; set; }
        public string PortfolioNameEN { get; set; }
        public string PortfolioCode { get; set; }

        public int PurchaseOrderID { get; set; }
        public string PurchaseOrderCode { get; set; }
        public int PortfolioAccount { get; set; }
        public int Count { get; set; }

        public string Codepurchase { get; set; }
        public string LastCode { get; set; }

        public float TotalStockCount { get; set; }
        public decimal? newRialBalance { get; set; }

        public IEnumerable<PurchaseInvoiceDetailModel> DetailsModels { get; set; }
        public SettingModel SettingModel { get; set; }
        public EntryModel EntryModel { get; set; }

    }
}
