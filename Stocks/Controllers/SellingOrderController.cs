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



        [HttpGet]//القيد
        [Route("~/api/SellingOrder/GetEntry")]
        public EntryModel GetEntry(int sellingOrderID)
        {
            var Entry = unitOfWork.EntryRepository.Get(x => x.SellingOrderID == sellingOrderID).SingleOrDefault();
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

            return entryModel;
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







        [HttpPost] // يولد قيد يدوي مع ترحيل تلقائي
        [Route("~/api/sellingOrderModel/GenerateconstraintManual")]
        public IActionResult GenerateconstraintManual([FromBody] SellingOrderModel sellingOrderModel)
        {
            if (sellingOrderModel.SettingModel.GenerateEntry == true)
            {
                var lastEntry = unitOfWork.EntryRepository.Last();


                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(sellingOrderModel, null, null, null, lastEntry);
                var Entry = _mapper.Map<Entry>(EntryMODEL);


                var DetailEnt = EntryMODEL.EntryDetailModel;

                if (sellingOrderModel.SettingModel.TransferToAccounts == true)
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
                    Entry.TransferedToAccounts = false;
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


            unitOfWork.Save();



            return Ok(sellingOrderModel);

        }



        [HttpPost]// ترحيل يدوي للقيد اليدوي والتلقائي
        [Route("~/api/SellingOrder/ManualmigrationSellingOrder")]
        public IActionResult ManualmigrationSellingOrder([FromBody]EntryModel entryModel)
        {
            //var Entry = unitOfWork.EntryRepository.GetByID(EntryMODEL.EntryID);
            //Entry.TransferedToAccounts = true;
            //unitOfWork.EntryRepository.Update(Entry);
            var Details = entryModel.EntryDetailModel; 
            //foreach (var item in Details)
            //{
            //    var detail = _mapper.Map<SellingOrderDetail>(item);

            //    unitOfWork.SellingOrderDetailRepository.Update(detail);
            //}

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


        [HttpGet]
        [Route("~/api/SellingOrder/GetLastSellingOrder")]
        public IActionResult GetLastSellingOrder()
        {
            var selling = unitOfWork.SellingOrderReposetory.Last();


            var sellingOrderModel = _mapper.Map<SellingOrderModel>(selling);
            sellingOrderModel.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == selling.PortfolioID && x.Type == true).AccountID;
            if (sellingOrderModel == null)
            {
                return Ok(sellingOrderModel);

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


                sellingOrderModel.DetailsModels = Details;
            }

            sellingOrderModel.SettingModel = GetSetting(1);

            var check = unitOfWork.EntryRepository.Get(x => x.SellingOrderID == selling.SellingOrderID).SingleOrDefault();
            if (check != null)
            {
                sellingOrderModel.EntryModel = GetEntry(selling.SellingOrderID);
            }
            return Ok(sellingOrderModel);

        }



        [HttpGet]
        [Route("~/api/SellingOrder/GetSellingOrderByID/{id}")]
        public IActionResult GetSellingOrderByID(int id)
        {
            var selling = unitOfWork.SellingOrderReposetory.GetByID(id);


            var model = _mapper.Map<SellingOrderModel>(selling);
            model.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == selling.PortfolioID && x.Type == true).AccountID;
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
            var check = unitOfWork.EntryRepository.Get(x => x.SellingOrderID == selling.SellingOrderID).SingleOrDefault();
            if (check != null)
            {
                model.EntryModel = GetEntry(selling.SellingOrderID);
            }

            return Ok(model);

        }


        [HttpGet]
        [Route("~/api/Pagination/SellingOrder/{pageNumber}")]
        public IActionResult PaginationSellingOrder(int pageNumber)
        {
            var selling = unitOfWork.SellingOrderReposetory.Get(page: pageNumber).FirstOrDefault();

            var model = _mapper.Map<SellingOrderModel>(selling);
            model.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == selling.PortfolioID && x.Type == true).AccountID;
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

            var check = unitOfWork.EntryRepository.Get(x => x.SellingOrderID == selling.SellingOrderID).SingleOrDefault();
            if (check != null)
            {
                model.EntryModel = GetEntry(selling.SellingOrderID);
            }

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

                    //==================================================لا تولد قيد ===================================
                    if (sellingOrderModel.SettingModel.DoNotGenerateEntry == true)
                    {
                        unitOfWork.Save();

                        return Ok(sellingOrderModel);
                    }

                    //===============================================================توليد قيد مع ترحيل تلقائي============================
                    else if (sellingOrderModel.SettingModel.AutoGenerateEntry == true)
                    {
                        var lastEntry = unitOfWork.EntryRepository.Last();
                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(sellingOrderModel, null, null, null, lastEntry);
                        EntryMODEL.SellingOrderID = modelselling.SellingOrderID;
                        var Entry = _mapper.Map<Entry>(EntryMODEL);
                        var DetailEnt = EntryMODEL.EntryDetailModel;

                        if (sellingOrderModel.SettingModel.TransferToAccounts == true)
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
                    }
                        //================================توليد قيد مع عدم الترحيل====================================== 
                    else if (sellingOrderModel.SettingModel.GenerateEntry==true )
                        
                        {
                        var lastEntry = unitOfWork.EntryRepository.Last();
                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(sellingOrderModel, null, null, null, lastEntry);
                        EntryMODEL.SellingOrderID = modelselling.SellingOrderID;
                        var Entry = _mapper.Map<Entry>(EntryMODEL);
                        var DetailEnt = EntryMODEL.EntryDetailModel;
                        Entry.TransferedToAccounts = false;
                            unitOfWork.EntryRepository.Insert(Entry);
                            foreach (var item in DetailEnt)
                            {
                                item.EntryID = Entry.EntryID;
                                item.EntryDetailID = 0;
                                var details = _mapper.Map<EntryDetail>(item);

                                unitOfWork.EntryDetailRepository.Insert(details);
                            }
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

                var sellingOrder = _mapper.Map<SellingOrder>(sellingOrderModel);
                var NewdDetails = sellingOrderModel.DetailsModels;
                var Newdetails = _mapper.Map<IEnumerable<SellingOrderDetail>>(NewdDetails);
                var OldDetails = unitOfWork.SellingOrderDetailRepository.Get(filter: m => m.SellingOrderID == sellingOrder.SellingOrderID);
                var EntryCheck = unitOfWork.EntryRepository.Get(x => x.SellingOrderID == sellingOrder.SellingOrderID).SingleOrDefault();
                if (EntryCheck != null)
                {

                    var Entry = unitOfWork.EntryRepository.Get(filter: x => x.SellingOrderID == sellingOrder.SellingOrderID).SingleOrDefault();
                    var OldEntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
                    if (Entry.TransferedToAccounts == true)
                    {
                        accountingHelper.CancelTransferToAccounts(OldEntryDetails.ToList());
                    }
                    unitOfWork.EntryDetailRepository.RemovRange(OldEntryDetails);

                    if (Check.Any(m => m.Code != sellingOrder.Code))
                    {
                        unitOfWork.SellingOrderReposetory.Update(sellingOrder);
                        if (OldDetails != null)
                        {
                            unitOfWork.SellingOrderDetailRepository.RemovRange(OldDetails);
                            unitOfWork.Save();
                        }


                        if (Newdetails != null)
                        {
                            foreach (var item in Newdetails)
                            {
                                item.SellingOrderID = sellingOrder.SellingOrderID;
                                item.SellOrderDetailID = 0;
                                var details = _mapper.Map<SellingOrderDetail>(item);

                                unitOfWork.SellingOrderDetailRepository.Insert(details);

                            }
                        }


                        //==================================================لا تولد قيد ===================================
                        if (sellingOrderModel.SettingModel.DoNotGenerateEntry == true)
                        {
                            unitOfWork.EntryRepository.Delete(Entry.EntryID);
                            unitOfWork.Save();

                            return Ok(sellingOrderModel);
                        }
                        //===================================توليد قيد مع ترحيل تلقائي===================================
                        if (sellingOrderModel.SettingModel.AutoGenerateEntry == true)
                        {
                            var EntryDitails = EntriesHelper.UpdateCalculateEntries(Entry.EntryID, sellingOrderModel, null, null, null);

                            if (sellingOrderModel.SettingModel.TransferToAccounts == true)
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
                           
                        }
                        //===================================توليد قيد مع  عدم ترحيل=================================== 
                        if (sellingOrderModel.SettingModel.GenerateEntry==true)
                        
                        {
                            var EntryDitails = EntriesHelper.UpdateCalculateEntries(Entry.EntryID, sellingOrderModel, null, null, null);
                            Entry.TransferedToAccounts = false;
                            unitOfWork.EntryRepository.Update(Entry);
                            foreach (var item in EntryDitails)
                            {
                                item.EntryID = Entry.EntryID;
                                item.EntryDetailID = 0;
                                var details = _mapper.Map<EntryDetail>(item);

                                unitOfWork.EntryDetailRepository.Insert(details);

                            }
                        }
                        unitOfWork.Save();



                        return Ok(sellingOrderModel);


                    }


                    //==========================================Second Case OF Code Of Purchase=======================================

                    else
                    {
                        if (Check.Any(m => m.Code == sellingOrder.Code && m.SellingOrderID == id))
                        {
                            unitOfWork.SellingOrderReposetory.Update(sellingOrder);
                            if (OldDetails != null)
                            {
                                unitOfWork.SellingOrderDetailRepository.RemovRange(OldDetails);
                                unitOfWork.Save();
                            }


                            if (Newdetails != null)
                            {
                                foreach (var item in Newdetails)
                                {
                                    item.SellingOrderID = sellingOrder.SellingOrderID;
                                    item.SellOrderDetailID = 0;
                                    var details = _mapper.Map<SellingOrderDetail>(item);

                                    unitOfWork.SellingOrderDetailRepository.Insert(details);

                                }
                            }


                            //==================================================لا تولد قيد ===================================
                            if (sellingOrderModel.SettingModel.DoNotGenerateEntry == true)
                            {
                                unitOfWork.EntryRepository.Delete(Entry.EntryID);
                                unitOfWork.Save();

                                return Ok(sellingOrderModel);
                            }
                            //===================================توليد قيد مع ترحيل تلقائي===================================
                            if (sellingOrderModel.SettingModel.AutoGenerateEntry == true)
                            {
                                var EntryDitails = EntriesHelper.UpdateCalculateEntries(Entry.EntryID, sellingOrderModel, null, null, null);

                                if (sellingOrderModel.SettingModel.TransferToAccounts == true)
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
                               
                            }
                            //===================================توليد قيد مع  عدم ترحيل=================================== 
                            if (sellingOrderModel.SettingModel.GenerateEntry == true)

                            {
                                var EntryDitails = EntriesHelper.UpdateCalculateEntries(Entry.EntryID, sellingOrderModel, null, null, null);
                                Entry.TransferedToAccounts = false;
                                unitOfWork.EntryRepository.Update(Entry);
                                foreach (var item in EntryDitails)
                                {
                                    item.EntryID = Entry.EntryID;
                                    item.EntryDetailID = 0;
                                    var details = _mapper.Map<EntryDetail>(item);

                                    unitOfWork.EntryDetailRepository.Insert(details);

                                }
                            }
                            unitOfWork.Save();



                            return Ok(sellingOrderModel);

                        }


                    }
                    return Ok(sellingOrderModel);
                }

                // now We Will Create new Entry As Insert


                else
                {
                    if (Check.Any(m => m.Code != sellingOrder.Code))
                    {
                        unitOfWork.SellingOrderReposetory.Update(sellingOrder);
                        if (OldDetails != null)
                        {
                            unitOfWork.SellingOrderDetailRepository.RemovRange(OldDetails);
                            unitOfWork.Save();
                        }


                        if (Newdetails != null)
                        {
                            foreach (var item in Newdetails)
                            {
                                item.SellingOrderID = sellingOrder.SellingOrderID;
                                item.SellOrderDetailID = 0;
                                var details = _mapper.Map<SellingOrderDetail>(item);

                                unitOfWork.SellingOrderDetailRepository.Insert(details);

                            }
                        }


                        //==================================================لا تولد قيد ===================================
                        if (sellingOrderModel.SettingModel.DoNotGenerateEntry == true)
                        {

                            unitOfWork.Save();

                            return Ok(sellingOrderModel);
                        }
                        //===============================================================توليد قيد مع ترحيل تلقائي============================



                        else if (sellingOrderModel.SettingModel.AutoGenerateEntry == true)
                        {
                            var lastEntry = unitOfWork.EntryRepository.Last();
                            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(sellingOrderModel, null, null, null, lastEntry);
                            EntryMODEL.SellingOrderID = sellingOrder.SellingOrderID;
                            var Entry = _mapper.Map<Entry>(EntryMODEL);
                            Entry.SellingOrderID = sellingOrder.SellingOrderID;

                            var DetailEnt = EntryMODEL.EntryDetailModel;

                            if (sellingOrderModel.SettingModel.TransferToAccounts == true)
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
                            //================================توليد قيد مع عدم الترحيل======================================
                           

                        }
                        if (sellingOrderModel.SettingModel.GenerateEntry==true)
                        
                        {
                            var lastEntry = unitOfWork.EntryRepository.Last();
                            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(sellingOrderModel, null, null, null, lastEntry);
                            EntryMODEL.SellingOrderID = sellingOrder.SellingOrderID;
                            var Entry = _mapper.Map<Entry>(EntryMODEL);
                            Entry.SellingOrderID = sellingOrder.SellingOrderID;

                            var DetailEnt = EntryMODEL.EntryDetailModel;
                            Entry.TransferedToAccounts = false;
                            unitOfWork.EntryRepository.Insert(Entry);
                            foreach (var item in DetailEnt)
                            {
                                item.EntryID = Entry.EntryID;
                                item.EntryDetailID = 0;
                                var details = _mapper.Map<EntryDetail>(item);

                                unitOfWork.EntryDetailRepository.Insert(details);

                            }
                        }


                        unitOfWork.Save();



                        return Ok(sellingOrderModel);


                    }


                    //==========================================Second Case OF Code Of Purchase=======================================

                    else
                    {
                        if (Check.Any(m => m.Code == sellingOrder.Code && m.SellingOrderID == id))
                        {
                            unitOfWork.SellingOrderReposetory.Update(sellingOrder);
                            if (OldDetails != null)
                            {
                                unitOfWork.SellingOrderDetailRepository.RemovRange(OldDetails);
                                unitOfWork.Save();
                            }


                            if (Newdetails != null)
                            {
                                foreach (var item in Newdetails)
                                {
                                    item.SellingOrderID = sellingOrder.SellingOrderID;
                                    item.SellOrderDetailID = 0;
                                    var details = _mapper.Map<SellingOrderDetail>(item);

                                    unitOfWork.SellingOrderDetailRepository.Insert(details);

                                }
                            }


                            //==================================================لا تولد قيد ===================================
                            if (sellingOrderModel.SettingModel.DoNotGenerateEntry == true)
                            {

                                unitOfWork.Save();

                                return Ok(sellingOrderModel);
                            }
                            //===============================================================توليد قيد مع ترحيل تلقائي============================



                            else if (sellingOrderModel.SettingModel.AutoGenerateEntry == true)
                            {
                                var lastEntry = unitOfWork.EntryRepository.Last();
                                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(sellingOrderModel, null, null, null, lastEntry);
                                var Entry = _mapper.Map<Entry>(EntryMODEL);
                                Entry.SellingOrderID = sellingOrder.SellingOrderID;

                                var DetailEnt = EntryMODEL.EntryDetailModel;

                                if (sellingOrderModel.SettingModel.TransferToAccounts == true)
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
                               
                               

                            }
                            //================================توليد قيد مع عدم الترحيل====================================== 
                            if (sellingOrderModel.SettingModel.GenerateEntry==true)
                           
                            {
                                var lastEntry = unitOfWork.EntryRepository.Last();
                                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(sellingOrderModel, null, null, null, lastEntry);
                                var Entry = _mapper.Map<Entry>(EntryMODEL);
                                Entry.SellingOrderID = sellingOrder.SellingOrderID;

                                var DetailEnt = EntryMODEL.EntryDetailModel;
                                Entry.TransferedToAccounts = false;
                                unitOfWork.EntryRepository.Insert(Entry);
                                foreach (var item in DetailEnt)
                                {
                                    item.EntryID = Entry.EntryID;
                                    item.EntryDetailID = 0;
                                    var details = _mapper.Map<EntryDetail>(item);

                                    unitOfWork.EntryDetailRepository.Insert(details);

                                }
                            }


                            unitOfWork.Save();



                            return Ok(sellingOrderModel);
                        }


                    }
                    return Ok(sellingOrderModel);
                }
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
            if (Entry.TransferedToAccounts==true)
            {
                accountingHelper.CancelTransferToAccounts(EntryDetails.ToList());
            }
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

