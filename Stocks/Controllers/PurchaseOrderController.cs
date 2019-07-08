﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DAL.Entities;
using BAL.Helper;

namespace Stocks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAccountingHelper accountingHelper;
        public PurchaseOrderController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
            accountingHelper = new AccountingHelper(context, mapper);
        }

        [HttpGet] //القيد 
        [Route("~/api/PurchaseOrder/GetEntry")]
        public EntryModel GetEntryPurchaseOrderModel( int PurchaseOrderID)
        {
            var Entry = unitOfWork.EntryRepository.Get(x => x.PurchaseOrderID == PurchaseOrderID).SingleOrDefault();
            var EntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a=> a.EntryID==Entry.EntryID);
            EntryModel entryModel = new EntryModel();
            entryModel.EntryID = Entry.EntryID;
            entryModel.Code = Entry.Code;
            entryModel.Date = Entry.Date.ToString();
            entryModel.DateHijri = DateHelper.GetHijriDate(Entry.Date);
            entryModel.NoticeID = Entry.NoticeID;
            entryModel.PurchaseOrderID = Entry.PurchaseOrderID;
            entryModel.ReceiptID = Entry.ReceiptID;
            entryModel.SellingOrderID = Entry.SellingOrderID;
            entryModel.EntryDetailModel = EntryDetails.Select(m=> new EntryDetailModel {
                AccCode=m.Account.Code,
                AccNameAR=m.Account.NameAR,
                AccNameEN=m.Account.NameEN,
                AccountID=m.AccountID,
                Credit=m.Credit,
                Debit=m.Debit,
                EntryDetailID=m.EntryDetailID,
                EntryID=m.EntryID,


            });

            return entryModel;
        }

        [Route("~/api/PurchaseOrder/GetSettingAccounts/{id}")]
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



        [Route("~/api/PurchaseOrder/GetSetting")]
        public SettingModel GetSetting( int flag)
        {

            var setsetting = unitOfWork.SettingRepository.Get(filter: x => x.VoucherType == flag).Select(a => new SettingModel
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





        [HttpPost] // يولد قيد يدوي مع ترحيل تلقائي
        [Route("~/api/PurchaseOrder/GenerateconstraintManual")]
        public IActionResult GenerateconstraintManual([FromBody]PurchaseOrderModel purchaseOrderModel)
        { 
            if (purchaseOrderModel.SettingModel.GenerateEntry==true)
            {
                var lastEntry = unitOfWork.EntryRepository.Last();


                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(null, purchaseOrderModel, null, null, lastEntry);
                var Entry = _mapper.Map<Entry>(EntryMODEL);


                var DetailEnt = EntryMODEL.EntryDetailModel;

                if (purchaseOrderModel.SettingModel.TransferToAccounts == true)
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



            return Ok(purchaseOrderModel);

        }



        [HttpPost]// ترحيل يدوي للقيد اليدوي والتلقائي
        [Route("~/api/PurchaseOrder/Manualmigration")]
        public IActionResult Manualmigration([FromBody]EntryModel EntryModel)
        {
            //var Entry = unitOfWork.EntryRepository.GetByID(EntryMODEL.EntryID);
            //Entry.TransferedToAccounts = true;
            //unitOfWork.EntryRepository.Update(Entry);
            var Details = EntryModel.EntryDetailModel;
            //foreach (var item in Details)
            //{
            //    var detail = _mapper.Map<EntryDetail>(item);

            //    unitOfWork.EntryDetailRepository.Update(detail);
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
        [Route("~/api/PurchaseOrder/GetLastPurchaseOrder")]
        public IActionResult GetLastPurchaseOrder()
        {
            var purchase = unitOfWork.PurchaseOrderRepository.Last();


            var model = _mapper.Map<PurchaseOrderModel>(purchase);
            model.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(x=> x.PortfolioID==purchase.PortfolioID && x.Type==true).AccountID;
            if (model == null)
            {
                return Ok(model);

            }


            model.Count = unitOfWork.PurchaseOrderRepository.Count();

            if (model.Count == 0)
            {
                return Ok(model);
            }
            


            var Details = unitOfWork.PurchaseOrderDetailRepository.Get(filter: a => a.PurchaseID == purchase.PurchaseOrderID)
                            .Select(m => new PurchaseOrderDetailModel
                            {

                                PurchaseID = m.PurchaseID,
                                BankCommission = m.BankCommission,
                                NetAmmount = m.NetAmmount,
                                PurchaseOrderDetailID = m.PurchaseOrderDetailID,
                                BankCommissionRate = m.BankCommissionRate,
                                PurchasePrice = m.PurchasePrice,
                                PurchaseValue = m.PurchaseValue,
                                StockCount = m.StockCount,
                                TaxOnCommission = m.TaxOnCommission,
                                TaxRateOnCommission = m.TaxRateOnCommission,
                            });
            if (Details != null)
            {


                model.DetailsModels = Details;
            }


           model.SettingModel = GetSetting(2);

            var check = unitOfWork.EntryRepository.Get(x=> x.PurchaseOrderID== purchase.PurchaseOrderID).SingleOrDefault();
            if (check !=null)
            {
                model.EntryModel = GetEntryPurchaseOrderModel(purchase.PurchaseOrderID);
            }
          
           
            

            return Ok(model);

        }



        [HttpGet]
        [Route("~/api/PurchaseOrder/GetPurchaseOrderbyID/{id}")]
        public IActionResult GetSPurchaseOrderByID(int id)
        {
            var purchase = unitOfWork.PurchaseOrderRepository.GetByID(id);

            var model = _mapper.Map<PurchaseOrderModel>(purchase);
            model.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == purchase.PortfolioID && x.Type == true).AccountID;
            if (model == null)
            {
                return Ok(model);

            }


            model.Count = unitOfWork.PurchaseOrderRepository.Count();

            if (model.Count == 0)
            {
                return Ok(model);
            }



            var Details = unitOfWork.PurchaseOrderDetailRepository.Get(filter: a => a.PurchaseID == purchase.PurchaseOrderID)
                            .Select(m => new PurchaseOrderDetailModel
                            {

                                PurchaseID = m.PurchaseID,
                                BankCommission = m.BankCommission,
                                NetAmmount = m.NetAmmount,
                                PurchaseOrderDetailID = m.PurchaseOrderDetailID,
                                BankCommissionRate = m.BankCommissionRate,
                                PurchasePrice = m.PurchasePrice,
                                PurchaseValue = m.PurchaseValue,
                                StockCount = m.StockCount,
                                TaxOnCommission = m.TaxOnCommission,
                                TaxRateOnCommission = m.TaxRateOnCommission,
                            });
            if (Details != null)
            {


                model.DetailsModels = Details;
            }


           model.SettingModel = GetSetting(2);

            var check = unitOfWork.EntryRepository.Get(x => x.PurchaseOrderID == purchase.PurchaseOrderID).SingleOrDefault();
            if (check != null)
            {
                model.EntryModel = GetEntryPurchaseOrderModel(purchase.PurchaseOrderID);
            }
            return Ok(model);

        }


        [HttpGet]
        [Route("~/api/Pagination/PurchaseOrder/{pageNumber}")]
        public IActionResult PaginationPurchaseOrder(int pageNumber)
        {
            var purchase = unitOfWork.PurchaseOrderRepository.Get(page: pageNumber).FirstOrDefault();
            var model = _mapper.Map<PurchaseOrderModel>(purchase);
            model.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == purchase.PortfolioID && x.Type == true).AccountID;
            if (model == null)
            {
                return Ok(model);

            }


            model.Count = unitOfWork.PurchaseOrderRepository.Count();

            if (model.Count == 0)
            {
                return Ok(model);
            }



            var Details = unitOfWork.PurchaseOrderDetailRepository.Get(filter: a => a.PurchaseID == purchase.PurchaseOrderID)
                            .Select(m => new PurchaseOrderDetailModel
                            {

                                PurchaseID = m.PurchaseID,
                                BankCommission = m.BankCommission,
                                NetAmmount = m.NetAmmount,
                                PurchaseOrderDetailID = m.PurchaseOrderDetailID,
                                BankCommissionRate = m.BankCommissionRate,
                                PurchasePrice = m.PurchasePrice,
                                PurchaseValue = m.PurchaseValue,
                                StockCount = m.StockCount,
                                TaxOnCommission = m.TaxOnCommission,
                                TaxRateOnCommission = m.TaxRateOnCommission,
                            });
            if (Details != null)
            {


                model.DetailsModels = Details;
            }


            model.SettingModel = GetSetting(2);
            var check = unitOfWork.EntryRepository.Get(x => x.PurchaseOrderID == purchase.PurchaseOrderID).SingleOrDefault();
            if (check != null)
            {
                model.EntryModel = GetEntryPurchaseOrderModel(purchase.PurchaseOrderID);
            }
            return Ok(model);
        }

          

        



        [HttpPost]
        [Route("~/api/PurchaseOrder/PostPurchaseOrder")]
        public IActionResult PostPurchaseOrder([FromBody] PurchaseOrderModel purchaseOrderModel)
        {
            if (ModelState.IsValid)
            {
                var Check = unitOfWork.PurchaseOrderRepository.Get();
                if (Check.Any(m => m.Code == purchaseOrderModel.Code))
                {

                    return Ok("كود امر بيع مكرر");
                }
                else
                {

                    var purchaseOrder = _mapper.Map<PurchaseOrder>(purchaseOrderModel);


                    var Details = purchaseOrderModel.DetailsModels;

                    unitOfWork.PurchaseOrderRepository.Insert(purchaseOrder);

                    foreach (var item in Details)
                    {
                        PurchaseOrderDetailModel purchaseOrderDetailModel = new PurchaseOrderDetailModel();
                        purchaseOrderDetailModel.PurchaseID = purchaseOrder.PurchaseOrderID;
                        purchaseOrderDetailModel.NetAmmount = item.NetAmmount;
                        purchaseOrderDetailModel.StockCount = item.StockCount;
                        purchaseOrderDetailModel.TaxOnCommission = item.TaxOnCommission;
                        purchaseOrderDetailModel.TaxRateOnCommission = item.TaxRateOnCommission;
                        purchaseOrderDetailModel.BankCommission = item.BankCommission;
                        purchaseOrderDetailModel.BankCommissionRate = item.BankCommissionRate;
                        purchaseOrderDetailModel.PurchaseValue = item.PurchaseValue;
                        purchaseOrderDetailModel.PurchasePrice = item.PurchasePrice;



                        var details = _mapper.Map<PurchaseOrderDetail>(purchaseOrderDetailModel);
                        unitOfWork.PurchaseOrderDetailRepository.Insert(details);

                    }


                    //==================================================لا تولد قيد ===================================
                    if (purchaseOrderModel.SettingModel.DoNotGenerateEntry == true)
                    {
                        unitOfWork.Save();
                       
                        return Ok(purchaseOrderModel);
                    }

                    //===============================================================توليد قيد مع ترحيل تلقائي============================



                    else  if (purchaseOrderModel.SettingModel.AutoGenerateEntry==true)
                    {
                        var lastEntry = unitOfWork.EntryRepository.Last();
                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(null, purchaseOrderModel, null, null, lastEntry);
                        EntryMODEL.PurchaseOrderID = purchaseOrder.PurchaseOrderID;
                        var Entry = _mapper.Map<Entry>(EntryMODEL);
                        

                        var DetailEnt = EntryMODEL.EntryDetailModel;

                        if (purchaseOrderModel.SettingModel.TransferToAccounts == true)
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
                    else if (purchaseOrderModel.SettingModel.GenerateEntry == true)

                    {
                        var lastEntry = unitOfWork.EntryRepository.Last();
                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(null, purchaseOrderModel, null, null, lastEntry);
                        EntryMODEL.PurchaseOrderID = purchaseOrder.PurchaseOrderID;
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



                    return Ok(purchaseOrderModel);



                }
            }
            else
            {
                return BadRequest();
            }
        }





        [HttpPut]
        [Route("~/api/PurchaseOrder/PutPurchaseOrder/{id}")]
        public IActionResult PutPurchaseOrder(int id, [FromBody]  PurchaseOrderModel purchaseOrderModel )
        {
            if (id != purchaseOrderModel.PurchaseOrderID)
            {

                return BadRequest();
            }

            if (ModelState.IsValid)
            {

                var Check = unitOfWork.PurchaseOrderRepository.Get(NoTrack: "NoTrack");

                var purchaseOrder = _mapper.Map<PurchaseOrder>(purchaseOrderModel);
                var NewdDetails = purchaseOrderModel.DetailsModels;
                var Newdetails = _mapper.Map<IEnumerable<PurchaseOrderDetail>>(NewdDetails);
                var OldDetails = unitOfWork.PurchaseOrderDetailRepository.Get(filter: m => m.PurchaseID == purchaseOrder.PurchaseOrderID);
                var EntryCheck = unitOfWork.EntryRepository.Get(x => x.PurchaseOrderID == purchaseOrder.PurchaseOrderID).SingleOrDefault();
                if (EntryCheck != null)
                {
                    
                      var Entry = unitOfWork.EntryRepository.Get(filter: x => x.PurchaseOrderID == purchaseOrder.PurchaseOrderID).SingleOrDefault();
                    var OldEntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
                    if (Entry.TransferedToAccounts == true)
                    {
                        accountingHelper.CancelTransferToAccounts(OldEntryDetails.ToList());
                    }
                    unitOfWork.EntryDetailRepository.RemovRange(OldEntryDetails);

                    if (Check.Any(m => m.Code != purchaseOrder.Code))
                    {
                        unitOfWork.PurchaseOrderRepository.Update(purchaseOrder);
                        if (OldDetails != null)
                        {
                            unitOfWork.PurchaseOrderDetailRepository.RemovRange(OldDetails);
                            unitOfWork.Save();
                        }


                        if (Newdetails != null)
                        {
                            foreach (var item in Newdetails)
                            {
                                item.PurchaseID = purchaseOrder.PurchaseOrderID;
                                item.PurchaseOrderDetailID = 0;
                                var details = _mapper.Map<PurchaseOrderDetail>(item);

                                unitOfWork.PurchaseOrderDetailRepository.Insert(details);

                            }
                        }


                        //==================================================لا تولد قيد ===================================
                        if (purchaseOrderModel.SettingModel.DoNotGenerateEntry == true)
                        {
                            unitOfWork.EntryRepository.Delete(Entry.EntryID);
                            unitOfWork.Save();

                            return Ok(purchaseOrderModel);
                        }
                        //===================================توليد قيد مع ترحيل تلقائي===================================
                        if (purchaseOrderModel.SettingModel.AutoGenerateEntry == true)
                        {
                            var EntryDitails = EntriesHelper.UpdateCalculateEntries(Entry.EntryID, null, purchaseOrderModel, null, null);

                            if (purchaseOrderModel.SettingModel.TransferToAccounts == true)
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
                        if (purchaseOrderModel.SettingModel.GenerateEntry==true)
                        
                        {
                            var EntryDitails = EntriesHelper.UpdateCalculateEntries(Entry.EntryID, null, purchaseOrderModel, null, null);
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



                        return Ok(purchaseOrderModel);


                    }


                    //==========================================Second Case OF Code Of Purchase=======================================

                    else
                    {
                        if (Check.Any(m => m.Code == purchaseOrder.Code && m.PurchaseOrderID == id))
                        {
                            unitOfWork.PurchaseOrderRepository.Update(purchaseOrder);
                            if (OldDetails != null)
                            {
                                unitOfWork.PurchaseOrderDetailRepository.RemovRange(OldDetails);
                                unitOfWork.Save();
                            }


                            if (Newdetails != null)
                            {
                                foreach (var item in Newdetails)
                                {
                                    item.PurchaseID = purchaseOrder.PurchaseOrderID;
                                    item.PurchaseOrderDetailID = 0;
                                    var details = _mapper.Map<PurchaseOrderDetail>(item);

                                    unitOfWork.PurchaseOrderDetailRepository.Insert(details);

                                }
                            }


                            //==================================================لا تولد قيد ===================================
                            if (purchaseOrderModel.SettingModel.DoNotGenerateEntry == true)
                            {
                                unitOfWork.EntryRepository.Delete(Entry.EntryID);
                                unitOfWork.Save();

                                return Ok(purchaseOrderModel);
                            }
                            //===================================توليد قيد مع ترحيل تلقائي===================================
                            if (purchaseOrderModel.SettingModel.AutoGenerateEntry == true)
                            {
                                var EntryDitails = EntriesHelper.UpdateCalculateEntries(Entry.EntryID, null, purchaseOrderModel, null, null);

                                if (purchaseOrderModel.SettingModel.TransferToAccounts == true)
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
                            if (purchaseOrderModel.SettingModel.GenerateEntry == true)

                            {
                                var EntryDitails = EntriesHelper.UpdateCalculateEntries(Entry.EntryID, null, purchaseOrderModel, null, null);
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



                            return Ok(purchaseOrderModel);

                        }


                    }
                    return Ok(purchaseOrderModel);
                }

                // now We Will Create new Entry As Insert


                else
                {
                    if (Check.Any(m => m.Code != purchaseOrder.Code))
                    {
                        unitOfWork.PurchaseOrderRepository.Update(purchaseOrder);
                        if (OldDetails != null)
                        {
                            unitOfWork.PurchaseOrderDetailRepository.RemovRange(OldDetails);
                            unitOfWork.Save();
                        }


                        if (Newdetails != null)
                        {
                            foreach (var item in Newdetails)
                            {
                                item.PurchaseID = purchaseOrder.PurchaseOrderID;
                                item.PurchaseOrderDetailID = 0;
                                var details = _mapper.Map<PurchaseOrderDetail>(item);

                                unitOfWork.PurchaseOrderDetailRepository.Insert(details);

                            }
                        }


                        //==================================================لا تولد قيد ===================================
                        if (purchaseOrderModel.SettingModel.DoNotGenerateEntry == true)
                        {
                           
                            unitOfWork.Save();

                            return Ok(purchaseOrderModel);
                        }
                        //===============================================================توليد قيد مع ترحيل تلقائي============================



                        else if (purchaseOrderModel.SettingModel.AutoGenerateEntry == true)
                        {
                            var lastEntry = unitOfWork.EntryRepository.Last();
                            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(null, purchaseOrderModel, null, null, lastEntry);
                            var Entry = _mapper.Map<Entry>(EntryMODEL);
                            Entry.PurchaseOrderID = purchaseOrder.PurchaseOrderID;

                            var DetailEnt = EntryMODEL.EntryDetailModel;

                            if (purchaseOrderModel.SettingModel.TransferToAccounts == true)
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



                        return Ok(purchaseOrderModel);


                    }


                    //==========================================Second Case OF Code Of Purchase=======================================

                    else
                    {
                        if (Check.Any(m => m.Code == purchaseOrder.Code && m.PurchaseOrderID == id))
                        {
                            unitOfWork.PurchaseOrderRepository.Update(purchaseOrder);
                            if (OldDetails != null)
                            {
                                unitOfWork.PurchaseOrderDetailRepository.RemovRange(OldDetails);
                                unitOfWork.Save();
                            }


                            if (Newdetails != null)
                            {
                                foreach (var item in Newdetails)
                                {
                                    item.PurchaseID = purchaseOrder.PurchaseOrderID;
                                    item.PurchaseOrderDetailID = 0;
                                    var details = _mapper.Map<PurchaseOrderDetail>(item);

                                    unitOfWork.PurchaseOrderDetailRepository.Insert(details);

                                }
                            }


                            //==================================================لا تولد قيد ===================================
                            if (purchaseOrderModel.SettingModel.DoNotGenerateEntry == true)
                            {
                                
                                unitOfWork.Save();

                                return Ok(purchaseOrderModel);
                            }
                            //===============================================================توليد قيد مع ترحيل تلقائي============================



                            else if (purchaseOrderModel.SettingModel.AutoGenerateEntry == true)
                            {
                                var lastEntry = unitOfWork.EntryRepository.Last();
                                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(null, purchaseOrderModel, null, null, lastEntry);
                                purchaseOrder.PurchaseOrderID = purchaseOrder.PurchaseOrderID;
                                var Entry = _mapper.Map<Entry>(EntryMODEL);
                                Entry.PurchaseOrderID = purchaseOrder.PurchaseOrderID;

                                var DetailEnt = EntryMODEL.EntryDetailModel;

                                if (purchaseOrderModel.SettingModel.TransferToAccounts == true)
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
                            if (purchaseOrderModel.SettingModel.GenerateEntry==true)
                            { 
                                var lastEntry = unitOfWork.EntryRepository.Last();
                                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(null, purchaseOrderModel, null, null, lastEntry);
                                purchaseOrder.PurchaseOrderID = purchaseOrder.PurchaseOrderID;
                                var Entry = _mapper.Map<Entry>(EntryMODEL);
                                Entry.PurchaseOrderID = purchaseOrder.PurchaseOrderID;

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



                            return Ok(purchaseOrderModel);
                        }


                    }
                    return Ok(purchaseOrderModel);
                }    
            }
            else
            {
                return BadRequest();
            }




        }







        [HttpDelete]
        [Route("~/api/DeletePurchaseOrder/DeletePurchase/{id}")]
        public IActionResult DeletePurchase(int? id)
        {

            if (id == null)
            {

                return BadRequest();
            }
            var modelPurchase = unitOfWork.PurchaseOrderRepository.GetByID(id);
            if (modelPurchase == null)
            {
                return BadRequest();
            }
            var Details = unitOfWork.PurchaseOrderDetailRepository.Get(filter: m => m.PurchaseID == id);

            unitOfWork.PurchaseOrderDetailRepository.RemovRange(Details);
            var Entry = unitOfWork.EntryRepository.Get(filter: x => x.PurchaseOrderID == id).SingleOrDefault();
            var EntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
            if (Entry.TransferedToAccounts == true)
            {
                accountingHelper.CancelTransferToAccounts(EntryDetails.ToList());
            }
            unitOfWork.EntryDetailRepository.RemovRange(EntryDetails); 
            
            unitOfWork.EntryRepository.Delete(Entry.EntryID);

            unitOfWork.PurchaseOrderRepository.Delete(id);
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