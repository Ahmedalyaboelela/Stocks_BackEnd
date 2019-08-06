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
        public bool CheckStockCount(SellingOrderModel sellingOrderModel)
        {
            var PortofolioStocks = unitOfWork.PortfolioTransactionsRepository.Get(filter:m=> m.PortfolioID== sellingOrderModel.PortfolioID);
            var Details = sellingOrderModel.DetailsModels;
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

        // Discount Selling Order Stocks Count From Portofolio
        public void TransferSellingFromStocks(SellingOrderModel sellingOrderModel)
        {
            var PortofolioStocks = unitOfWork.PortfolioTransactionsRepository.Get(filter: m => m.PortfolioID == sellingOrderModel.PortfolioID);
            var Details = sellingOrderModel.DetailsModels;
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
        public void CancelSellingFromStocks(int PortofolioId, IEnumerable<SellingOrderDetail> oldDetils)
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
        public void TransferPurchaseToStocks(PurchaseOrderModel purchaseOrderModel)
        {
            var PortofolioStocks = unitOfWork.PortfolioTransactionsRepository.Get(filter: m => m.PortfolioID == purchaseOrderModel.PortfolioID);
            var Details = purchaseOrderModel.DetailsModels;
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
        public void CancelPurchaseFromStocks(int PortofolioId, IEnumerable<PurchaseOrderDetail> oldDetils)
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


    }
}
