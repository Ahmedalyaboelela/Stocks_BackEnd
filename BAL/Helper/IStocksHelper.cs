using BAL.Model;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Helper
{
    public interface IStocksHelper
    {
        bool CheckStockCountForSelling(SellingOrderModel sellingOrderModel);
        void TransferSellingFromStocks(SellingOrderModel sellingOrderModel);

        void CancelSellingFromStocks(int PortofolioId, IEnumerable<SellingOrderDetail> oldDetils);

        void TransferPurchaseToStocks(PurchaseOrderModel purchaseOrderModel);

        void CancelPurchaseFromStocks(int PortofolioId, IEnumerable<PurchaseOrderDetail> oldDetils);

        bool CheckStockCountForNCredit(NoticeModel noticeModel);

        void TransferNCreditFromStocks(NoticeModel noticeModel);

        void CancelNCreditFromStocks(int PortofolioId, IEnumerable<NoticeDetail> oldDetils);

        void TransferNDebitToStocks(NoticeModel noticeModel);

        void CancelNDebitFromStocks(int PortofolioId, IEnumerable<NoticeDetail> oldDetils);

    }
}
