using AutoMapper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.Helper
{
    public class StocksHelper : IStocksHelper
    {
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public StocksHelper(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
        }


        //Check Stocks Count Allowed For Selling 
        public bool CheckStockCountForSelling(SellingOrderModel sellingOrderModel)
        {
            var PortofolioStocks = unitOfWork.PortfolioTransactionsRepository.Get(filter:m=> m.PortfolioID== sellingOrderModel.PortfolioID);
            var Details = sellingOrderModel.sellingOrderDetailModels;
            foreach (var detail in Details)
            {
                if (!PortofolioStocks.Any(m => m.PartnerID == detail.PartnerID))
                {
                    return false;
                }
                else
                {
                    foreach (var item in PortofolioStocks)
                    {
                        if (detail.PartnerID == item.PartnerID)
                        {
                            if (detail.StockCount > item.CurrentStocksCount)
                            {
                                return false;
                            }


                        }
                    }
         
                }
            }
            return true; 
        }

        //Rial Balanc of portfolio 
        public decimal? RialBalanc(int PortfolioID)
        {
            var accountid = unitOfWork.PortfolioAccountRepository.GetEntity(filter: x => x.PortfolioID == PortfolioID).AccountID;
            var account = unitOfWork.AccountRepository.GetEntity(filter: x => x.AccountID == accountid);
            decimal? Debit = account.Debit;
            decimal? Credit = account.Credit;
            decimal? DebitOpenningBalance = account.DebitOpenningBalance;
            decimal? CreditOpenningBalance = account.CreditOpenningBalance;
            decimal? RealBalance = 0.0m;
            if (Debit == null)
            {
                Debit = 0.0m;
            }
            if (Credit == null)
            {
                Credit = 0.0m;
            }
            if (DebitOpenningBalance == null && CreditOpenningBalance != null)
            {
                RealBalance = -CreditOpenningBalance + (Debit - Credit);
               
            }
            else if (DebitOpenningBalance != null && CreditOpenningBalance == null)
            {
                RealBalance = DebitOpenningBalance + (Debit -Credit);
               
            }
            else if (DebitOpenningBalance == null && CreditOpenningBalance == null)
            {
                RealBalance = Debit - Credit;
               
            }
            else if (DebitOpenningBalance != null && CreditOpenningBalance != null)
            {
               RealBalance = DebitOpenningBalance + (Debit - Credit);
               
            }


            return RealBalance;
        }
        //Check Stocks Count Allowed For Selling Invoice  on selling Order
        public bool CheckStockCountForSellingInvoice(SellingInvoiceModel sellingInvoiceModel)
        {
            var PortofolioStocks = unitOfWork.SellingOrderDetailRepository.Get(filter: m => m.SellingOrder.PortfolioID == sellingInvoiceModel.PortfolioID);
            var Details = sellingInvoiceModel.DetailsModels;
            foreach (var detail in Details)
            {
                if (PortofolioStocks.Any(m => m.PartnerID == detail.PartnerID && detail.StockCount <= m.StockCount))
                {
                    return true;
                }
                else
                {
                    return false;

                }
            }
            return true;
        }



        // Discount Selling Order Stocks Count From Portofolio
        public void TransferSellingFromStocks(SellingInvoiceModel sellingInvoiceModel)
        {
            var PortofolioStocks = unitOfWork.PortfolioTransactionsRepository.Get(filter: m => m.PortfolioID == sellingInvoiceModel.PortfolioID);
            var Details = sellingInvoiceModel.DetailsModels;
            foreach (var detail in Details)
            {
                    foreach (var item in PortofolioStocks)
                    {
                        if (detail.PartnerID == item.PartnerID)
                        {
                                item.CurrentStocksCount = item.CurrentStocksCount - detail.StockCount;
                                unitOfWork.PortfolioTransactionsRepository.Update(item);
                        }

                    }
            }
        }

        //Cancel Selling Order From Portofolio Stocks
        public void CancelSellingFromStocks(int PortofolioId, IEnumerable<SellingInvoiceDetail> oldDetils)
        {
            var PortofolioStocks = unitOfWork.PortfolioTransactionsRepository.Get(filter: m => m.PortfolioID == PortofolioId);
            foreach (var detail in oldDetils)
            {
                foreach (var item in PortofolioStocks)
                {
                    if (detail.PartnerID == item.PartnerID)
                    {
                        item.CurrentStocksCount = item.CurrentStocksCount + detail.StockCount;
                        unitOfWork.PortfolioTransactionsRepository.Update(item);
                    }

                }
            }

        }

        // Add Purchase Order Stocks Count To Portofolio
        public void TransferPurchaseToStocks(PurchaseInvoiceModel purchaseInvoiceModel)
        {
            var PortofolioStocks = unitOfWork.PortfolioTransactionsRepository.Get(filter: m => m.PortfolioID == purchaseInvoiceModel.PortfolioID);
            var Details = purchaseInvoiceModel.DetailsModels;
            foreach (var detail in Details)
            {
                foreach (var item in PortofolioStocks)
                {
                    if (detail.PartnerID == item.PartnerID)
                    {
                        item.CurrentStocksCount = item.CurrentStocksCount + detail.StockCount;
                        item.CurrentStockValue = (((decimal)item.CurrentStocksCount *  item.CurrentStockValue) + 
                        ((decimal)detail.StockCount *  detail.PurchaseValue))
                         /(decimal) (item.CurrentStocksCount + detail.StockCount);
                        unitOfWork.PortfolioTransactionsRepository.Update(item);
                    }

                }
            }
        }

        //Cancel Purchase Order From Portofolio Stocks
        public void CancelPurchaseFromStocks(int PortofolioId, IEnumerable<PurchaseInvoiceDetail> oldDetils)
        {
            var PortofolioStocks = unitOfWork.PortfolioTransactionsRepository.Get(filter: m => m.PortfolioID == PortofolioId);
            foreach (var detail in oldDetils)
            {
                foreach (var item in PortofolioStocks)
                {
                    if (detail.PartnerID == item.PartnerID)
                    {
                        item.CurrentStocksCount = item.CurrentStocksCount - detail.StockCount;
                        item.CurrentStockValue = ((item.CurrentStockValue * (decimal)(item.CurrentStocksCount + detail.StockCount))
                            - ((decimal)detail.StockCount * detail.PurchaseValue))
                            / (decimal)item.CurrentStocksCount;
                        unitOfWork.PortfolioTransactionsRepository.Update(item);
                    }

                }
            }

        }

        //Check Stocks Count Allowed For Notice Credit 
        public bool CheckStockCountForNCredit(NoticeModel noticeModel)
        {
            var PortofolioStocks = unitOfWork.PortfolioTransactionsRepository.Get(filter: m => m.PortfolioID == noticeModel.PortfolioID);
            var Details = noticeModel.NoticeModelDetails;
            foreach (var detail in Details)
            {
                if (!PortofolioStocks.Any(m => m.PartnerID == detail.PartnerID))
                {
                    return false;
                }
                else
                {
                    foreach (var item in PortofolioStocks)
                    {
                        if (detail.PartnerID == item.PartnerID)
                        {
                            if(detail.StocksCredit != null)
                            {
                                if (detail.StocksCredit > item.CurrentStocksCount)
                                {
                                    return false;
                                }

                            }


                        }
                    }

                }
            }
            return true;
        }


        // Discount Notice Credit Stocks Count From Portofolio
        public void TransferNCreditFromStocks(NoticeModel noticeModel)
        {
            var PortofolioStocks = unitOfWork.PortfolioTransactionsRepository.Get(filter: m => m.PortfolioID == noticeModel.PortfolioID);
            var Details = noticeModel.NoticeModelDetails;
            foreach (var detail in Details)
            {
                foreach (var item in PortofolioStocks)
                {
                    if (detail.PartnerID == item.PartnerID)
                    {
                        if( detail.StocksCredit != null)
                        {
                            item.CurrentStocksCount = (float)(item.CurrentStocksCount - detail.StocksCredit);

                        }
                        unitOfWork.PortfolioTransactionsRepository.Update(item);
                    }

                }
            }
        }

        //Cancel Notice Credit  From Portofolio Stocks
        public void CancelNCreditFromStocks(int PortofolioId, IEnumerable<NoticeDetail> oldDetils)
        {
            var PortofolioStocks = unitOfWork.PortfolioTransactionsRepository.Get(filter: m => m.PortfolioID == PortofolioId);
            foreach (var detail in oldDetils)
            {
                foreach (var item in PortofolioStocks)
                {
                    if (detail.PartnerID == item.PartnerID)
                    {
                        if (detail.StocksCredit != null)
                        {

                            item.CurrentStocksCount = (float)(item.CurrentStocksCount + detail.StocksCredit);
                            unitOfWork.PortfolioTransactionsRepository.Update(item);
                        }
                    }

                }
            }

        }

        // Add Purchase Notice Debit Stocks Count To Portofolio
        public void TransferNDebitToStocks(NoticeModel noticeModel)
        {
            var PortofolioStocks = unitOfWork.PortfolioTransactionsRepository.Get(filter: m => m.PortfolioID == noticeModel.PortfolioID);
            var Details = noticeModel.NoticeModelDetails;
            foreach (var detail in Details)
            {
                foreach (var item in PortofolioStocks)
                {
                    if (detail.PartnerID == item.PartnerID)
                    {
                        if (detail.StocksDebit != null)
                        {

                            item.CurrentStocksCount = (float)(item.CurrentStocksCount + detail.StocksDebit);
                            unitOfWork.PortfolioTransactionsRepository.Update(item);
                        }
                    }

                }
            }
        }

        //Cancel Notice Debit From Portofolio Stocks
        public void CancelNDebitFromStocks(int PortofolioId, IEnumerable<NoticeDetail> oldDetils)
        {
            var PortofolioStocks = unitOfWork.PortfolioTransactionsRepository.Get(filter: m => m.PortfolioID == PortofolioId);
            foreach (var detail in oldDetils)
            {
                foreach (var item in PortofolioStocks)
                {
                    if (detail.PartnerID == item.PartnerID)
                    {
                        if (detail.StocksDebit != null)
                        {

                            item.CurrentStocksCount = (float)(item.CurrentStocksCount - detail.StocksDebit);
                            unitOfWork.PortfolioTransactionsRepository.Update(item);
                        }
                    }

                }
            }

        }




    }
}
