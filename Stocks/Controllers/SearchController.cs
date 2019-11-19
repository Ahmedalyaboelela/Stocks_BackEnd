using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.Helper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Stocks.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin,Employee")]
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : Controller
    {
      
        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public SearchController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
        }
        #endregion

        #region Get Receipt Exchange

        public ReceiptExchangeModel GetReceiptExchange(ReceiptExchange RecExc, bool ReceiptExchangeType, bool type, int num)
        {
            var model = _mapper.Map<ReceiptExchangeModel>(RecExc);
            if (model == null)
            {
                return model;
            }
            model.BankName = RecExc.BankName;
            model.ChiqueDate = RecExc.ChiqueDate != null ? RecExc.ChiqueDate.Value.ToString("d/M/yyyy") : null;
            model.ChiqueDateHijri = RecExc.ChiqueDate != null ? DateHelper.GetHijriDate(RecExc.ChiqueDate) : null;
            model.ChiqueNumber = RecExc.ChiqueNumber;
            model.Code = RecExc.Code;
            model.Date = RecExc.Date != null ? RecExc.Date.Value.ToString("d/M/yyyy") : null;
            model.DateHijri = RecExc.Date != null ? DateHelper.GetHijriDate(RecExc.Date) : null;
            model.Description = RecExc.Description;
            model.Handling = RecExc.Handling;
            model.ReceiptExchangeType = RecExc.ReceiptExchangeType;
            model.ReceiptID = RecExc.ReceiptID;
            model.Type = RecExc.Type;
            model.RecieptValue = RecExc.RecieptValue;
            model.TaxNumber = RecExc.TaxNumber;
            model.Count = unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type && m.ReceiptExchangeType == ReceiptExchangeType).Count();
            model.EntryModel = ReceiptExchangeGetEntry(RecExc.ReceiptID);
            model.RecExcDetails = unitOfWork.ReceiptExchangeDetailRepository

                   .Get(filter: m => m.ReceiptID == RecExc.ReceiptID) != null ? unitOfWork.ReceiptExchangeDetailRepository.Get(filter: m => m.ReceiptID == RecExc.ReceiptID)
                   .Select(m => new ReceiptExchangeDetailModel
                   {
                       ReceiptExchangeID = m.ReceiptExchangeID,
                       ReceiptID = m.ReceiptID,
                       AccountID = m.AccountID,
                       AccNameAR = m.Account.NameAR,
                       AccNameEN = m.Account.NameEN,
                       AccCode = m.Account.Code,
                       DetailType = m.DetailType,
                       Debit = m.Debit,
                       Credit = m.Credit,


                   }) : null;
            model.LastCode = unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type && m.ReceiptExchangeType == ReceiptExchangeType) != null ? unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type && m.ReceiptExchangeType == ReceiptExchangeType).FirstOrDefault().Code : null;
            model.SettingModel = GetSetting(num);



            return model;
        }
        #endregion

        #region GetNotice
        public EntryModel ReceiptExchangeGetEntry(int receiptID)
        {
            var Entry = unitOfWork.EntryRepository.Get(x => x.ReceiptID == receiptID).FirstOrDefault();
            EntryModel entryModel = new EntryModel();
            if (Entry != null)
            {
                entryModel.EntryID = Entry.EntryID;
                entryModel.Code = Entry.Code;
                entryModel.Date = Entry.Date != null ? Entry.Date.Value.ToString("d/M/yyyy") : null;
                entryModel.DateHijri = Entry.Date != null ? DateHelper.GetHijriDate(Entry.Date) : null;
                entryModel.NoticeID = Entry.NoticeID;
                entryModel.PurchaseInvoiceID = Entry.PurchaseInvoiceID;
                entryModel.ReceiptID = Entry.ReceiptID;
                entryModel.SellingInvoiceID = Entry.SellingInvoiceID;
                entryModel.EntryDetailModel = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID) != null ? unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID).Select(m => new EntryDetailModel
                {
                    AccCode = m.Account.Code,
                    AccNameAR = m.Account.NameAR,
                    AccNameEN = m.Account.NameEN,
                    AccountID = m.AccountID,
                    Credit = m.Credit,
                    Debit = m.Debit,
                    EntryDetailID = m.EntryDetailID,
                    EntryID = m.EntryID,


                }) : null;
                entryModel.TransferedToAccounts = Entry.TransferedToAccounts;

            }

            return entryModel;
        }
        public EntryModel GetEntry(int noticeID)
        {
            var Entry = unitOfWork.EntryRepository.Get(x => x.NoticeID == noticeID).SingleOrDefault();
            EntryModel entryModel = new EntryModel();
            if (Entry != null)
            {   var EntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
                
                entryModel.EntryID = Entry.EntryID;
                entryModel.Code = Entry.Code;
                entryModel.Date = Entry.Date != null ? Entry.Date.Value.ToString("d/M/yyyy") : null;
                entryModel.DateHijri = Entry.Date != null ? DateHelper.GetHijriDate(Entry.Date) : null;
                entryModel.NoticeID = Entry.NoticeID;
                entryModel.PurchaseInvoiceID = Entry.PurchaseInvoiceID;
                entryModel.ReceiptID = Entry.ReceiptID;
                entryModel.SellingInvoiceID = Entry.SellingInvoiceID;
                entryModel.EntryDetailModel = EntryDetails.Select(m => new EntryDetailModel
                {
                    AccCode = m.Account.Code,
                    AccNameAR = m.Account.NameAR,
                    AccNameEN = m.Account.NameEN,
                    AccountID = m.AccountID,
                    ParentAccountID = m.Account.AccoutnParentID,
                    ParentAccCode = unitOfWork.AccountRepository.Get(filter: a => a.AccountID == m.Account.AccoutnParentID).Select(s => s.Code).FirstOrDefault(),
                    ParentAccNameAR = unitOfWork.AccountRepository.Get(filter: a => a.AccountID == m.Account.AccoutnParentID).Select(s => s.NameAR).FirstOrDefault(),
                    Credit = m.Credit,
                    StocksCredit = m.StocksCredit,
                    Debit = m.Debit,
                    StocksDebit = m.StocksDebit,
                    EntryDetailID = m.EntryDetailID,
                    EntryID = m.EntryID,


                });
                entryModel.TransferedToAccounts = Entry.TransferedToAccounts;
            }
            return entryModel;
        }

        public IEnumerable<SettingAccountModel> SettingAccounts(int id)
        {

            var setAccounts = unitOfWork.SettingAccountRepository.Get(filter: x => x.SettingID == id).Select(a => new SettingAccountModel
            {

                SettingID = a.SettingID,
                AccNameEN = a.Account.NameEN,
                AccCode = a.Account.Code,
                AccNameAR = a.Account.NameAR,
                AccountID = a.AccountID,
                AccountType = a.AccountType,
                SettingAccountID = a.SettingAccountID,
                Code = a.Setting.Code,



            });



            return setAccounts;


        }

        public SettingModel GetSetting(int type)
        {

            var setsetting = unitOfWork.SettingRepository.Get(filter: x => x.VoucherType == type).Select(a => new SettingModel
            {

                SettingID = a.SettingID,
                VoucherType = 2,
                AutoGenerateEntry = a.AutoGenerateEntry,
                Code = a.Code,
                DoNotGenerateEntry = a.DoNotGenerateEntry,
                GenerateEntry = a.GenerateEntry,
                TransferToAccounts = a.TransferToAccounts,
                SettingAccs = SettingAccounts(a.SettingID),

            }).SingleOrDefault();
            return setsetting;


        }

        public NoticeModel GetNotice(Notice notice, bool type)
        {
            var model = _mapper.Map<NoticeModel>(notice);
            if (model == null)
            {
                return model;
            }

            #region Date part
            if (notice.NoticeDate != null)
            {
                model.NoticeDate = notice.NoticeDate.Value.ToString("d/M/yyyy");
                model.NoticeDateHijri = DateHelper.GetHijriDate(notice.NoticeDate);
            }
            if (notice.DistributionDate != null)
            {
                model.DistributionDate = notice.DistributionDate.Value.ToString("d/M/yyyy");
                model.DistributionDateHijri = DateHelper.GetHijriDate(notice.DistributionDate);
            }
            #endregion

            #region Details part
            var Details = unitOfWork.NoticeDetailRepository

                .Get(filter: m => m.NoticeID == notice.NoticeID)
                .Select(m => new NoticeDetailModel
                {
                    NoticeDetailID = m.NoticeDetailID,
                    NoticeID = m.NoticeID,
                    Debit = m.Debit,
                    Credit = m.Credit,
                    StocksCredit = m.StocksCredit,
                    StocksDebit = m.StocksDebit,
                    AccountID = m.AccountID,
                    AccCode = m.Account.Code,
                    AccNameAR = m.Account.NameAR,
                    AccNameEN = m.Account.NameEN,
                    PartnerID = m.PartnerID != null && m.PartnerID > 0 ? m.PartnerID : 0,
                    PartnerName = m.PartnerID != null && m.PartnerID > 0 ? m.Partner.NameAR : null
                });
            if (Details != null)
                model.NoticeModelDetails = Details;

            #endregion

            model.Count = unitOfWork.NoticeRepository.Get(filter: m => m.Type == type).Count();

            model.PortfolioNameAR = notice.Portfolio.NameAR;
            model.PortfolioNameEN = notice.Portfolio.NameEN;
            model.PortfolioCode = notice.Portfolio.Code;
            model.PortfolioAccountID = notice.Portfolio.PortfolioAccounts.Select(a => a.AccountID).FirstOrDefault();
            model.PortfolioAccountCode = notice.Portfolio.PortfolioAccounts.Select(a => a.Account.Code).FirstOrDefault();
            model.PortfolioAccountNameAR = notice.Portfolio.PortfolioAccounts.Select(a => a.Account.NameAR).FirstOrDefault();
            model.PortfolioAccountNameEN = notice.Portfolio.PortfolioAccounts.Select(a => a.Account.NameEN).FirstOrDefault();

            model.EmployeeNameAR = notice.Employee.NameAR;
            model.EmployeeNameEN = notice.Employee.NameEN;
            model.EmployeeCode = notice.Employee.Code;

            #region Setting part
            if (type == true)
            {
                model.SettingModel = GetSetting(4);
            }
            else
            {
                model.SettingModel = GetSetting(3);
            }

            #region get partners data
            if (unitOfWork.PortfolioTransactionsRepository.Get(z => z.PortfolioID == notice.PortfolioID).Count() > 0)
            {
                model.portfolioTransactionModels = unitOfWork.PortfolioTransactionsRepository.Get(z => z.PortfolioID == notice.PortfolioID).Select(q => new PortfolioTransactionModel
                {
                    PartnerID = q.PartnerID,
                    CurrentStocksCount = q.CurrentStocksCount,
                    CurrentStockValue = q.CurrentStockValue,
                    partenerCode = q.Partner.Code,
                    partenerNameAR = q.Partner.NameAR,
                    partenerNameEN = q.Partner.NameEN,
                    PortfolioID = q.PortfolioID,
                    PortTransID = q.PortTransID,

                });
            }
            else
                model.portfolioTransactionModels = Array.Empty<PortfolioTransactionModel>();
            #endregion

            #endregion
            var check = unitOfWork.EntryRepository.Get(x => x.NoticeID == notice.NoticeID).SingleOrDefault();
            if (check != null)
            {
                model.EntryModel = GetEntry(model.NoticeID);
            }

            return model;
        }

        #endregion

        #region Getport
        public PortfolioModel GetPortfolio(Portfolio portfolio)
        {
            var model = _mapper.Map<PortfolioModel>(portfolio);
            if (model == null)
            {
                return model;
            }
            #region Date part

            if (portfolio.EstablishDate != null)
            {
                model.EstablishDate = portfolio.EstablishDate.Value.ToString("d/M/yyyy");
            }

            //  model.EstablishDateHijri = DateHelper.GetHijriDate(portfolio.EstablishDate);

            #endregion

            #region Accounts part
            var PortAccount = unitOfWork.PortfolioAccountRepository

                .GetEntity(filter: m => m.PortfolioID == portfolio.PortfolioID);

            if (PortAccount != null)
            {

                model.AccountID = PortAccount.AccountID;
                model.AccountCode = PortAccount.Account.Code;
                model.AccountNameAR = PortAccount.Account.NameAR;
                model.AccountNameEN = PortAccount.Account.NameEN;
                model.RSBalance = PortAccount.Account.Debit - PortAccount.Account.Credit;
            }


            #endregion

            #region Shareholders part
            var OpeningStocks = unitOfWork.PortfolioOpeningStocksRepository

                .Get(filter: m => m.PortfolioID == portfolio.PortfolioID)
                .Select(m => new PortfolioOpeningStocksModel
                {
                    PortOPenStockID = m.PortOPenStockID,
                    OpeningStocksCount = m.OpeningStocksCount,
                    OpeningStockValue = m.OpeningStockValue,
                    PartnerID = m.PartnerID,
                    PartnerCode = m.Partner.Code,
                    PartnerNameAR = m.Partner.NameAR,
                    PartnerNameEN = m.Partner.NameEN,
                    PortfolioID = m.PortfolioID,
                    PortfolioCode = m.Portfolio.Code,
                    PortfolioNameAR = m.Portfolio.NameAR,
                    PortfolioNameEN = m.Portfolio.NameEN,


                });




            if (OpeningStocks != null)
            {
                model.portfolioOpeningStocksArray = OpeningStocks;

            }
            //if (currentstocks != null)
            //{
            //    model.TotalStocksCount = 0;

            //    foreach (var item in currentstocks)
            //    {

            //        model.TotalStocksCount += item.CurrentStocksCount;
            //    }



            //}



            #endregion




            model.Count = unitOfWork.PortfolioRepository.Count();




            return model;
        }
        #endregion


        [Route("~/api/Search/{TableNum}/{Code}")]
        [HttpGet]
        public IActionResult Search(int TableNum , string Code)
        {
            switch (TableNum)
            {
                #region Purchase Order
                case 1:
                   var PurchaseInvoiceEntitylist = unitOfWork.PurchaseInvoiceRepository.Get(filter: a => a.Code == Code).SingleOrDefault();
                    if (PurchaseInvoiceEntitylist != null)
                    {
                        
                    var purchaseInvoiceModel = _mapper.Map<PurchaseInvoiceModel>(PurchaseInvoiceEntitylist);

                        purchaseInvoiceModel.PurchaseDate = PurchaseInvoiceEntitylist.Date.Value.ToString("d/M/yyyy");
                        purchaseInvoiceModel.PurchaseDate = DateHelper.GetHijriDate(PurchaseInvoiceEntitylist.Date);

                        var EmplyeeEntity = unitOfWork.EmployeeRepository.Get(filter: e => e.EmployeeID == purchaseInvoiceModel.EmployeeID).SingleOrDefault();
                        purchaseInvoiceModel.EmpCode = EmplyeeEntity.Code;
                        purchaseInvoiceModel.EmpNameAR = EmplyeeEntity.NameAR;
                        purchaseInvoiceModel.EmpNameEN = EmplyeeEntity.NameEN;
                        var id = unitOfWork.PurchaseOrderRepository.GetEntity(filter: a => a.PurchaseOrderID == purchaseInvoiceModel.PurchaseOrderID).PortfolioID;
                        var PortfolioEntity = unitOfWork.PortfolioRepository.GetEntity(filter: p => p.PortfolioID == id);
                        purchaseInvoiceModel.PortfolioCode = PortfolioEntity.Code;
                        purchaseInvoiceModel.PortfolioNameAR = PortfolioEntity.NameAR;
                        purchaseInvoiceModel.PortfolioNameEN = PortfolioEntity.NameEN;

                        purchaseInvoiceModel.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(filter: s => s.PortfolioID == id).AccountID;

                         purchaseInvoiceModel.DetailsModels = unitOfWork.PurchaseInvoiceDetailRepository.Get(filter: z => z.PurchaseInvoiceID == purchaseInvoiceModel.PurchaseInvoiceID).Select(a=> new PurchaseInvoiceDetailModel {
                             BankCommission=a.BankCommission,
                             BankCommissionRate=a.BankCommissionRate,
                             NetAmmount=a.NetAmmount,
                             PartnerCode=a.Partner.Code,
                             PartnerID=a.PartnerID,
                              PartnerNameAR=a.Partner.NameAR,
                              PartnerNameEN=a.Partner.NameEN,
                              PurchaseInvoiceDetailID=a.PurchaseInvoiceDetailID,
                              PurchaseInvoiceID=a.PurchaseInvoiceID,
                              PurchasePrice=a.PurchasePrice,
                               PurchaseValue=a.PurchaseValue,
                               StockCount=a.StockCount,
                               TaxOnCommission=a.TaxOnCommission,
                               TaxRateOnCommission=a.TaxRateOnCommission,
                               

                         });
                       

                      
                        purchaseInvoiceModel.SettingModel = GetSetting(2);

                        var EntryPurchaseInvoiceEntitylist = unitOfWork.EntryRepository.Get(filter: a => a.PurchaseInvoiceID == purchaseInvoiceModel.PurchaseInvoiceID);
                        purchaseInvoiceModel.EntryModel = _mapper.Map<EntryModel>(EntryPurchaseInvoiceEntitylist.SingleOrDefault());
                        if (EntryPurchaseInvoiceEntitylist.Count() > 0)
                        {
                            var EntryDitailsPurchaseInvoiceEntitylist = unitOfWork.EntryDetailRepository.Get(filter: d => d.EntryID == purchaseInvoiceModel.EntryModel.EntryID);
                            purchaseInvoiceModel.EntryModel.EntryDetailModel = _mapper.Map<IEnumerable<EntryDetailModel>>(EntryDitailsPurchaseInvoiceEntitylist);
                        }
                        
                        return Ok(purchaseInvoiceModel);
                    }
                    else
                    {
                        return Ok(0);
                    }
                #endregion

                #region selling Invoice
                case 2:
                    var SellingInvoiceEntitylist = unitOfWork.SellingInvoiceReposetory.Get(NoTrack: "NoTrack", filter: a => a.Code == Code).SingleOrDefault();
                    if (SellingInvoiceEntitylist.Code != null)
                    {
                        SellingInvoiceModel SellingInvoiceModel = new SellingInvoiceModel();
                        SellingInvoiceModel = _mapper.Map<SellingInvoiceModel>(SellingInvoiceEntitylist);

                        SellingInvoiceModel.SellDate = SellingInvoiceEntitylist.Date.Value.ToString("d/M/yyyy");
                        SellingInvoiceModel.SellDateHijri = DateHelper.GetHijriDate(SellingInvoiceEntitylist.Date);

                        var EmplyeeEntity = unitOfWork.EmployeeRepository.Get(filter: e => e.EmployeeID == SellingInvoiceModel.EmployeeID).SingleOrDefault();
                        SellingInvoiceModel.EmpCode = EmplyeeEntity.Code;
                        SellingInvoiceModel.EmpNameAR = EmplyeeEntity.NameAR;
                        SellingInvoiceModel.EmpCode = EmplyeeEntity.NameEN;
                        var id = unitOfWork.SellingOrderRepository.GetEntity(filter: x => x.SellingOrderID == SellingInvoiceEntitylist.SellingOrderID).PortfolioID;
                        var PortfolioEntity = unitOfWork.PortfolioRepository.Get(filter: p => p.PortfolioID == id).SingleOrDefault();
                        SellingInvoiceModel.PortfolioCode = PortfolioEntity.Code;
                        SellingInvoiceModel.PortfolioNameAR = PortfolioEntity.NameAR;
                        SellingInvoiceModel.PortfolioNameEN = PortfolioEntity.NameEN;

                        SellingInvoiceModel.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(filter: s => s.PortfolioID == id).AccountID;

                        SellingInvoiceModel.DetailsModels = unitOfWork.SellingInvoiceDetailRepository.Get(filter: z => z.SellingInvoiceID == SellingInvoiceEntitylist.SellingInvoiceID).Select( a=> new SellingInvoiceDetailsModel {
                            BankCommission=a.BankCommission,
                            BankCommissionRate=a.BankCommissionRate,
                            NetAmmount=a.NetAmmount,
                            PartnerCode=a.Partner.Code,
                            PartnerID=a.PartnerID,
                            PartnerNameAR=a.Partner.NameAR,
                            PartnerNameEN=a.Partner.NameEN,
                            SelingValue=a.SelingValue,
                            SellingInvoiceDetailID=a.SellInvoiceDetailID,
                            SellingInvoiceID=a.SellingInvoiceID,
                            SellingPrice=a.SellingPrice,
                            StockCount=a.StockCount,
                            StocksCount=a.StockCount,
                            TaxRateOnCommission=a.TaxRateOnCommission,
                            TaxOnCommission=a.TaxOnCommission,
                        });
                         

                        SellingInvoiceModel.SettingModel = GetSetting(1);
                      

                        var EntrySellingInvoiceEntitylist = unitOfWork.EntryRepository.Get(filter: a => a.SellingInvoiceID == SellingInvoiceModel.SellingInvoiceID);
                        SellingInvoiceModel.EntryModel = _mapper.Map<EntryModel>(EntrySellingInvoiceEntitylist.SingleOrDefault());
                        if (EntrySellingInvoiceEntitylist.Count() > 0)
                        {
                            var EntryDitailsSellingInvoiceEntitylist = unitOfWork.EntryDetailRepository.Get(filter: d => d.EntryID == SellingInvoiceModel.EntryModel.EntryID);
                            SellingInvoiceModel.EntryModel.EntryDetailModel = _mapper.Map<IEnumerable<EntryDetailModel>>(EntryDitailsSellingInvoiceEntitylist);
                        }
                        return Ok(SellingInvoiceModel);
                    }
                    else
                    {
                        return Ok(0);
                    }
                #endregion

             


                #region Notice Creditor
                case 3:
                    var NoticeCreditorEntityList = unitOfWork.NoticeRepository.Get(filter: a => a.Code == Code && a.Type==false);
                    if (NoticeCreditorEntityList.Count() > 0)
                    {
                        NoticeModel noticeModel = new NoticeModel();
                        noticeModel = GetNotice(NoticeCreditorEntityList.SingleOrDefault(), false);
                        return Ok(noticeModel);
                    }
                    else
                    {
                        return Ok(0);
                    }
                #endregion

                #region Notice Debitor
                case 4:
                    var NoticeDebitorEntityList = unitOfWork.NoticeRepository.Get(filter: a => a.Code == Code && a.Type == true);
                    if (NoticeDebitorEntityList.Count() > 0)
                    {
                        NoticeModel noticeModel = new NoticeModel();
                        noticeModel = GetNotice(NoticeDebitorEntityList.SingleOrDefault(), true);
                        return Ok(noticeModel);
                    }
                    else
                    {
                        return Ok(0);
                    }
                #endregion

                #region  Receipt voucher Ryal
                case 5:
                    {
                        var ReceiptExchangeEntitylist = unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Code == Code);
                        if (ReceiptExchangeEntitylist.Count() > 0)
                        {
                            ReceiptExchangeModel receiptExchangeModel = new ReceiptExchangeModel();
                            receiptExchangeModel = GetReceiptExchange(ReceiptExchangeEntitylist.SingleOrDefault(), false, true, 5);
                            return Ok(receiptExchangeModel);
                        }
                        else
                        {
                            return Ok(0);
                        }
                    }
                #endregion

                #region  Receipt voucher chique
                case 6:
                    {
                        var ReceiptExchangeEntitylist = unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Code == Code);
                        if (ReceiptExchangeEntitylist.Count() > 0)
                        {
                            ReceiptExchangeModel receiptExchangeModel = new ReceiptExchangeModel();
                            receiptExchangeModel = GetReceiptExchange(ReceiptExchangeEntitylist.SingleOrDefault(), false, false, 5);
                            return Ok(receiptExchangeModel);
                        }
                        else
                        {
                            return Ok(0);
                        }
                    }
                #endregion


                #region  Exchange voucher Ryal
                case 7:
                    {
                        var ReceiptExchangeEntitylist = unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Code == Code);
                        if (ReceiptExchangeEntitylist.Count() > 0)
                        {
                            ReceiptExchangeModel receiptExchangeModel = new ReceiptExchangeModel();
                            receiptExchangeModel = GetReceiptExchange(ReceiptExchangeEntitylist.SingleOrDefault(), true, true, 6);
                            return Ok(receiptExchangeModel);
                        }
                        else
                        {
                            return Ok(0);
                        }
                    }
                #endregion

                #region  Exchange voucher chique
                case 8:
                    {
                        var ReceiptExchangeEntitylist = unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Code == Code);
                        if (ReceiptExchangeEntitylist.Count() > 0)
                        {
                            ReceiptExchangeModel receiptExchangeModel = new ReceiptExchangeModel();
                            receiptExchangeModel = GetReceiptExchange(ReceiptExchangeEntitylist.SingleOrDefault(), true, false, 6);
                            return Ok(receiptExchangeModel);
                        }
                        else
                        {
                            return Ok(0);
                        }
                    }
                #endregion

                #region Country
                case 9:
                    {
                        var CountryEntityList = unitOfWork.CountryRepository.Get(filter: x => x.Code==Code||x.NameAR==Code);
                        if (CountryEntityList.Count() >0)
                        {
                            CountryModel countryModel =_mapper.Map<CountryModel>(CountryEntityList.SingleOrDefault());
                            return Ok(countryModel);
                        }
                        else
                        {
                            return Ok(0);
                        }
                    }
                #endregion


                #region Account
                case 10:
                    {
                        var AccountEntityList = unitOfWork.AccountRepository.Get(filter: x => x.Code == Code || x.NameAR == Code);
                        if (AccountEntityList.Count() > 0)
                        {
                            AccountModel AccountModel = _mapper.Map<AccountModel>(AccountEntityList.SingleOrDefault());
                            return Ok(AccountModel);
                        }
                        else
                        {
                            return Ok(0);
                        }
                    }
                #endregion
                #region Employee
                case 11:
                    {
                        var EmployeeEntityList = unitOfWork.EmployeeRepository.Get(filter: x => x.Code == Code || x.NameAR == Code);
                        if (EmployeeEntityList.Count() > 0)
                        {
                            EmployeeModel EmployeeModel = _mapper.Map<EmployeeModel>(EmployeeEntityList.SingleOrDefault());
                            var EmployCardsEntityList = unitOfWork.EmployeeCardRepository.Get(filter: x => x.EmployeeID == EmployeeModel.EmployeeID);
                            EmployeeModel.emplCards = _mapper.Map<IEnumerable<EmployeeCardModel>>(EmployCardsEntityList);
                            return Ok(EmployeeModel);
                        }
                        else
                        {
                            return Ok(0);
                        }
                    }
                #endregion

                #region Partner
                case 12:
                    {
                        var PartnerEntityList = unitOfWork.PartnerRepository.Get(filter: x => x.Code == Code || x.NameAR == Code);
                        if (PartnerEntityList.Count() > 0)
                        {
                            PartenerModel partnerModel = _mapper.Map<PartenerModel>(PartnerEntityList.SingleOrDefault());
                            var Countries = unitOfWork.CountryRepository.Get(filter: x => x.CountryID == partnerModel.CountryID);
                            partnerModel.Countries = _mapper.Map<IEnumerable<CountryModel>>(Countries);
                            IEnumerable<PortfolioTransaction> StocksCountList = unitOfWork.PortfolioTransactionsRepository.Get(filter: x => x.PartnerID == partnerModel.PartnerID);
                            partnerModel.StocksCount = 0;
                            for (int ii = 0; ii < StocksCountList.Count(); ii++)
                            {
                                partnerModel.StocksCount += StocksCountList.ElementAt(ii).CurrentStocksCount;
                            }
                            return Ok(partnerModel);
                        }
                        else
                        {
                            return Ok(0);
                        }
                    }
                #endregion

                #region portfolio
                case 13:
                    {
                        
                        var PortfolioEntityList = unitOfWork.PortfolioRepository.Get(filter: x => x.Code == Code || x.NameAR == Code);
                        if (PortfolioEntityList.Count() > 0)
                        {
                            return Ok(GetPortfolio(PortfolioEntityList.SingleOrDefault()));
                        }
                        else
                        {
                            return Ok(0);
                        }
                    }
                #endregion


                #region selling Order


                case 14:
                    {

                      
                           
                                var sellingorder = unitOfWork.SellingOrderRepository.Get(filter: x=> x.Code== Code).SingleOrDefault();
                                var model = _mapper.Map<SellingOrderModel>(sellingorder);
                        if (sellingorder != null)
                        {
                            #region Date part 
                            if (sellingorder.OrderDate != null)
                            {
                                model.OrderDateGorg = sellingorder.OrderDate.ToString("d/M/yyyy");
                                model.OrderDateHigri = DateHelper.GetHijriDate(sellingorder.OrderDate);
                            }



                            #endregion

                            #region  Details
                            var Details = unitOfWork.SellingOrderDetailRepository

                                .Get(filter: m => m.SellingOrderID == sellingorder.SellingOrderID)
                                .Select(m => new SellingOrderDetailModel
                                {
                                    PartnerID = m.PartnerID,
                                    PartnerNameAr = m.Partner.NameAR,
                                    PriceType = m.PriceType,
                                    SellingOrderID = m.SellingOrderID,
                                    SellOrderDetailID = m.SellOrderDetailID,
                                    StockCount = m.StockCount,
                                    PartnerCode = m.Partner.Code




                                });




                            if (Details != null)
                            {
                                model.sellingOrderDetailModels = Details;

                            }

                            #endregion
                            model.Count = unitOfWork.SellingOrderRepository.Count();
                            model.Portfoliocode = unitOfWork.PortfolioRepository.GetEntity(filter: a => a.PortfolioID == sellingorder.PortfolioID).Code;

                            return Ok(model);

                        }
                        else
                        {
                            return Ok(0);
                        }
                            }









                #endregion

                #region  Order


                case 15:
                    {



                        var Purchaseorder = unitOfWork.PurchaseOrderRepository.Get(filter: x => x.Code == Code).SingleOrDefault();
                        var model = _mapper.Map<PurchaseOrderModel>(Purchaseorder);

                        #region Date part 
                        if (Purchaseorder.OrderDate != null)
                        {
                            model.OrderDate = Purchaseorder.OrderDate.ToString("d/M/yyyy");
                            model.OrderDateHijri = DateHelper.GetHijriDate(Purchaseorder.OrderDate);
                        }



                        #endregion

                        #region  Details
                        var Details = unitOfWork.PurchaseOrderDetailRepository

                            .Get(filter: m => m.PurchaseOrderID == Purchaseorder.PurchaseOrderID)
                            .Select(m => new PurchaseOrderDetailModel
                            {
                                PartnerID = m.PartnerID,
                                PartnerNameAr = m.Partner.NameAR,
                                PriceType = m.PriceType,
                                PurchaseOrderID = m.PurchaseOrderID,
                                PurchaseOrderDetailID = m.PurchaseOrderDetailID,
                                StockCount = m.StockCount,
                                PartnerCode = m.Partner.Code




                            });




                        if (Details != null)
                        {
                            model.purchaseOrderDetailModels = Details;

                        }

                        #endregion
                        model.Count = unitOfWork.PurchaseOrderRepository.Count();
                        model.Portfoliocode = unitOfWork.PortfolioRepository.GetEntity(filter: a => a.PortfolioID == Purchaseorder.PortfolioID).Code;

                        return Ok(model);


                    }









                    #endregion


            }
            return Ok("Error Table Number");
         }
     }
 }
