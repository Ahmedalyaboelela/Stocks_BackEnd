using BAL.Model;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Helper
{
    public interface IStocksHelper
    {
        bool CheckStockCount(SellingOrderModel sellingOrderModel);
        void TransferSellingFromStocks(SellingOrderModel sellingOrderModel);

        void CancelSellingFromStocks(int PortofolioId, IEnumerable<SellingOrderDetail> oldDetils);

    }
}
