using System;
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
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

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
            entryModel.PurchaseOrderID = Entry.PurchaseOrderID;
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
                ParentAccountID = a.Account.AccoutnParentID,
                ParentAccCode = unitOfWork.AccountRepository.GetEntity(s => s.AccountID == a.Account.AccoutnParentID).Code,
                ParentAccNameAR = unitOfWork.AccountRepository.GetEntity(s => s.AccountID == a.Account.AccoutnParentID).NameAR,
                ParentAccNameEN = unitOfWork.AccountRepository.GetEntity(s => s.AccountID == a.Account.AccoutnParentID).NameEN,


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


        [HttpGet]
        [Route("~/api/PurchaseOrder/FirstOpen")]
        public IActionResult FirstOpen()
        {
            PurchaseOrderModel model = new PurchaseOrderModel();
            var count = unitOfWork.PurchaseOrderRepository.Count();
            if (count > 0)
            {
                model.LastCode = unitOfWork.PurchaseOrderRepository.Last().Code;
                model.Count = count;
            }
            else
                model.Count = 0;
            model.SettingModel = GetSetting(2);


            return Ok(model);
        }




        [HttpPost] // يولد قيد يدوي مع ترحيل تلقائي
        [Route("~/api/PurchaseOrder/GenerateconstraintManual")]
        public IActionResult GenerateconstraintManual([FromBody]PurchaseOrderModel purchaseOrderModel)
        { 
            if (purchaseOrderModel.SettingModel.GenerateEntry==true)
            {
                var lastEntry = unitOfWork.EntryRepository.Last();
                int portofolioaccount = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == purchaseOrderModel.PortfolioID && m.Type == true).Select(m => m.PortfolioAccountID).SingleOrDefault();


                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,null, purchaseOrderModel, null, null, lastEntry);
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
        public IActionResult Manualmigration([FromBody]EntryModel entryModel)
        {
            //var Entry = unitOfWork.EntryRepository.GetByID(EntryMODEL.EntryID);
            //Entry.TransferedToAccounts = true;
            //unitOfWork.EntryRepository.Update(Entry);
            var Details = entryModel.EntryDetailModel;
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

            var Entry = unitOfWork.EntryRepository.Get(filter: x => x.EntryID == entryModel.EntryID).SingleOrDefault();
            Entry.TransferedToAccounts = true;
            unitOfWork.EntryRepository.Update(Entry);



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
                return Ok(0);

            }

            #region portfolio data
            var portfolio = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == purchase.PortfolioID && x.Type == true);
            if (portfolio != null)
            {

                model.PortfolioAccount = portfolio.AccountID;

                // portfolio data
                model.PortfolioCode = portfolio.Portfolio.Code;
                model.PortfolioNameAR = portfolio.Portfolio.NameAR;
                model.PortfolioNameEN = portfolio.Portfolio.NameEN;
                model.PortfolioID = portfolio.Portfolio.PortfolioID;
            }

            #endregion

            // employee data
            #region employee part
            var employee = unitOfWork.EmployeeRepository.GetEntity(x => x.EmployeeID == purchase.EmployeeID);
            if (employee != null)
            {
                model.EmpCode = employee.Code;
                model.EmpNameAR = employee.NameAR;
                model.EmpNameEN = employee.NameEN;
                model.EmployeeID = employee.EmployeeID;

            }
            #endregion

            // date part
            #region Date part
            if (purchase.Date != null)
            {

                model.PurchaseDate = purchase.Date.Value.ToString("dd/MM/yyyy");
                model.PurchaseDateHijri = DateHelper.GetHijriDate(purchase.Date);
            }

            #endregion

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
                                PartnerID = m.PartnerID,
                                PartnerCode = m.Partner.Code,
                                PartnerNameAR = m.Partner.NameAR,
                                PartnerNameEN = m.Partner.NameEN
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
        public IActionResult GetPurchaseOrderByID(int id)
        {
            if (id > 0)
            {


                var purchase = unitOfWork.PurchaseOrderRepository.GetByID(id);

                var model = _mapper.Map<PurchaseOrderModel>(purchase);
                model.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == purchase.PortfolioID && x.Type == true).AccountID;
                if (model == null)
                {
                    return Ok(model);

                }


                #region portfolio data
                var portfolio = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == purchase.PortfolioID && x.Type == true);
                if (portfolio != null)
                {

                    model.PortfolioAccount = portfolio.AccountID;

                    // portfolio data
                    model.PortfolioCode = portfolio.Portfolio.Code;
                    model.PortfolioNameAR = portfolio.Portfolio.NameAR;
                    model.PortfolioNameEN = portfolio.Portfolio.NameEN;
                    model.PortfolioID = portfolio.Portfolio.PortfolioID;
                }

                #endregion

                // employee data
                #region employee part
                var employee = unitOfWork.EmployeeRepository.GetEntity(x => x.EmployeeID == purchase.EmployeeID);
                if (employee != null)
                {
                    model.EmpCode = employee.Code;
                    model.EmpNameAR = employee.NameAR;
                    model.EmpNameEN = employee.NameEN;
                    model.EmployeeID = employee.EmployeeID;

                }
                #endregion

                // date part
                #region Date part
                if (purchase.Date != null)
                {

                    model.PurchaseDate = purchase.Date.Value.ToString("dd/MM/yyyy");
                    model.PurchaseDateHijri = DateHelper.GetHijriDate(purchase.Date);
                }

                #endregion


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
                                    PartnerID = m.PartnerID,
                                    PartnerCode = m.Partner.Code,
                                    PartnerNameAR = m.Partner.NameAR,
                                    PartnerNameEN = m.Partner.NameEN
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
            else
                return Ok(1);

        }


        [HttpGet]
        [Route("~/api/PurchaseOrder/Paging/{pageNumber}")]
        public IActionResult PaginationPurchaseOrder(int pageNumber)
        {
            if (pageNumber > 0)
            {


                var purchase = unitOfWork.PurchaseOrderRepository.Get(page: pageNumber).FirstOrDefault();
                var model = _mapper.Map<PurchaseOrderModel>(purchase);
                model.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == purchase.PortfolioID && x.Type == true).AccountID;


                #region portfolio data
                var portfolio = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == purchase.PortfolioID && x.Type == true);
                if (portfolio != null)
                {

                    model.PortfolioAccount = portfolio.AccountID;

                    // portfolio data
                    model.PortfolioCode = portfolio.Portfolio.Code;
                    model.PortfolioNameAR = portfolio.Portfolio.NameAR;
                    model.PortfolioNameEN = portfolio.Portfolio.NameEN;
                    model.PortfolioID = portfolio.Portfolio.PortfolioID;
                }

                #endregion

                // employee data
                #region employee part
                var employee = unitOfWork.EmployeeRepository.GetEntity(x => x.EmployeeID == purchase.EmployeeID);
                if (employee != null)
                {
                    model.EmpCode = employee.Code;
                    model.EmpNameAR = employee.NameAR;
                    model.EmpNameEN = employee.NameEN;
                    model.EmployeeID = employee.EmployeeID;

                }
                #endregion

                // date part
                #region Date part
                if (purchase.Date != null)
                {

                    model.PurchaseDate = purchase.Date.Value.ToString("dd/MM/yyyy");
                    model.PurchaseDateHijri = DateHelper.GetHijriDate(purchase.Date);
                }

                #endregion


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
                                    PartnerID = m.PartnerID,
                                    PartnerCode = m.Partner.Code,
                                    PartnerNameAR = m.Partner.NameAR,
                                    PartnerNameEN = m.Partner.NameEN
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
            else
                return Ok(1);

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

                    return Ok(2);
                }
                else
                {

                    var purchaseOrder = _mapper.Map<PurchaseOrder>(purchaseOrderModel);
                    int portofolioaccount = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == purchaseOrderModel.PortfolioID && m.Type == true).Select(m => m.PortfolioAccountID).SingleOrDefault();


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
                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,null, purchaseOrderModel, null, null, lastEntry);
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
                                EntryID = x.EntryID,
                                StocksCredit = x.StocksCredit,
                                StocksDebit = x.StocksDebit,

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
                    #region generate entry commented
                    //else if (purchaseOrderModel.SettingModel.GenerateEntry == true)

                    //{
                    //    var lastEntry = unitOfWork.EntryRepository.Last();
                    //    var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,null, purchaseOrderModel, null, null, lastEntry);
                    //    EntryMODEL.PurchaseOrderID = purchaseOrder.PurchaseOrderID;
                    //    var Entry = _mapper.Map<Entry>(EntryMODEL);


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
                    #endregion


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



                    return Ok(purchaseOrderModel);



                }
            }
            else
            {
                return Ok(3);
            }
        }





        [HttpPut]
        [Route("~/api/PurchaseOrder/PutPurchaseOrder/{id}")]
        public IActionResult PutPurchaseOrder(int id, [FromBody]  PurchaseOrderModel purchaseOrderModel )
        {
            if (purchaseOrderModel != null)
            {
                if (id != purchaseOrderModel.PurchaseOrderID)
                {

                    return Ok(1);
                }
            }

            if (ModelState.IsValid)
            {

                var Check = unitOfWork.PurchaseOrderRepository.Get(NoTrack: "NoTrack");
                int portofolioaccount = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == purchaseOrderModel.PortfolioID && m.Type == true).Select(m => m.PortfolioAccountID).SingleOrDefault();

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

                            return Ok(purchaseOrderModel);
                        }
                        //===================================توليد قيد مع ترحيل تلقائي===================================
                        if (purchaseOrderModel.SettingModel.AutoGenerateEntry == true)
                        {
                            var EntryDitails = EntriesHelper.UpdateCalculateEntries(portofolioaccount,Entry.EntryID, null, purchaseOrderModel, null, null);

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
                                    EntryID = x.EntryID,
                                    StocksCredit = x.StocksCredit,
                                    StocksDebit = x.StocksDebit

                                }).ToList());
                            }
                            
                        }
                        //===================================توليد قيد مع  عدم ترحيل=================================== 
                        if (purchaseOrderModel.SettingModel.GenerateEntry==true)
                        
                        {
                            var EntryDitails = EntriesHelper.UpdateCalculateEntries(portofolioaccount,Entry.EntryID, null, purchaseOrderModel, null, null);
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

                                return Ok(purchaseOrderModel);
                            }
                            //===================================توليد قيد مع ترحيل تلقائي===================================
                            if (purchaseOrderModel.SettingModel.AutoGenerateEntry == true)
                            {
                                var EntryDitails = EntriesHelper.UpdateCalculateEntries(portofolioaccount,Entry.EntryID, null, purchaseOrderModel, null, null);

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
                                        EntryID = x.EntryID,
                                        StocksDebit = x.StocksDebit,
                                        StocksCredit = x.StocksCredit


                                    }).ToList());
                                }
                                
                            }
                            if (purchaseOrderModel.SettingModel.GenerateEntry == true)

                            {
                                var EntryDitails = EntriesHelper.UpdateCalculateEntries(portofolioaccount,Entry.EntryID, null, purchaseOrderModel, null, null);
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

                            return Ok(purchaseOrderModel);
                        }
                        //===============================================================توليد قيد مع ترحيل تلقائي============================



                        else if (purchaseOrderModel.SettingModel.AutoGenerateEntry == true)
                        {
                            var lastEntry = unitOfWork.EntryRepository.Last();
                            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,null, purchaseOrderModel, null, null, lastEntry);
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
                                    EntryID = x.EntryID,
                                    StocksCredit = x.StocksCredit,
                                    StocksDebit = x.StocksDebit


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

                                return Ok(purchaseOrderModel);
                            }
                            //===============================================================توليد قيد مع ترحيل تلقائي============================



                            else if (purchaseOrderModel.SettingModel.AutoGenerateEntry == true)
                            {
                                var lastEntry = unitOfWork.EntryRepository.Last();
                                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,null, purchaseOrderModel, null, null, lastEntry);
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
                                        EntryID = x.EntryID,
                                        StocksDebit = x.StocksDebit,
                                        StocksCredit = x.StocksCredit


                                    }).ToList());
                                }
                            }
                            //================================توليد قيد مع عدم الترحيل====================================== 
                            if (purchaseOrderModel.SettingModel.GenerateEntry==true)
                            { 
                                var lastEntry = unitOfWork.EntryRepository.Last();
                                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,null, purchaseOrderModel, null, null, lastEntry);
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



                            return Ok(purchaseOrderModel);
                        }


                    }
                    return Ok(purchaseOrderModel);
                }    
            }
            else
            {
                return Ok(3);
            }




        }







        [HttpDelete]
        [Route("~/api/PurchaseOrder/DeletePurchase/{id}")]
        public IActionResult DeletePurchase(int? id)
        {

            if (id == null )
            {

                return Ok(1);
            }
            var modelPurchase = unitOfWork.PurchaseOrderRepository.GetByID(id);
            if (modelPurchase == null)
            {
                return Ok(0);
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


        }


    }
}