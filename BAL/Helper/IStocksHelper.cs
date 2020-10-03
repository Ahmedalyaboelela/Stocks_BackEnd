using BAL.Model;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Helper
{
    public interface IStocksHelper
    {
        decimal? RialBalanc(int PortfolioID);
        decimal? RialBalancUpdate(int PortfolioID, decimal? OldNetAmount);
        float? sumOfstocksOnInvoice(int sellOrderID, int partenerID);
        float? sumOfstocksOnInvoiceUpdate(int sellOrderID, int partenerID, int InVoiceDetailsID);
        bool CheckStockCountForSelling(SellingOrderModel sellingOrderModel);
         bool CheckStockCountForSellingInvoice(SellingInvoiceModel sellingInvoiceModel);
        void TransferSellingFromStocks(SellingInvoiceModel sellingInvoiceModel);

        void CancelSellingFromStocks(int PortofolioId, IEnumerable<SellingInvoiceDetail> oldDetils);

        void TransferPurchaseToStocks(PurchaseInvoiceModel purchaseInvoiceModel);

        void CancelPurchaseFromStocks(int PortofolioId, IEnumerable<PurchaseInvoiceDetail> oldDetils);

        bool CheckStockCountForNCredit(NoticeModel noticeModel);

        void TransferNCreditFromStocks(NoticeModel noticeModel);

        void CancelNCreditFromStocks(int PortofolioId, IEnumerable<NoticeDetail> oldDetils);

        void TransferNDebitToStocks(NoticeModel noticeModel);

        void CancelNDebitFromStocks(int PortofolioId, IEnumerable<NoticeDetail> oldDetils);

    }
}
