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

        #region Generate Entry

        public IActionResult GenerateEntry()
        {
            var settingObj = setting.GetSpecificSetting(1);
            return Ok();
        }
        #endregion

        #region Default Setting
        public SettingModel DefaultSetting(bool type)
        {
            var screenNo = 5;
            if (type)
                screenNo = 6;

            var settingObj = setting.GetSpecificSetting(screenNo);
           
            return settingObj;
        }
        #endregion

        #region GET Methods

        public NoticeModel GetNotice(Notice notice, bool type)
        {
            var model = _mapper.Map<NoticeModel>(notice);
            if (model == null)
            {
                return model;
            }

            #region Date part

            model.NoticeDate = notice.NoticeDate.Value.ToString("dd/MM/yyyy");
            model.NoticeDateHijri = DateHelper.GetHijriDate(notice.NoticeDate);
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
                    AccNameEN=m.Account.NameEN
                });
            if (Details != null)
                model.NoticeModelDetails = Details;

            #endregion

            model.Count = unitOfWork.NoticeRepository.Get(filter: m => m.Type == type).Count();

            model.PortfolioNameAR = notice.Portfolio.NameAR;
            model.PortfolioNameEN = notice.Portfolio.NameEN;
            model.PortfolioCode = notice.Portfolio.Code;

            model.EmployeeNameAR = notice.Employee.NameAR;
            model.EmployeeNameEN = notice.Employee.NameEN;
            model.EmployeeCode = notice.Employee.Code;

            #region Setting part
           
            model.SettingModel = DefaultSetting(type);

            #endregion

            return model;
        }


        [HttpGet]
        [Route("~/api/Notice/FirstOpen/{type}")]
        public IActionResult FirstOpen(bool type)
        {
            DefaultSettingModel model = new DefaultSettingModel();
            model.ScreenSetting = DefaultSetting(type);
            model.LastCode = unitOfWork.EntryRepository.Last().Code;
            
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
        [Route("~/api/Notice/Paging/{pageNumber}/{type}")]
        public IActionResult Pagination(int pageNumber, bool type)
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
        [Route("~/api/Notice/Get/{id}/{type}")]

        public IActionResult GetById(int id, bool type)
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




        [HttpGet]//القيد
        [Route("~/api/Notice/GetEntry")]
        public IActionResult GetEntry(NoticeModel noticeModel)
        {
            var Entry = unitOfWork.EntryRepository.Get(x => x.NoticeID == noticeModel.NoticeID).SingleOrDefault();
            var EntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
            EntryModel entryModel = new EntryModel();
            entryModel.EntryID = Entry.EntryID;
            entryModel.Code = Entry.Code;
            entryModel.Date = Entry.Date.ToString();
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
                Credit = m.Credit,
                Debit = m.Debit,
                EntryDetailID = m.EntryDetailID,
                EntryID = m.EntryID,


            });

            return Ok(entryModel);
        }

        [HttpPost]// ترحيل يدوي للقيد اليدوي والتلقائي
        [Route("~/api/Notice/Manualmigration")]
        public IActionResult Manualmigration(EntryModel EntryMODEL)
        {
            var Entry = unitOfWork.EntryRepository.GetByID(EntryMODEL.EntryID);
            Entry.TransferedToAccounts = true;
            unitOfWork.EntryRepository.Update(Entry);
            var Details = EntryMODEL.EntryDetailModel;
            foreach (var item in Details)
            {
                var detail = _mapper.Map<NoticeDetail>(item);

                unitOfWork.NoticeDetailRepository.Update(detail);
            }

            accountingHelper.TransferToAccounts(Details.Select(x => new EntryDetail
            {
                EntryDetailID = x.EntryDetailID,
                AccountID = x.AccountID,
                Credit = x.Credit,
                Debit = x.Debit,
                EntryID = x.EntryID


            }).ToList());




            unitOfWork.Save();



            return Ok("تم ترحيل القيد");

        }




        [HttpPost] // يولد قيد مع ترحيل تلقائي
        [Route("~/api/Notice/Generateconstraint")]
        public IActionResult Generateconstraint(NoticeModel noticeModel)
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

            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(null, null, null, noticeModel,lastEntry);
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
        public IActionResult PostItem([FromBody] NoticeModel Model)
        {

            if (ModelState.IsValid)
            {

                var Check = unitOfWork.NoticeRepository.Get();
                if (Model == null)
                {
                    return Ok(0);
                }
                if (Check.Any(m => m.Code == Model.Code))
                {
                    return Ok(2);
                }
                else
                {
                    // map model to entity
                    var model = _mapper.Map<Notice>(Model);


                    //var Details = Model.NoticeModelDetails;

                    var Details = _mapper.Map<IEnumerable<NoticeDetail>>(Model.NoticeModelDetails);
                    try
                    {
                        // insert main data of receipt exchange 
                        unitOfWork.NoticeRepository.Insert(model);

                        if (Details != null)
                        {
                            foreach (var item in Model.NoticeModelDetails)
                            {
                                var obj = _mapper.Map<NoticeDetail>(item);
                                obj.NoticeID = model.NoticeID;
                                unitOfWork.NoticeDetailRepository.Insert(obj);
                            }
                        }


                        try
                        {

                            #region Generate entry
                            //===============================================================توليد قيد مع ترحيل تلقائي============================

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

                            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(null, null, null, Model, lastEntry);
                            var Entry = _mapper.Map<Entry>(EntryMODEL);


                            var DetailEnt = EntryMODEL.EntryDetailModel;

                            if (Model.SettingModel.TransferToAccounts == true)
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


                            unitOfWork.Save();


                            #endregion
                        }
                        catch (DbUpdateException ex)
                        {
                            var sqlException = ex.GetBaseException() as SqlException;

                            if (sqlException != null)
                            {
                                var number = sqlException.Number;

                                if (number == 547)
                                {
                                    return Ok(5);

                                }
                                else
                                    return Ok(6);
                            }
                        }

                        return Ok(Model);

                    }
                    catch (Exception ex)
                    {
                        return Ok("خطأ في ادخال البيانات");

                    }


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
        [Route("~/api/Notice/Update/{id}/{type}")]
        public IActionResult Update(int id, bool type, [FromBody] NoticeModel Model)
        {
            if (id != Model.NoticeID)
            {

                return Ok(1);
            }

            if (ModelState.IsValid)
            {
                // map main data to entity
                var model = _mapper.Map<Notice>(Model);

                var Detail = Model.NoticeModelDetails;
                //var EmpolyeeCard = _mapper.Map<IEnumerable<EmployeeCard>>(empolyeeCard);

                var Check = unitOfWork.NoticeRepository.Get(NoTrack: "NoTrack", filter: m => m.Type == type);
                var oldDetail = unitOfWork.NoticeDetailRepository.Get(NoTrack: "NoTrack", filter: m => m.NoticeID == model.NoticeID);
                 var Entry = unitOfWork.EntryRepository.Get(filter: x => x.NoticeID == model.NoticeID).SingleOrDefault();
                var OldEntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);

                if (oldDetail != null)
                {
                    unitOfWork.NoticeDetailRepository.RemovRange(oldDetail); 
                } 
                if (Entry.TransferedToAccounts==true)
                {
                    accountingHelper.CancelTransferToAccounts(OldEntryDetails.ToList());
                }
                    unitOfWork.EntryDetailRepository.RemovRange(OldEntryDetails);
                
             //   unitOfWork.EntryRepository.Delete(OldEntry.EntryID);


                if (Check.Any(m => m.Code != Model.Code))
                {
                    unitOfWork.NoticeRepository.Update(model);


                    foreach (var item in Detail)
                    {
                        item.NoticeID = model.NoticeID;
                        item.NoticeDetailID = 0;
                        var newDetail = _mapper.Map<NoticeDetail>(item);

                        unitOfWork.NoticeDetailRepository.Insert(newDetail);

                    }

                    try
                    {

                        #region Generate entry

                       

                        var DetailEnt = EntriesHelper.UpdateCalculateEntries(Entry.EntryID,null,null,null, Model);
                       
                        if (Model.SettingModel.TransferToAccounts == true)
                        {
                            Entry.TransferedToAccounts = true;
                            unitOfWork.EntryRepository.Update(Entry);
                            foreach (var item in DetailEnt)
                            {
                               // item.EntryID = Entry.EntryID;
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
                            unitOfWork.EntryRepository.Update(Entry);
                            foreach (var item in DetailEnt)
                            {
                              //  item.EntryID = Entry.EntryID;
                                item.EntryDetailID = 0;
                                var details = _mapper.Map<NoticeDetail>(item);

                                unitOfWork.NoticeDetailRepository.Insert(details);

                            }
                        }



                        unitOfWork.Save();

                        #endregion
                    }
                    catch (DbUpdateException ex)
                    {
                        var sqlException = ex.GetBaseException() as SqlException;

                        if (sqlException != null)
                        {
                            var number = sqlException.Number;

                            if (number == 547)
                            {
                                return Ok(5);

                            }
                            else
                                return Ok(6);
                        }
                    }

                    return Ok(Model);
                }
                else
                {
                    if (Check.Any(m => m.Code == Model.Code && m.NoticeID == id))
                    {

                        unitOfWork.NoticeRepository.Update(model);

                        foreach (var item in Detail)
                        {
                            item.NoticeID = model.NoticeID;
                            item.NoticeDetailID = 0;
                            var neweDetail = _mapper.Map<NoticeDetail>(item);

                            unitOfWork.NoticeDetailRepository.Insert(neweDetail);

                        }

                        try
                        {

                            #region Generate entry
                            

                            var DetailEnt = EntriesHelper.UpdateCalculateEntries(Entry.EntryID, null, null, null, Model);

                            if (Model.SettingModel.TransferToAccounts == true)
                            {
                                Entry.TransferedToAccounts = true;
                                unitOfWork.EntryRepository.Update(Entry);
                                foreach (var item in DetailEnt)
                                {
                                   // item.EntryID = Entry.EntryID;
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



                            unitOfWork.Save();

                            #endregion
                        }
                        catch (DbUpdateException ex)
                        {
                            var sqlException = ex.GetBaseException() as SqlException;

                            if (sqlException != null)
                            {
                                var number = sqlException.Number;

                                if (number == 547)
                                {
                                    return Ok(5);

                                }
                                else
                                    return Ok(6);
                            }
                        }

                        return Ok(Model);
                    }
                    else
                    {
                        return Ok(2);
                    }
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

            //var RecExc = unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type && m.ReceiptID == id).FirstOrDefault();
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
                try
                {
                    unitOfWork.Save();
                }
                catch (DbUpdateException ex)
                {
                    var sqlException = ex.GetBaseException() as SqlException;

                    if (sqlException != null)
                    {
                        var number = sqlException.Number;

                        if (number == 547)
                        {
                            return Ok(5);

                        }
                        else
                            return Ok(6);
                    }
                }
                return Ok(4);

                //var Result = unitOfWork.Save();
                //if (Result == true)
                //{
                //    return Ok(4);
                //}
                //else
                //{
                //    return Ok("not deleted");
                //} 
            }
            else
                return Ok(1);


        }

        #endregion
    }
}