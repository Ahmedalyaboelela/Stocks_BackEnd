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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Stocks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticeController : ControllerBase
    {
        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private SettingController setting;
        private readonly IAccountingHelper accountingHelper;
        public NoticeController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
            this.setting = new SettingController(context, mapper);
            accountingHelper = new AccountingHelper(context, mapper);
        }

        #endregion



        [Route("~/api/Notice/GetCurrency")]
        public IEnumerable<CurrencyModel> GetAllcurency()
        {

            var currency = unitOfWork.CurrencyRepository.Get().Select(a => new CurrencyModel
            {

                Code = a.Code,
                CurrencyID=a.CurrencyID,
                CurrencyValue=a.CurrencyValue,
                NameAR=a.NameAR,
                NameEN=a.NameEN,
                PartName=a.PartName,
                PartValue=a.PartValue
                 



            });



            return currency;


        }

        [Route("~/api/Notice/GetAllAccounts")]
      
        public IActionResult GetAllAccounts()
        {
            var account = unitOfWork.AccountRepository.Get(filter: a => a.AccountType == false);
            var model = _mapper.Map<IEnumerable<AccountModel>>(account);

            if (model == null)
            {
                return Ok(0);
            }

            return Ok(model);
        }
    




        [Route("~/api/Notice/GetPortfolios")]
        public IEnumerable<PortfolioModel> GetPortfolio()
        {

            var Portfoli = unitOfWork.PortfolioRepository.Get().Select(a => new PortfolioModel
            {
                PortfolioID = a.PortfolioID,
                Code = a.Code,
                Description = a.Description,
                EstablishDate = a.EstablishDate.Value.ToString("dd/MM/yyyy"),
                EstablishDateHijri = DateHelper.GetHijriDate(a.EstablishDate),
                AccountID = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == a.PortfolioID).AccountID,
                AccountCode = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == a.PortfolioID).Account.Code,
                AccountNameAR = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == a.PortfolioID).Account.NameAR,
                AccountNameEN = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == a.PortfolioID).Account.NameEN,
                PortfolioAccountDebit = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == a.PortfolioID).Account.Debit,
                NameAR = a.NameAR,
                NameEN = a.NameEN,
                TotalStocksCount = a.TotalStocksCount,
                portfolioTransactionModels = unitOfWork.PortfolioTransactionsRepository.Get(filter: s => s.PortfolioID == a.PortfolioID).Select(q => new PortfolioTransactionModel
                {
                    PartnerID = q.PartnerID,
                    CurrentStocksCount = q.CurrentStocksCount,
                    CurrentStockValue = q.CurrentStockValue,
                    partenerCode = q.Partner.Code,
                    partenerNameAR = q.Partner.NameAR,
                    partenerNameEN = q.Partner.NameEN,
                    PortfolioID = q.PortfolioID,
                    PortTransID = q.PortTransID,
                }),
                

        });



            return Portfoli;


        }


        [Route("~/api/Notice/GetEmps")]
        public IEnumerable<EmployeeModel> GetEmps()
        {

            var Epms = unitOfWork.EmployeeRepository.Get().Select(a => new EmployeeModel
            {
                EmployeeID=a.EmployeeID,
                Age=a.Age,
                BankAccNum=a.BankAccNum,
                BirthDate=a.BirthDate.Value.ToString("dd/MM/yyyy"),
                BirthDateHijri=DateHelper.GetHijriDate(a.BirthDate),
                Code=a.Code,
                Email=a.Email,
                ImagePath=a.ImagePath,
                IsActive=a.IsActive,
                IsInternal=a.IsInternal,
                NameAR=a.NameAR,
                NameEN=a.NameEN,
                Mobile=a.Mobile,
                Nationality=a.Nationality,
                Religion=a.Religion,
                Profession=a.Profession,
                PassportProfession=a.PassportProfession,
            });



            return Epms;


        }



        [Route("~/api/Notice/GetSettingAccounts/{id}")]
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

        [Route("~/api/Notice/GetSetting/{type}")]
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
                SettingAccs = SettingAccounts(a.SettingID),

            }).SingleOrDefault();
            return setsetting;


        }


        #region GET Methods

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
                model.NoticeDate = notice.NoticeDate.Value.ToString("dd/MM/yyyy");
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
                    Credit=m.Credit,
                    StocksCredit=m.StocksCredit,
                    StocksDebit=m.StocksDebit,
                    AccountID=m.AccountID,
                    AccCode=m.Account.Code,
                    AccNameAR=m.Account.NameAR,
                    AccNameEN=m.Account.NameEN,
                    PartnerID=m.PartnerID
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
            if (type==true)
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


        [HttpGet]
        [Route("~/api/Notice/FirstOpen/{type}")]
        public IActionResult FirstOpen(bool type)
        {
            //  DefaultSettingModel model = new DefaultSettingModel();
            //model.ScreenSetting = GetSetting(3);
            //model.LastCode = unitOfWork.EntryRepository.Last().Code;
            NoticeModel model = new NoticeModel();
           
            if (type==true)
            {
                if(unitOfWork.NoticeRepository.Get(filter: x => x.Type == true).Count()>0)
                {
                    if (unitOfWork.NoticeRepository.Get(filter: x => x.Type == true).Last() == null)
                    {
                        return Ok(0);
                    }
                    var noti = unitOfWork.NoticeRepository.Get(filter: x => x.Type == true).Last();


                    model.LastCode = noti.Code;
                    model.SettingModel = GetSetting(4);
                    model.Count = unitOfWork.NoticeRepository.Get(filter: x => x.Type == true).Count();
                }

            }
            else
            {
                if (unitOfWork.NoticeRepository.Get(filter: x => x.Type == false).Count() > 0)
                {
                    if (unitOfWork.NoticeRepository.Get(filter: x => x.Type == false).Last() == null)
                    {
                        return Ok(0);
                    }

                    var noti = unitOfWork.NoticeRepository.Get(filter: x => x.Type == false).Last();

                    model.LastCode = noti.Code;
                    model.SettingModel = GetSetting(3);
                    model.Count = unitOfWork.NoticeRepository.Get(filter: x => x.Type == false).Count();
                }

               
            }

           
            
            return Ok(model);
        }

        [HttpGet]
        [Route("~/api/Notice/GetLast/{type}")]
        public IActionResult GetLast(bool type)
        {

            // get last Receipt or Exchange
            var notice = unitOfWork.NoticeRepository.Get(filter: m => m.Type == type).Last();
            if (notice != null)
                return Ok(GetNotice(notice, type));
            else
                return Ok(0);

        }


        [HttpGet]
        [Route("~/api/Notice/Paging/{type}/{pageNumber}")]
        public IActionResult Pagination(bool type ,int pageNumber )
        {
            if (pageNumber > 0)
            {

                var notice = unitOfWork.NoticeRepository.Get(filter: m => m.Type == type, page: pageNumber).FirstOrDefault();
                if (notice != null)
                    return Ok(GetNotice(notice, type));
                else
                    return Ok(0);

            }
            else
                return Ok(1);
        }


        [HttpGet]
        [Route("~/api/Notice/Get/{type}/{id}")]

        public IActionResult GetById(bool type,int id)
        {

            if (id > 0)
            {

                var notice = unitOfWork.NoticeRepository.Get(filter: m => m.Type == type && m.NoticeID == id).FirstOrDefault();
                if (notice != null)
                    return Ok(GetNotice(notice, type));
                else
                    return Ok(0);


            }
            else
                return Ok(1);
        }


        [Route("~/api/Notice/GetAll/{type}")]
        public IActionResult GetAll(bool type)
        {
            var notices = unitOfWork.NoticeRepository.Get(filter: m => m.Type == type).ToList();

            var model = _mapper.Map<IEnumerable<NoticeModel>>(notices).ToList();

            if (model == null)
            {
                return Ok(0);
            }

            for (int i = 0; i < notices.Count(); i++)
            {
                for (int j = i; j < model.Count(); j++)
                {
                    if (model[j].NoticeID == notices[i].NoticeID)
                    {
                        #region Date part

                        model[j].NoticeDate = notices[i].NoticeDate.Value.ToString("dd/MM/yyyy");
                        model[j].NoticeDateHijri = DateHelper.GetHijriDate(notices[i].NoticeDate);
                        #endregion

                        model[j].Count = unitOfWork.NoticeRepository.Get(filter: m => m.Type == type).Count();

                        #region Details part
                        var Details = unitOfWork.NoticeDetailRepository

                            .Get(filter: m => m.NoticeID == notices[i].NoticeID)
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
                                AccNameEN = m.Account.NameEN

                            });
                        if (Details != null)
                            model[j].NoticeModelDetails = Details;

                        #endregion


                    }
                    else
                        continue;


                }

            }


            return Ok(model);
        }

        #endregion


        public EntryModel GetEntry(Entry Entry)
        {
            EntryModel entryModel = new EntryModel();
            if (Entry != null)
            {
                var EntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
                entryModel.EntryID = Entry.EntryID;
                entryModel.Code = Entry.Code;
                entryModel.Date = Entry.Date.Value.ToString("dd/MM/yyyy");
                entryModel.DateHijri = DateHelper.GetHijriDate(Entry.Date);
                entryModel.NoticeID = Entry.NoticeID;
                entryModel.PurchaseOrderID = Entry.PurchaseOrderID;
                entryModel.ReceiptID = Entry.ReceiptID;
                entryModel.PurchaseOrderID = Entry.PurchaseOrderID;
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
                    Debit = m.Debit,
                    EntryDetailID = m.EntryDetailID,
                    EntryID = m.EntryID,


                });
                entryModel.TransferedToAccounts = Entry.TransferedToAccounts;

            }
            return entryModel;
        }

        [HttpGet]//القيد
        [Route("~/api/Notice/GetEntry/{noticeID}")]
        public EntryModel GetEntry(int  noticeID)
        {
            var Entry = unitOfWork.EntryRepository.Get(x => x.NoticeID == noticeID).SingleOrDefault();
            var EntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
            EntryModel entryModel = new EntryModel();
            entryModel.EntryID = Entry.EntryID;
            entryModel.Code = Entry.Code;
            entryModel.Date = Entry.Date.Value.ToString("dd/MM/yyyy");
            entryModel.DateHijri = DateHelper.GetHijriDate(Entry.Date);
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
                Debit = m.Debit,
                EntryDetailID = m.EntryDetailID,
                EntryID = m.EntryID,


            });
            entryModel.TransferedToAccounts = Entry.TransferedToAccounts;

            return entryModel;
        }

        [HttpPost]// ترحيل يدوي للقيد اليدوي والتلقائي
        [Route("~/api/Notice/ManualmigrationNotice")]
        public IActionResult ManualmigrationNotice([FromBody]EntryModel EntryMODEL)
        {
            var Entry = unitOfWork.EntryRepository.GetByID(EntryMODEL.EntryID);
            Entry.TransferedToAccounts = true;
            unitOfWork.EntryRepository.Update(Entry);

            var Details = EntryMODEL.EntryDetailModel;
            accountingHelper.TransferToAccounts(Details.Select(x => new EntryDetail
            {
                EntryDetailID = x.EntryDetailID,
                AccountID = x.AccountID,
                Credit = x.Credit,
                Debit = x.Debit,
                EntryID = x.EntryID


            }).ToList());




            unitOfWork.Save();



            return Ok(GetEntry(Entry));

        }




        [HttpPost] // يولد قيد مع ترحيل تلقائي
        [Route("~/api/Notice/GenerateconstraintNotice")]
        public IActionResult GenerateconstraintNotice([FromBody]NoticeModel noticeModel)
        {
            var lastEntry = unitOfWork.EntryRepository.Last();
            var lastEntryDitails = unitOfWork.EntryDetailRepository.Get(filter: x => x.EntryID == lastEntry.EntryID).Select(m => new EntryDetailModel
            {
                AccountID = m.AccountID,
                Credit = m.Credit,
                Debit = m.Debit,
                EntryID = m.EntryID,
                EntryDetailID = m.EntryDetailID,
                AccCode = m.Account.Code,
                AccNameAR = m.Account.NameAR,
                AccNameEN = m.Account.NameEN,



            });
            int portofolioaccount = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == noticeModel.PortfolioID && m.Type == true)
                .Select(m => m.PortfolioAccountID).SingleOrDefault();


            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,null, null, null, noticeModel,lastEntry);
            var Entry = _mapper.Map<Entry>(EntryMODEL);


            var DetailEnt = EntryMODEL.EntryDetailModel;

            if (noticeModel.SettingModel.TransferToAccounts == true)
            {
                Entry.TransferedToAccounts = true;
                unitOfWork.EntryRepository.Insert(Entry);
                foreach (var item in DetailEnt)
                {
                    item.EntryID = Entry.EntryID;
                    item.EntryDetailID = 0;
                    var details = _mapper.Map<NoticeDetail>(item);

                    unitOfWork.NoticeDetailRepository.Insert(details);
                }
                accountingHelper.TransferToAccounts(DetailEnt.Select(a => new EntryDetail
                {

                    Credit = a.Credit,
                    Debit = a.Debit,
                    EntryDetailID = a.EntryDetailID,
                    EntryID = a.EntryID,
                    StocksCredit = a.StocksCredit,
                    StocksDebit = a.StocksDebit,
                    AccountID = a.AccountID

                }).ToList());
            }
            else
            {
                Entry.TransferedToAccounts = false;
                unitOfWork.EntryRepository.Insert(Entry);
                foreach (var item in DetailEnt)
                {
                    item.EntryID = Entry.EntryID;
                    item.EntryDetailID = 0;
                    var details = _mapper.Map<NoticeDetail>(item);

                    unitOfWork.NoticeDetailRepository.Insert(details);

                }
            }
            return Ok(noticeModel);
        }


        #region Insert Methods
        [HttpPost]
        [Route("~/api/Notice/Add")]
        public IActionResult PostItem([FromBody] NoticeModel noticeModel)
        {

           
            if (ModelState.IsValid)
            {
                int portofolioaccount = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == noticeModel.PortfolioID && m.Type == true).Select(m => m.PortfolioAccountID).SingleOrDefault();

                var Check = unitOfWork.NoticeRepository.Get();
                if (Check.Any(m => m.Code == noticeModel.Code))
                {

                    return Ok("كود امر بيع مكرر");
                }
                else
                {

                    var notice = _mapper.Map<Notice>(noticeModel);


                    var Details = noticeModel.NoticeModelDetails;

                    unitOfWork.NoticeRepository.Insert(notice);

                    foreach (var item in Details)
                    {
                        NoticeDetailModel noticeDetailModel = new NoticeDetailModel();
                        noticeDetailModel.NoticeID = notice.NoticeID;
                        noticeDetailModel.AccountID = item.AccountID;
                        noticeDetailModel.Credit = item.Credit;
                        noticeDetailModel.Debit = item.Debit;
                        noticeDetailModel.StocksCredit = item.StocksCredit;
                        noticeDetailModel.StocksDebit = item.StocksDebit;
                        noticeDetailModel.PartnerID = item.PartnerID;

                        var details = _mapper.Map<NoticeDetail>(noticeDetailModel);
                        unitOfWork.NoticeDetailRepository.Insert(details);

                    }
                

                    //==================================================لا تولد قيد ===================================
                    //if (noticeModel.SettingModel.DoNotGenerateEntry == true)
                    //{
                    //    unitOfWork.Save();

                    //    return Ok(noticeModel);
                    //}

                    //===============================================================توليد قيد مع ترحيل تلقائي============================



                    if (noticeModel.SettingModel.AutoGenerateEntry == true)
                    {
                        var lastEntry = unitOfWork.EntryRepository.Last();
                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, null, null, null, noticeModel, lastEntry);
                        var Entry = _mapper.Map<Entry>(EntryMODEL);
                        Entry.NoticeID = notice.NoticeID;

                        var DetailEnt = EntryMODEL.EntryDetailModel;

                        if (noticeModel.SettingModel.TransferToAccounts == true)
                        {
                            Entry.TransferedToAccounts = true;
                            unitOfWork.EntryRepository.Insert(Entry);
                            foreach (var item in DetailEnt)
                            {
                                item.EntryID = Entry.EntryID;
                                item.EntryDetailID = 0;
                                var details = _mapper.Map<EntryDetail>(item);

                                unitOfWork.EntryDetailRepository.Insert(details);

                            }
                            accountingHelper.TransferToAccounts(DetailEnt.Select(x => new EntryDetail
                            {


                                EntryDetailID = x.EntryDetailID,
                                AccountID = x.AccountID,
                                Credit = x.Credit,
                                Debit = x.Debit,
                                EntryID = x.EntryID


                            }).ToList());
                        }

                        else
                        {
                            Entry.TransferedToAccounts = true;
                            unitOfWork.EntryRepository.Insert(Entry);
                            foreach (var item in DetailEnt)
                            {
                                item.EntryID = Entry.EntryID;
                                item.EntryDetailID = 0;
                                var details = _mapper.Map<EntryDetail>(item);

                                unitOfWork.EntryDetailRepository.Insert(details);
                            }
                        }
                    }

                    //================================توليد قيد مع عدم الترحيل====================================== 
                    //if (noticeModel.SettingModel.GenerateEntry == true)

                    //{

                    //    var lastEntry = unitOfWork.EntryRepository.Last();
                    //    var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, null, null, null, noticeModel, lastEntry);
                    //    var Entry = _mapper.Map<Entry>(EntryMODEL);
                    //    Entry.NoticeID = notice.NoticeID;

                    //    var DetailEnt = EntryMODEL.EntryDetailModel;
                    //    Entry.TransferedToAccounts = false;
                    //    unitOfWork.EntryRepository.Insert(Entry);
                    //    foreach (var item in DetailEnt)
                    //    {
                    //        item.EntryID = Entry.EntryID;
                    //        item.EntryDetailID = 0;
                    //        var details = _mapper.Map<EntryDetail>(item);

                    //        unitOfWork.EntryDetailRepository.Insert(details);

                    //    }
                    //}


                    unitOfWork.Save();



                    return Ok(noticeModel);



                }
            }
            else
            {
                return Ok(3);
            }
        }
        #endregion


        #region Update Methods
        [HttpPut]
        [Route("~/api/Notice/Update/{type}/{id}")]
        public IActionResult Update(bool type, int id, [FromBody] NoticeModel noticeModel)
        {
            if (id != noticeModel.NoticeID)
            {

                return Ok(1);
            }

            if (ModelState.IsValid)
            {

                var Check = unitOfWork.NoticeRepository.Get(NoTrack: "NoTrack");
                int portofolioaccount = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == noticeModel.PortfolioID && m.Type == true).Select(m => m.PortfolioAccountID).SingleOrDefault();

                var notice = _mapper.Map<Notice>(noticeModel);
                var NewdDetails = noticeModel.NoticeModelDetails;
                var Newdetails = _mapper.Map<IEnumerable<NoticeDetail>>(NewdDetails);
                var OldDetails = unitOfWork.NoticeDetailRepository.Get(filter: m => m.NoticeID == notice.NoticeID);
                var EntryCheck = unitOfWork.EntryRepository.Get(x => x.NoticeID == notice.NoticeID).SingleOrDefault();
                if (EntryCheck != null)
                {

                    var Entry = unitOfWork.EntryRepository.Get(filter: x => x.NoticeID == notice.NoticeID).SingleOrDefault();
                    var OldEntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
                    if (Entry.TransferedToAccounts == true)
                    {
                        accountingHelper.CancelTransferToAccounts(OldEntryDetails.ToList());
                    }
                    unitOfWork.EntryDetailRepository.RemovRange(OldEntryDetails);

                    if (Check.Any(m => m.Code != notice.Code))
                    {
                        unitOfWork.NoticeRepository.Update(notice);
                        if (OldDetails != null)
                        {
                            unitOfWork.NoticeDetailRepository.RemovRange(OldDetails);
                            unitOfWork.Save();
                        }


                        if (Newdetails != null)
                        {
                            foreach (var item in Newdetails)
                            {
                                item.NoticeID = notice.NoticeID;
                                item.NoticeDetailID = 0;
                                var details = _mapper.Map<NoticeDetail>(item);

                                unitOfWork.NoticeDetailRepository.Insert(details);

                            }
                        }


                        //==================================================لا تولد قيد ===================================
                        //if (noticeModel.SettingModel.DoNotGenerateEntry == true)
                        //{
                        //    unitOfWork.EntryRepository.Delete(Entry.EntryID);
                        //    unitOfWork.Save();

                        //    return Ok(noticeModel);
                        //}
                        //===================================توليد قيد مع ترحيل تلقائي===================================
                        if (noticeModel.SettingModel.AutoGenerateEntry == true)
                        {
                            var EntryDitails = EntriesHelper.UpdateCalculateEntries(portofolioaccount,Entry.EntryID, null, null, null, noticeModel);

                            if (noticeModel.SettingModel.TransferToAccounts == true)
                            {
                                Entry.TransferedToAccounts = true;
                                unitOfWork.EntryRepository.Update(Entry);
                                foreach (var item in EntryDitails)
                                {
                                    item.EntryID = Entry.EntryID;
                                    item.EntryDetailID = 0;
                                    var details = _mapper.Map<EntryDetail>(item);

                                    unitOfWork.EntryDetailRepository.Insert(details);

                                }
                                accountingHelper.TransferToAccounts(EntryDitails.Select(x => new EntryDetail
                                {


                                    EntryDetailID = x.EntryDetailID,
                                    AccountID = x.AccountID,
                                    Credit = x.Credit,
                                    Debit = x.Debit,
                                    EntryID = x.EntryID


                                }).ToList());
                            }
                            else
                            {
                                Entry.TransferedToAccounts = true;
                                unitOfWork.EntryRepository.Insert(Entry);
                                foreach (var item in EntryDitails)
                                {
                                    item.EntryID = Entry.EntryID;
                                    item.EntryDetailID = 0;
                                    var details = _mapper.Map<EntryDetail>(item);

                                    unitOfWork.EntryDetailRepository.Insert(details);
                                }
                            }
                        }
                        //===================================توليد قيد مع  عدم ترحيل=================================== 
                        //if (noticeModel.SettingModel.GenerateEntry==true)
                        
                        //{
                        //    var EntryDitails = EntriesHelper.UpdateCalculateEntries(portofolioaccount,Entry.EntryID, null, null, null, noticeModel);
                        //    Entry.TransferedToAccounts = false;
                        //    unitOfWork.EntryRepository.Update(Entry);
                        //    foreach (var item in EntryDitails)
                        //    {
                        //        item.EntryID = Entry.EntryID;
                        //        item.EntryDetailID = 0;
                        //        var details = _mapper.Map<EntryDetail>(item);

                        //        unitOfWork.EntryDetailRepository.Insert(details);

                        //    }
                        //}

                        unitOfWork.Save();



                        return Ok(noticeModel);


                    }


                    //==========================================Second Case OF Code Of notice=======================================

                    else
                    {
                        if (Check.Any(m => m.Code == notice.Code && m.NoticeID == id))
                        {
                            unitOfWork.NoticeRepository.Update(notice);
                            if (OldDetails != null)
                            {
                                unitOfWork.NoticeDetailRepository.RemovRange(OldDetails);
                                unitOfWork.Save();
                            }


                            if (Newdetails != null)
                            {
                                foreach (var item in Newdetails)
                                {
                                    item.NoticeID = notice.NoticeID;
                                    item.NoticeDetailID = 0;
                                    var details = _mapper.Map<NoticeDetail>(item);

                                    unitOfWork.NoticeDetailRepository.Insert(details);

                                }
                            }


                            //==================================================لا تولد قيد ===================================
                            //if (noticeModel.SettingModel.DoNotGenerateEntry == true)
                            //{
                            //    unitOfWork.EntryRepository.Delete(Entry.EntryID);
                            //    unitOfWork.Save();

                            //    return Ok(noticeModel);
                            //}
                            //===================================توليد قيد مع ترحيل تلقائي===================================
                            if (noticeModel.SettingModel.AutoGenerateEntry == true)
                            {
                                var EntryDitails = EntriesHelper.UpdateCalculateEntries(portofolioaccount,Entry.EntryID, null, null, null, noticeModel);

                                if (noticeModel.SettingModel.TransferToAccounts == true)
                                {
                                    Entry.TransferedToAccounts = true;
                                    unitOfWork.EntryRepository.Update(Entry);
                                    foreach (var item in EntryDitails)
                                    {
                                        item.EntryID = Entry.EntryID;
                                        item.EntryDetailID = 0;
                                        var details = _mapper.Map<EntryDetail>(item);

                                        unitOfWork.EntryDetailRepository.Insert(details);

                                    }
                                    accountingHelper.TransferToAccounts(EntryDitails.Select(x => new EntryDetail
                                    {


                                        EntryDetailID = x.EntryDetailID,
                                        AccountID = x.AccountID,
                                        Credit = x.Credit,
                                        Debit = x.Debit,
                                        EntryID = x.EntryID


                                    }).ToList());
                                }
                                else
                                {
                                    Entry.TransferedToAccounts = true;
                                    unitOfWork.EntryRepository.Insert(Entry);
                                    foreach (var item in EntryDitails)
                                    {
                                        item.EntryID = Entry.EntryID;
                                        item.EntryDetailID = 0;
                                        var details = _mapper.Map<EntryDetail>(item);

                                        unitOfWork.EntryDetailRepository.Insert(details);
                                    }
                                }
                            }
                            //===================================توليد قيد مع  عدم ترحيل=================================== 
                            //if (noticeModel.SettingModel.GenerateEntry == true)

                            //{
                            //    var EntryDitails = EntriesHelper.UpdateCalculateEntries(portofolioaccount, Entry.EntryID, null, null, null, noticeModel);
                            //    Entry.TransferedToAccounts = false;
                            //    unitOfWork.EntryRepository.Update(Entry);
                            //    foreach (var item in EntryDitails)
                            //    {
                            //        item.EntryID = Entry.EntryID;
                            //        item.EntryDetailID = 0;
                            //        var details = _mapper.Map<EntryDetail>(item);

                            //        unitOfWork.EntryDetailRepository.Insert(details);

                            //    }
                            //}

                            unitOfWork.Save();



                            return Ok(noticeModel);

                        }


                    }
                    return Ok(noticeModel);
                }

                // now We Will Create new Entry As Insert


                else
                {
                    if (Check.Any(m => m.Code != notice.Code))
                    {
                        unitOfWork.NoticeRepository.Update(notice);
                        if (OldDetails != null)
                        {
                            unitOfWork.NoticeDetailRepository.RemovRange(OldDetails);
                            unitOfWork.Save();
                        }


                        if (Newdetails != null)
                        {
                            foreach (var item in Newdetails)
                            {
                                item.NoticeID = notice.NoticeID;
                                item.NoticeDetailID = 0;
                                var details = _mapper.Map<NoticeDetail>(item);

                                unitOfWork.NoticeDetailRepository.Insert(details);

                            }
                        }


                        ////==================================================لا تولد قيد ===================================
                        //if (noticeModel.SettingModel.DoNotGenerateEntry == true)
                        //{

                        //    unitOfWork.Save();

                        //    return Ok(noticeModel);
                        //}
                        //===============================================================توليد قيد مع ترحيل تلقائي============================



                       if (noticeModel.SettingModel.AutoGenerateEntry == true)
                       {
                            var lastEntry = unitOfWork.EntryRepository.Last();
                            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, null, null, null, noticeModel, lastEntry);
                            var Entry = _mapper.Map<Entry>(EntryMODEL);
                            Entry.ReceiptID = notice.NoticeID;

                            var DetailEnt = EntryMODEL.EntryDetailModel;

                            if (noticeModel.SettingModel.TransferToAccounts == true)
                            {
                                Entry.TransferedToAccounts = true;
                                unitOfWork.EntryRepository.Insert(Entry);
                                foreach (var item in DetailEnt)
                                {
                                    item.EntryID = Entry.EntryID;
                                    item.EntryDetailID = 0;
                                    var details = _mapper.Map<EntryDetail>(item);

                                    unitOfWork.EntryDetailRepository.Insert(details);

                                }
                                accountingHelper.TransferToAccounts(DetailEnt.Select(x => new EntryDetail
                                {


                                    EntryDetailID = x.EntryDetailID,
                                    AccountID = x.AccountID,
                                    Credit = x.Credit,
                                    Debit = x.Debit,
                                    EntryID = x.EntryID


                                }).ToList());
                            }

                            else
                            {
                                Entry.TransferedToAccounts = true;
                                unitOfWork.EntryRepository.Insert(Entry);
                                foreach (var item in DetailEnt)
                                {
                                    item.EntryID = Entry.EntryID;
                                    item.EntryDetailID = 0;
                                    var details = _mapper.Map<EntryDetail>(item);

                                    unitOfWork.EntryDetailRepository.Insert(details);
                                }
                            }
                        }
                        //================================توليد قيد مع عدم الترحيل====================================== 
                        //if (noticeModel.SettingModel.GenerateEntry == true)

                        //{

                        //    var lastEntry = unitOfWork.EntryRepository.Last();
                        //    var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, null, null, null, noticeModel, lastEntry);
                        //    var Entry = _mapper.Map<Entry>(EntryMODEL);
                        //    Entry.NoticeID = notice.NoticeID;

                        //    var DetailEnt = EntryMODEL.EntryDetailModel;
                        //    Entry.TransferedToAccounts = false;
                        //    unitOfWork.EntryRepository.Insert(Entry);
                        //    foreach (var item in DetailEnt)
                        //    {
                        //        item.EntryID = Entry.EntryID;
                        //        item.EntryDetailID = 0;
                        //        var details = _mapper.Map<EntryDetail>(item);

                        //        unitOfWork.EntryDetailRepository.Insert(details);

                        //    }
                        //}


                        unitOfWork.Save();



                        return Ok(noticeModel);


                    }


                    //==========================================Second Case OF Code Of notice=======================================

                    else
                    {
                        if (Check.Any(m => m.Code == notice.Code && m.NoticeID == id))
                        {
                            unitOfWork.NoticeRepository.Update(notice);
                            if (OldDetails != null)
                            {
                                unitOfWork.NoticeDetailRepository.RemovRange(OldDetails);
                                unitOfWork.Save();
                            }


                            if (Newdetails != null)
                            {
                                foreach (var item in Newdetails)
                                {
                                    item.NoticeID = notice.NoticeID;
                                    item.NoticeDetailID = 0;
                                    var details = _mapper.Map<ReceiptExchangeDetail>(item);

                                    unitOfWork.ReceiptExchangeDetailRepository.Insert(details);

                                }
                            }


                            //==================================================لا تولد قيد ===================================
                            //if (noticeModel.SettingModel.DoNotGenerateEntry == true)
                            //{

                            //    unitOfWork.Save();

                            //    return Ok(noticeModel);
                            //}
                            //===============================================================توليد قيد مع ترحيل تلقائي============================



                            if (noticeModel.SettingModel.AutoGenerateEntry == true)
                            {
                                var lastEntry = unitOfWork.EntryRepository.Last();
                                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,null, null, null, noticeModel, lastEntry);
                                var Entry = _mapper.Map<Entry>(EntryMODEL);
                                Entry.NoticeID = notice.NoticeID;

                                var DetailEnt = EntryMODEL.EntryDetailModel;

                                if (noticeModel.SettingModel.TransferToAccounts == true)
                                {
                                    Entry.TransferedToAccounts = true;
                                    unitOfWork.EntryRepository.Insert(Entry);
                                    foreach (var item in DetailEnt)
                                    {
                                        item.EntryID = Entry.EntryID;
                                        item.EntryDetailID = 0;
                                        var details = _mapper.Map<EntryDetail>(item);

                                        unitOfWork.EntryDetailRepository.Insert(details);

                                    }
                                    accountingHelper.TransferToAccounts(DetailEnt.Select(x => new EntryDetail
                                    {


                                        EntryDetailID = x.EntryDetailID,
                                        AccountID = x.AccountID,
                                        Credit = x.Credit,
                                        Debit = x.Debit,
                                        EntryID = x.EntryID


                                    }).ToList());
                                }

                                else
                                {
                                    Entry.TransferedToAccounts = true;
                                    unitOfWork.EntryRepository.Insert(Entry);
                                    foreach (var item in DetailEnt)
                                    {
                                        item.EntryID = Entry.EntryID;
                                        item.EntryDetailID = 0;
                                        var details = _mapper.Map<EntryDetail>(item);

                                        unitOfWork.EntryDetailRepository.Insert(details);
                                    }
                                }
                            }
                            //================================توليد قيد مع عدم الترحيل====================================== 
                            //if (noticeModel.SettingModel.GenerateEntry == true)

                            //{

                            //    var lastEntry = unitOfWork.EntryRepository.Last();
                            //    var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, null, null, null, noticeModel, lastEntry);
                            //    var Entry = _mapper.Map<Entry>(EntryMODEL);
                            //    Entry.NoticeID = notice.NoticeID;

                            //    var DetailEnt = EntryMODEL.EntryDetailModel;
                            //    Entry.TransferedToAccounts = false;
                            //    unitOfWork.EntryRepository.Insert(Entry);
                            //    foreach (var item in DetailEnt)
                            //    {
                            //        item.EntryID = Entry.EntryID;
                            //        item.EntryDetailID = 0;
                            //        var details = _mapper.Map<EntryDetail>(item);

                            //        unitOfWork.EntryDetailRepository.Insert(details);

                            //    }
                            //}


                            unitOfWork.Save();



                            return Ok(noticeModel);
                        }


                    }
                    return Ok(noticeModel);
                }
            }
            else
            {
                return Ok(3);
            }
        }
        #endregion


        #region Delete Methods

        [HttpDelete]
        [Route("~/api/Notice/Delete/{id}")]
        public IActionResult Delete(int? id)
        {

          
            if (id>0)
            {
                var notice = unitOfWork.NoticeRepository.GetByID(id);

                if (notice == null)
                {
                    return Ok(0);
                }
                var noticeDetails = unitOfWork.NoticeDetailRepository.Get(filter: m => m.NoticeID == id);



                unitOfWork.NoticeDetailRepository.RemovRange(noticeDetails);
                var entry = unitOfWork.EntryRepository.Get(x => x.NoticeID == id).SingleOrDefault();
                var entryDitails = unitOfWork.EntryDetailRepository.Get(a => a.EntryID == entry.EntryID);
                if (entry.TransferedToAccounts == true)
                {
                    accountingHelper.CancelTransferToAccounts(entryDitails.ToList());
                }
                unitOfWork.EntryDetailRepository.RemovRange(entryDitails);
                unitOfWork.EntryRepository.Delete(entry.EntryID);
                unitOfWork.NoticeRepository.Delete(notice);
               
                    unitOfWork.Save();
             
                return Ok(4);

            }
            else
                return Ok(1);


        }

        #endregion
    }
}