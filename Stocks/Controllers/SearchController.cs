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
    //[Authorize(Roles = "SuperAdmin,Admin,Employee")]
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
                entryModel.PurchaseOrderID = Entry.PurchaseOrderID;
                entryModel.ReceiptID = Entry.ReceiptID;
                entryModel.SellingOrderID = Entry.SellingOrderID;
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
                entryModel.PurchaseOrderID = Entry.PurchaseOrderID;
                entryModel.ReceiptID = Entry.ReceiptID;
                entryModel.SellingOrderID = Entry.SellingOrderID;
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
                    var PurchaseOrderEntitylist = unitOfWork.PurchaseOrderRepository.Get(NoTrack: "NoTrack", filter: a => a.Code == Code);
                    if (PurchaseOrderEntitylist.Count() > 0)
                    {
                        PurchaseOrderModel purchaseOrderModel = new PurchaseOrderModel();
                        purchaseOrderModel = _mapper.Map<PurchaseOrderModel>(PurchaseOrderEntitylist.SingleOrDefault());
                        if (PurchaseOrderEntitylist.SingleOrDefault().Date != null)
                        {
                            purchaseOrderModel.PurchaseDate = PurchaseOrderEntitylist.SingleOrDefault().Date.Value.ToString("d/M/yyyy");
                            purchaseOrderModel.PurchaseDateHijri = DateHelper.GetHijriDate(PurchaseOrderEntitylist.SingleOrDefault().Date);
                        }
                        var EmplyeeEntity = unitOfWork.EmployeeRepository.Get(filter: e => e.EmployeeID == purchaseOrderModel.EmployeeID).SingleOrDefault();
                        if (EmplyeeEntity != null)
                        {
                            purchaseOrderModel.EmpCode = EmplyeeEntity.Code;
                            purchaseOrderModel.EmpNameAR = EmplyeeEntity.NameAR;
                            purchaseOrderModel.EmpCode = EmplyeeEntity.NameEN;
                        }
                        var PortfolioEntity = unitOfWork.PortfolioRepository.Get(filter: p => p.PortfolioID == purchaseOrderModel.PortfolioID).SingleOrDefault();
                        purchaseOrderModel.PortfolioCode = PortfolioEntity.Code;
                        purchaseOrderModel.PortfolioNameAR = PortfolioEntity.NameAR;
                        purchaseOrderModel.PortfolioNameEN = PortfolioEntity.NameEN;

                        purchaseOrderModel.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(filter: s => s.PortfolioID == purchaseOrderModel.PortfolioID).AccountID;

                        var PurchaseOrderDitailsEntitylist = unitOfWork.PurchaseOrderDetailRepository.Get(filter: z => z.PurchaseID == purchaseOrderModel.PurchaseOrderID);
                        purchaseOrderModel.DetailsModels = _mapper.Map<IEnumerable<PurchaseOrderDetailModel>>(PurchaseOrderDitailsEntitylist);

                      
                        purchaseOrderModel.SettingModel = GetSetting(2);

                        var EntryPurchaseOrderEntitylist = unitOfWork.EntryRepository.Get(filter: a => a.PurchaseOrderID == purchaseOrderModel.PurchaseOrderID);
                        purchaseOrderModel.EntryModel = _mapper.Map<EntryModel>(EntryPurchaseOrderEntitylist.SingleOrDefault());
                        if (EntryPurchaseOrderEntitylist.Count() > 0)
                        {
                            var EntryDitailsPurchaseOrderEntitylist = unitOfWork.EntryDetailRepository.Get(filter: d => d.EntryID == purchaseOrderModel.EntryModel.EntryID);
                            purchaseOrderModel.EntryModel.EntryDetailModel = _mapper.Map<IEnumerable<EntryDetailModel>>(EntryDitailsPurchaseOrderEntitylist);
                        }
                        
                        return Ok(purchaseOrderModel);
                    }
                    else
                    {
                        return Ok(0);
                    }
                #endregion

                #region selling Order
                case 2:
                    var SellingOrderEntitylist = unitOfWork.SellingOrderReposetory.Get(NoTrack: "NoTrack", filter: a => a.Code == Code);
                    if (SellingOrderEntitylist.Count() > 0)
                    {
                        SellingOrderModel SellingOrderModel = new SellingOrderModel();
                        SellingOrderModel = _mapper.Map<SellingOrderModel>(SellingOrderEntitylist.SingleOrDefault());
                        if (SellingOrderEntitylist.SingleOrDefault().Date != null)
                        {
                            SellingOrderModel.SellDate = SellingOrderEntitylist.SingleOrDefault().Date.Value.ToString("d/M/yyyy");
                            SellingOrderModel.SellDateHijri = DateHelper.GetHijriDate(SellingOrderEntitylist.SingleOrDefault().Date);
                        }
                        var EmplyeeEntity = unitOfWork.EmployeeRepository.Get(filter: e => e.EmployeeID == SellingOrderModel.EmployeeID).SingleOrDefault();
                        if (EmplyeeEntity != null)
                        {
                            SellingOrderModel.EmpCode = EmplyeeEntity.Code;
                            SellingOrderModel.EmpNameAR = EmplyeeEntity.NameAR;
                            SellingOrderModel.EmpCode = EmplyeeEntity.NameEN;
                        }
                        var PortfolioEntity = unitOfWork.PortfolioRepository.Get(filter: p => p.PortfolioID == SellingOrderModel.PortfolioID).SingleOrDefault();
                        SellingOrderModel.PortfolioCode = PortfolioEntity.Code;
                        SellingOrderModel.PortfolioNameAR = PortfolioEntity.NameAR;
                        SellingOrderModel.PortfolioNameEN = PortfolioEntity.NameEN;

                        SellingOrderModel.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(filter: s => s.PortfolioID == SellingOrderModel.PortfolioID).AccountID;

                        var SellingOrderDitailsEntitylist = unitOfWork.SellingOrderDetailRepository.Get(filter: z => z.SellingOrderID == SellingOrderModel.SellingOrderID);
                        SellingOrderModel.DetailsModels = _mapper.Map<IEnumerable<SelingOrderDetailsModel>>(SellingOrderDitailsEntitylist);

                        SellingOrderModel.SettingModel = GetSetting(1);
                      

                        var EntrySellingOrderEntitylist = unitOfWork.EntryRepository.Get(filter: a => a.SellingOrderID == SellingOrderModel.SellingOrderID);
                        SellingOrderModel.EntryModel = _mapper.Map<EntryModel>(EntrySellingOrderEntitylist.SingleOrDefault());
                        if (EntrySellingOrderEntitylist.Count() > 0)
                        {
                            var EntryDitailsSellingOrderEntitylist = unitOfWork.EntryDetailRepository.Get(filter: d => d.EntryID == SellingOrderModel.EntryModel.EntryID);
                            SellingOrderModel.EntryModel.EntryDetailModel = _mapper.Map<IEnumerable<EntryDetailModel>>(EntryDitailsSellingOrderEntitylist);
                        }
                        return Ok(SellingOrderModel);
                    }
                    else
                    {
                        return Ok(0);
                    }
                #endregion

                #region Notice Creditor
                case 3:
                    var NoticeCreditorEntityList = unitOfWork.NoticeRepository.Get(filter: a => a.Code == Code);
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
                    var NoticeDebitorEntityList = unitOfWork.NoticeRepository.Get(filter: a => a.Code == Code);
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
                            return Ok(partnerModel);
                        }
                        else
                        {
                            return Ok(0);
                        }
                    }
                #endregion

                #region Portfolio
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
            }
            return Ok("Error Table Number");
         }
     }
 }
