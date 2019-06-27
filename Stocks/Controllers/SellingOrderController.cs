using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.Repositories;
using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BAL.Model;
using BAL.Helper;

namespace Stocks.Controllers
{
    public class SellingOrderController : Controller
    {

        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAccountingHelper accountingHelper;
        public SellingOrderController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
            accountingHelper = new AccountingHelper(context,mapper);
        }
        [Route("~/api/SellingOrder/GetSettingAccounts/{id}")]
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
                Code = a.Setting.Code
            });



            return setAccounts;


        }
        

        

        [Route("~/api/SellingOrder/GetSetting")]
        public SettingModel GetSetting(int flag)
        {

            var setsetting = unitOfWork.SettingRepository.Get(filter: x => x.VoucherType == flag).Select(a => new SettingModel
            {

                SettingID = a.SettingID,
                VoucherType = 1,
                AutoGenerateEntry = a.AutoGenerateEntry,
                Code = a.Code,
                DoNotGenerateEntry = a.DoNotGenerateEntry,
                GenerateEntry = a.GenerateEntry,
                SettingAccs = SettingAccounts(a.SettingID),

            }).SingleOrDefault();
            return setsetting;


        }


        [HttpPost] // يولد قيد مع ترحيل تلقائي
        [Route("~/api/SellingOrder/Generateconstraint")]
        public IActionResult Generateconstraint(SellingOrderModel sellingOrderModel)
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
            EntryModel entryModel = new EntryModel();
            entryModel.Code = lastEntry.Code;
            entryModel.Date = lastEntry.Date.ToString();
            entryModel.DateHijri = DateHelper.GetHijriDate(lastEntry.Date);
            entryModel.EntryDetailModel = lastEntryDitails;
            entryModel.EntryID = lastEntry.EntryID;
            entryModel.NoticeID = lastEntry.NoticeID;
            entryModel.PurchaseOrderID = lastEntry.PurchaseOrderID;
            entryModel.ReceiptID = lastEntry.ReceiptID;
            entryModel.SellingOrderID = lastEntry.SellingOrderID;
            var EntryMODEL = EntriesHelper.CalculateSellingEntry(sellingOrderModel, null, entryModel);
            var Entry = _mapper.Map<Entry>(EntryMODEL);
            unitOfWork.EntryRepository.Insert(Entry);

            var Details = EntryMODEL.EntryDetailModel;
            foreach (var item in Details)
            {
                item.EntryID = Entry.EntryID;
                item.EntryDetailID = 0;
                var details = _mapper.Map<EntryDetail>(item);
                unitOfWork.EntryDetailRepository.Insert(details);

            }
            if (sellingOrderModel.SettingModel.TransferToAccounts == true)
            {
                accountingHelper.TransferToAccounts(EntryMODEL.EntryDetailModel.ToList());
            }
            return Ok(sellingOrderModel);
        }


        [HttpPost]//يولد فيد مع ترحيل يدوي
        [Route("~/api/SellingOrder/Manualmigration")]
        public IActionResult Manualmigration(SellingOrderModel sellingOrderModel)
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
            EntryModel entryModel = new EntryModel();
            entryModel.Code = lastEntry.Code;
            entryModel.Date = lastEntry.Date.ToString();
            entryModel.DateHijri = DateHelper.GetHijriDate(lastEntry.Date);
            entryModel.EntryDetailModel = lastEntryDitails;
            entryModel.EntryID = lastEntry.EntryID;
            entryModel.NoticeID = lastEntry.NoticeID;
            entryModel.PurchaseOrderID = lastEntry.PurchaseOrderID;
            entryModel.ReceiptID = lastEntry.ReceiptID;
            entryModel.SellingOrderID = lastEntry.SellingOrderID;
            var EntryMODEL = EntriesHelper.CalculateSellingEntry(sellingOrderModel, null, entryModel);
            var Entry = _mapper.Map<Entry>(EntryMODEL);
            unitOfWork.EntryRepository.Insert(Entry);

            var DetailEnt = EntryMODEL.EntryDetailModel;
            foreach (var item in DetailEnt)
            {
                item.EntryID = Entry.EntryID;
                item.EntryDetailID = 0;
                var details = _mapper.Map<EntryDetail>(item);
                unitOfWork.EntryDetailRepository.Insert(details);

                
            }
            if (sellingOrderModel.SettingModel.TransferToAccounts == false)
            {
                accountingHelper.TransferToAccounts(EntryMODEL.EntryDetailModel.ToList());
            }


            unitOfWork.Save();



            return Ok(sellingOrderModel);

        }


        [HttpGet]
        [Route("~/api/SellingOrder/GetLastSellingOrder")]
        public IActionResult GetLastSellingOrder()
        {
            var selling = unitOfWork.SellingOrderReposetory.Last();


            var model = _mapper.Map<SellingOrderModel>(selling);
            if (model == null)
            {
                return Ok(model);

            }





            var Details = unitOfWork.SellingOrderDetailRepository.Get(filter: a => a.SellingOrderID == selling.SellingOrderID)
                            .Select(m => new SelingOrderDetailsModel
                            {

                                SellingOrderID = m.SellingOrderID,
                                SellOrderDetailID = m.SellOrderDetailID,
                                BankCommission = m.BankCommission,
                                BankCommissionRate = m.BankCommissionRate,
                                NetAmmount = m.NetAmmount,
                                SelingValue = m.SelingValue,
                                SellingPrice = m.SellingPrice,
                                StockCount = m.StockCount,
                                TaxOnCommission = m.TaxOnCommission,
                                TaxRateOnCommission = m.TaxRateOnCommission,
                            });
            if (Details != null)
            {


                model.DetailsModels = Details;
            }

            model.SettingModel = GetSetting(1);
            return Ok(model);

        }



        [HttpGet]
        [Route("~/api/SellingOrder/GetLastSellingOrder/{id}")]
        public IActionResult GetSellingOrderByID(int id)
        {
            var selling = unitOfWork.SellingOrderReposetory.GetByID(id);


            var model = _mapper.Map<SellingOrderModel>(selling);
            if (model == null)
            {
                return Ok(model);

            }




            model.Count = unitOfWork.SellingOrderReposetory.Count();

            if (model.Count == 0)
            {
                return Ok(model);
            }



            var Details = unitOfWork.SellingOrderDetailRepository.Get(filter: a => a.SellingOrderID == selling.SellingOrderID)
                            .Select(m => new SelingOrderDetailsModel
                            {

                                SellingOrderID = m.SellingOrderID,
                                SellOrderDetailID = m.SellOrderDetailID,
                                BankCommission = m.BankCommission,
                                BankCommissionRate = m.BankCommissionRate,
                                NetAmmount = m.NetAmmount,
                                SelingValue = m.SelingValue,
                                SellingPrice = m.SellingPrice,
                                StockCount = m.StockCount,
                                TaxOnCommission = m.TaxOnCommission,
                                TaxRateOnCommission = m.TaxRateOnCommission,
                            });
            if (Details != null)
            {


                model.DetailsModels = Details;
            }

            model.SettingModel = GetSetting(1);

            return Ok(model);

        }


        [HttpGet]
        [Route("~/api/Pagination/SellingOrder/{pageNumber}")]
        public IActionResult PaginationSellingOrder(int pageNumber)
        {
            var selling = unitOfWork.SellingOrderReposetory.Get(page: pageNumber).FirstOrDefault();

            var model = _mapper.Map<SellingOrderModel>(selling);
            if (model == null)
            {
                return Ok(model);

            }


            model.Count = unitOfWork.SellingOrderReposetory.Count();

            if (model.Count == 0)
            {
                return Ok(model);
            }



            var Details = unitOfWork.SellingOrderDetailRepository.Get(filter: a => a.SellingOrderID == selling.SellingOrderID)
                            .Select(m => new SelingOrderDetailsModel
                            {

                                SellingOrderID = m.SellingOrderID,
                                SellOrderDetailID = m.SellOrderDetailID,
                                BankCommission = m.BankCommission,
                                BankCommissionRate = m.BankCommissionRate,
                                NetAmmount = m.NetAmmount,
                                SelingValue = m.SelingValue,
                                SellingPrice = m.SellingPrice,
                                StockCount = m.StockCount,
                                TaxOnCommission = m.TaxOnCommission,
                                TaxRateOnCommission = m.TaxRateOnCommission,
                            });
            if (Details != null)
            {


                model.DetailsModels = Details;
            }

            model.SettingModel = GetSetting(1);

            

            return Ok(model);

        }



        [HttpPost]
        [Route("~/api/SellingOrder/PostSellingOrder")]
        public IActionResult PostSellingOrder([FromBody] SellingOrderModel sellingOrderModel)
        {
            if (ModelState.IsValid)
            {
                var Check = unitOfWork.SellingOrderReposetory.Get();
                if (Check.Any(m => m.Code == sellingOrderModel.Code))
                {

                    return Ok("كود امر بيع مكرر");
                }
                else
                {

                    var modelselling = _mapper.Map<SellingOrder>(sellingOrderModel);


                    var Details = sellingOrderModel.DetailsModels;
                   
                        unitOfWork.SellingOrderReposetory.Insert(modelselling);

                        foreach (var item in Details)
                        {
                            SelingOrderDetailsModel selingOrderDetailsModel = new SelingOrderDetailsModel();
                            selingOrderDetailsModel.SellingOrderID = modelselling.SellingOrderID;
                            selingOrderDetailsModel.NetAmmount = item.NetAmmount;
                            selingOrderDetailsModel.SelingValue = item.SelingValue;
                            selingOrderDetailsModel.SellingPrice = item.SellingPrice;
                            selingOrderDetailsModel.StockCount = item.StockCount;
                            selingOrderDetailsModel.TaxOnCommission = item.TaxOnCommission;
                            selingOrderDetailsModel.TaxRateOnCommission = item.TaxRateOnCommission;
                            selingOrderDetailsModel.BankCommission = item.BankCommission;
                            selingOrderDetailsModel.BankCommissionRate = item.BankCommissionRate;
                           
                            var details = _mapper.Map<SellingOrderDetail>(selingOrderDetailsModel);
                            unitOfWork.SellingOrderDetailRepository.Insert(details);
                           
                        }
                    //===========================================================================================

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
                    EntryModel entryModel = new EntryModel();
                    entryModel.Code = lastEntry.Code;
                    entryModel.Date = lastEntry.Date.ToString();
                    entryModel.DateHijri = DateHelper.GetHijriDate(lastEntry.Date);
                    entryModel.EntryDetailModel = lastEntryDitails;
                    entryModel.EntryID = lastEntry.EntryID;
                    entryModel.NoticeID = lastEntry.NoticeID;
                    entryModel.PurchaseOrderID = lastEntry.PurchaseOrderID;
                    entryModel.ReceiptID = lastEntry.ReceiptID;
                    entryModel.SellingOrderID = lastEntry.SellingOrderID;
                    var EntryMODEL = EntriesHelper.CalculateSellingEntry(sellingOrderModel, null, entryModel);
                    var Entry = _mapper.Map<Entry>(EntryMODEL);
                    unitOfWork.EntryRepository.Insert(Entry);

                    var DetailEnt = EntryMODEL.EntryDetailModel;
                    foreach (var item in DetailEnt)
                    {
                        item.EntryID = Entry.EntryID;
                        item.EntryDetailID = 0;
                        var details = _mapper.Map<EntryDetail>(item);
                        unitOfWork.EntryDetailRepository.Insert(details);

                    }
                    if (sellingOrderModel.SettingModel.TransferToAccounts == true)
                    {
                        accountingHelper.TransferToAccounts(EntryMODEL.EntryDetailModel.ToList());
                    }


                    unitOfWork.Save();



                        return Ok(sellingOrderModel);
                    


                }
            }
            else
            {
                return BadRequest();
            }
        }




        [HttpPut]
        [Route("~/api/SellingOrder/PutSellingOrder/{id}")]
        public IActionResult PutSellingOrder(int id, [FromBody]  SellingOrderModel sellingOrderModel)
        {
            if (id != sellingOrderModel.SellingOrderID)
            {

                return BadRequest();
            }

            if (ModelState.IsValid)
            {

                var Check = unitOfWork.SellingOrderReposetory.Get(NoTrack: "NoTrack");

                var selling = _mapper.Map<SellingOrder>(sellingOrderModel);
                var NewdDetails = sellingOrderModel.DetailsModels;
                var Newdetails = _mapper.Map<IEnumerable<SellingOrderDetail>>(NewdDetails);
                var OldDetails = unitOfWork.SellingOrderDetailRepository.Get(filter: m => m.SellingOrderID == selling.SellingOrderID);

                if (Check.Any(m => m.Code != selling.Code))
                {
                    unitOfWork.SellingOrderReposetory.Update(selling);
                    if (OldDetails != null)
                    {
                        unitOfWork.SellingOrderDetailRepository.RemovRange(OldDetails);
                        unitOfWork.Save();
                    }


                    if (Newdetails != null)
                    {
                        foreach (var item in Newdetails)
                        {
                            item.SellingOrderID = selling.SellingOrderID;
                            item.SellOrderDetailID = 0;
                            var details = _mapper.Map<SellingOrderDetail>(item);

                            unitOfWork.SellingOrderDetailRepository.Insert(details);

                        }
                    }
                   

                }


                //=================================================================================

                else
                {
                    if (Check.Any(m => m.Code == selling.Code && m.SellingOrderID == id))
                    {
                        unitOfWork.SellingOrderReposetory.Update(selling);
                        if (OldDetails != null)
                        {
                            unitOfWork.SellingOrderDetailRepository.RemovRange(OldDetails);
                            unitOfWork.Save();
                        }


                        if (Newdetails != null)
                        {
                            foreach (var item in Newdetails)
                            {
                                item.SellingOrderID = selling.SellingOrderID;
                                item.SellOrderDetailID = 0;
                                var details = _mapper.Map<SellingOrderDetail>(item);

                                unitOfWork.SellingOrderDetailRepository.Insert(details);

                            }
                        }
                      

                       

                    }



                }
                return Ok(sellingOrderModel);
            }
            else
            {
                return BadRequest();
            }


        

        }
   


        [HttpDelete]
        [Route("~/api/DeleteSellingOrder/DeleteSelling/{id}")]
        public IActionResult DeleteSelling(int? id)
        {

            if (id == null)
            {

                return BadRequest();
            }
            var modelSelling = unitOfWork.SellingOrderReposetory.GetByID(id);
            if (modelSelling == null)
            {
                return BadRequest();
            }
            var Details = unitOfWork.SellingOrderDetailRepository.Get(filter: m => m.SellingOrderID == id);
           
            unitOfWork.SellingOrderDetailRepository.RemovRange(Details);
            var Entry = unitOfWork.EntryRepository.Get(filter: x=> x.SellingOrderID==id).SingleOrDefault();
            var EntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a=> a.EntryID==Entry.EntryID);
            unitOfWork.EntryDetailRepository.RemovRange(EntryDetails);
            unitOfWork.EntryRepository.Delete(Entry.EntryID);

            unitOfWork.SellingOrderReposetory.Delete(id);
            var Result = unitOfWork.Save();
            if (Result == true)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }

    }
}

