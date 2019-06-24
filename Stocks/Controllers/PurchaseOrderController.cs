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
namespace Stocks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        public PurchaseOrderController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
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
        public IEnumerable<SettingModel> GetSetting()
        {

            var setsetting = unitOfWork.SettingRepository.Get(filter: x => x.VoucherType == 2).Select(a => new SettingModel
            {

                SettingID = a.SettingID,
                VoucherType = 2,
                AutoGenerateEntry = a.AutoGenerateEntry,
                Code = a.Code,
                DoNotGenerateEntry = a.DoNotGenerateEntry,
                GenerateEntry = a.GenerateEntry,
                SettingAccs = SettingAccounts(a.SettingID),

            });
            return setsetting;


        }

        [HttpGet]//يولد قيد بشرط
        [Route("~/api/PurchaseOrder/Generateconstraint")]
        public IActionResult GenerateconstraintWithCon(EntryModel entryModel)
        {
            if (entryModel.PurchaseOrderID == null)
            {
                return Ok("يلا ياعم من هنا");
            }
            else
            {
                var purchase = unitOfWork.PurchaseOrderRepository.GetByID(entryModel.PurchaseOrderID);
                if (purchase == null)
                {
                    return Ok("يلا ياعم من هنا");
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var Check = unitOfWork.EntryRepository.Get();
                        if (Check.Any(m => m.Code == entryModel.Code))
                        {

                            return Ok("يلا ياعم من هنا");
                        }
                        else
                        {

                            var Entry = _mapper.Map<Entry>(entryModel);


                            var Details = entryModel.EntryDetailModel;
                            if (Details == null)
                            {
                                unitOfWork.EntryRepository.Insert(Entry);
                                unitOfWork.Save();
                                return Ok(entryModel);

                            }
                            else
                            {
                                unitOfWork.EntryRepository.Insert(Entry);

                                foreach (var item in Details)
                                {
                                    item.EntryID = Entry.EntryID;
                                    var details = _mapper.Map<EntryDetail>(item);
                                    unitOfWork.EntryDetailRepository.Insert(details);

                                }
                            }
                            unitOfWork.Save();
                            return Ok();
                        }
                    }
                    else
                    {
                        return BadRequest();
                    }
                }

            }
        }



        [HttpGet]
        [Route("~/api/PurchaseOrder/GetLastPurchaseOrder")]
        public IActionResult GetLastPurchaseOrder()
        {
            var purchase = unitOfWork.PurchaseOrderRepository.Last();


            var model = _mapper.Map<PurchaseOrderModel>(purchase);
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

            model.Settings = GetSetting();

            var Entry = unitOfWork.EntryRepository.Get(filter: x => x.PurchaseOrderID == purchase.PurchaseOrderID).SingleOrDefault();
            if (Entry != null)
            {
                var EntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID).Select(m => new EntryDetailModel
                {
                    AccountID = m.AccountID,
                    Credit = m.Credit,
                    Debit = m.Debit,
                    EntryID = m.EntryID,
                    EntryDetailID = m.EntryDetailID,


                });
                if (EntryDetails != null)
                {

                    model.entryModel = _mapper.Map<EntryModel>(Entry);
                    model.entryModel.EntryDetailModel = EntryDetails;
                }
            }








            return Ok(model);

        }



        [HttpGet]
        [Route("~/api/PurchaseOrder/GetLastPurchaseOrder/{id}")]
        public IActionResult GetSPurchaseOrderByID(int id)
        {
            var purchase = unitOfWork.PurchaseOrderRepository.GetByID(id);

            var model = _mapper.Map<PurchaseOrderModel>(purchase);
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

            model.Settings = GetSetting();

            var Entry = unitOfWork.EntryRepository.Get(filter: x => x.PurchaseOrderID == purchase.PurchaseOrderID).SingleOrDefault();
            if (Entry != null)
            {
                var EntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID).Select(m => new EntryDetailModel
                {
                    AccountID = m.AccountID,
                    Credit = m.Credit,
                    Debit = m.Debit,
                    EntryID = m.EntryID,
                    EntryDetailID = m.EntryDetailID,


                });
                if (EntryDetails != null)
                {

                    model.entryModel = _mapper.Map<EntryModel>(Entry);
                    model.entryModel.EntryDetailModel = EntryDetails;
                }
            }








            return Ok(model);

        }


        [HttpGet]
        [Route("~/api/Pagination/PurchaseOrder/{pageNumber}")]
        public IActionResult PaginationPurchaseOrder(int pageNumber)
        {
            var purchase = unitOfWork.PurchaseOrderRepository.Get(page: pageNumber).FirstOrDefault();
            var model = _mapper.Map<PurchaseOrderModel>(purchase);
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

            model.Settings = GetSetting();

            var Entry = unitOfWork.EntryRepository.Get(filter: x => x.PurchaseOrderID == purchase.PurchaseOrderID).SingleOrDefault();
            if (Entry != null)
            {
                var EntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID).Select(m => new EntryDetailModel
                {
                    AccountID = m.AccountID,
                    Credit = m.Credit,
                    Debit = m.Debit,
                    EntryID = m.EntryID,
                    EntryDetailID = m.EntryDetailID,


                });
                if (EntryDetails != null)
                {

                    model.entryModel = _mapper.Map<EntryModel>(Entry);
                    model.entryModel.EntryDetailModel = EntryDetails;
                }
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

                    var modelPurchase = _mapper.Map<PurchaseOrder>(purchaseOrderModel);


                    var Details = purchaseOrderModel.DetailsModels;
                    if (Details == null)
                    {
                        unitOfWork.PurchaseOrderRepository.Insert(modelPurchase);
                        unitOfWork.Save();
                        return Ok(purchaseOrderModel);

                    }
                    else
                    {
                        unitOfWork.PurchaseOrderRepository.Insert(modelPurchase);

                        foreach (var item in Details)
                        {
                            PurchaseOrderDetailModel purchaseOrderDetailModel = new PurchaseOrderDetailModel();
                            purchaseOrderDetailModel.BankCommission = item.BankCommission;
                            purchaseOrderDetailModel.BankCommissionRate = item.BankCommissionRate;
                            purchaseOrderDetailModel.NetAmmount = item.NetAmmount;
                            purchaseOrderDetailModel.PurchaseID = modelPurchase.PurchaseOrderID;
                          
                            purchaseOrderDetailModel.PurchasePrice = item.PurchasePrice;
                            purchaseOrderDetailModel.PurchaseValue = item.PurchaseValue;
                            purchaseOrderDetailModel.StockCount = item.StockCount;
                            purchaseOrderDetailModel.TaxOnCommission = item.TaxOnCommission;
                            purchaseOrderDetailModel.TaxRateOnCommission = item.TaxRateOnCommission;
                            
                            var details = _mapper.Map<PurchaseOrderDetail>(purchaseOrderDetailModel);
                            unitOfWork.PurchaseOrderDetailRepository.Insert(details);

                        }
                        if (purchaseOrderModel.entryModel != null)
                        {
                            var CheckEntry = unitOfWork.EntryRepository.Get();
                            if (CheckEntry.Any(m => m.Code == purchaseOrderModel.entryModel.Code))
                            {

                                return Ok("كود القيد مكرر");
                            }
                            else
                            {
                                EntryModel entryModel = new EntryModel();
                                entryModel.PurchaseOrderID = purchaseOrderModel.PurchaseOrderID;
                                entryModel.ReceiptID = purchaseOrderModel.entryModel.ReceiptID;
                                entryModel.PurchaseOrderID = modelPurchase.PurchaseOrderID;
                                entryModel.NoticeID = purchaseOrderModel.entryModel.NoticeID;
                                entryModel.Code = purchaseOrderModel.entryModel.Code;
                                entryModel.Count = purchaseOrderModel.entryModel.Count;
                                entryModel.Date = purchaseOrderModel.entryModel.Date;

                                var enry = _mapper.Map<Entry>(entryModel);
                                var entryDetails = purchaseOrderModel.entryModel.EntryDetailModel;
                                if (entryDetails == null)
                                {
                                    unitOfWork.EntryRepository.Insert(enry);
                                    unitOfWork.Save();
                                    return Ok(purchaseOrderModel);
                                }
                                else
                                {
                                    unitOfWork.EntryRepository.Insert(enry);
                                    foreach (var item2 in entryDetails)
                                    {
                                        EntryDetailModel entryDetailsModel = new EntryDetailModel();
                                        entryDetailsModel.AccountID = item2.AccountID;
                                        entryDetailsModel.Credit = item2.Credit;
                                        entryDetailsModel.Debit = item2.Debit;
                                        entryDetailsModel.EntryID = enry.EntryID;

                                        var details = _mapper.Map<EntryDetail>(entryDetailsModel);
                                        unitOfWork.EntryDetailRepository.Insert(details);

                                    }
                                    unitOfWork.Save();
                                    return Ok(purchaseOrderModel);
                                }
                            }

                        }


                        unitOfWork.Save();



                        return Ok(purchaseOrderModel);
                    }


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

                var purchase = _mapper.Map<PurchaseOrder>(purchaseOrderModel);
                var NewdDetails = purchaseOrderModel.DetailsModels;
                var Newdetails = _mapper.Map<IEnumerable<PurchaseOrderDetail>>(NewdDetails);
                var OldDetails = unitOfWork.PurchaseOrderDetailRepository.Get(filter: m => m.PurchaseID == purchase.PurchaseOrderID);



                //======================================================================================

                if (Check.Any(m => m.Code != purchase.Code))
                {
                    unitOfWork.PurchaseOrderRepository.Update(purchase);
                    if (OldDetails != null)
                    {
                        unitOfWork.PurchaseOrderDetailRepository.RemovRange(OldDetails);
                        unitOfWork.Save();
                    }


                    if (Newdetails != null)
                    {
                        foreach (var item in Newdetails)
                        {
                            item.PurchaseID = purchase.PurchaseOrderID;
                            item.PurchaseOrderDetailID = 0;
                            var details = _mapper.Map<PurchaseOrderDetail>(item);

                            unitOfWork.PurchaseOrderDetailRepository.Insert(details);

                        }
                    }
                    //********************************************

                    if (purchaseOrderModel.entryModel != null)
                    {
                        var Check2 = unitOfWork.EntryRepository.Get(NoTrack: "NoTrack");

                        var Entry = _mapper.Map<Entry>(purchaseOrderModel.entryModel);
                        var NewdDetailsEntry = purchaseOrderModel.entryModel.EntryDetailModel;
                        var NewdetailsEntry = _mapper.Map<IEnumerable<EntryDetail>>(NewdDetailsEntry);
                        var OldDetailsEntry = unitOfWork.EntryDetailRepository.Get(filter: m => m.EntryID == Entry.EntryID);

                        if (Check2.Any(m => m.Code != Entry.Code))
                        {
                            unitOfWork.EntryRepository.Update(Entry);
                            if (OldDetailsEntry != null)
                            {
                                unitOfWork.EntryDetailRepository.RemovRange(OldDetailsEntry);
                                unitOfWork.Save();
                            }


                            if (NewdetailsEntry != null)
                            {
                                foreach (var item in NewdetailsEntry)
                                {
                                    item.EntryID = Entry.EntryID;
                                    item.EntryDetailID = 0;
                                    var details = _mapper.Map<EntryDetail>(item);

                                    unitOfWork.EntryDetailRepository.Insert(details);

                                }
                            }



                        }



                        else
                        {
                            if (Check2.Any(m => m.Code == Entry.Code && m.EntryID == Entry.EntryID))
                            {
                                unitOfWork.EntryRepository.Update(Entry);
                                if (OldDetailsEntry != null)
                                {
                                    unitOfWork.EntryDetailRepository.RemovRange(OldDetailsEntry);
                                    unitOfWork.Save();
                                }


                                if (NewdetailsEntry != null)
                                {
                                    foreach (var item in NewdetailsEntry)
                                    {
                                        item.EntryID = Entry.EntryID;
                                        item.EntryDetailID = 0;
                                        var details = _mapper.Map<EntryDetail>(item);

                                        unitOfWork.EntryDetailRepository.Insert(details);

                                    }
                                }


                            }
                        }

                        unitOfWork.Save();
                        return Ok(purchaseOrderModel);
                    }
                }


                //=================================================================================

                else
                {
                    if (Check.Any(m => m.Code == purchase.Code && m.PurchaseOrderID == id))
                    {
                        unitOfWork.PurchaseOrderRepository.Update(purchase);
                        if (OldDetails != null)
                        {
                            unitOfWork.PurchaseOrderDetailRepository.RemovRange(OldDetails);
                            unitOfWork.Save();
                        }


                        if (Newdetails != null)
                        {
                            foreach (var item in Newdetails)
                            {
                                item.PurchaseID = purchase.PurchaseOrderID;
                                item.PurchaseOrderDetailID = 0;
                                var details = _mapper.Map<PurchaseOrderDetail>(item);

                                unitOfWork.PurchaseOrderDetailRepository.Insert(details);

                            }
                        }
                        //********************************************

                        if (purchaseOrderModel.entryModel != null)
                        {
                            var Check2 = unitOfWork.EntryRepository.Get(NoTrack: "NoTrack");

                            var Entry = _mapper.Map<Entry>(purchaseOrderModel.entryModel);
                            var NewdDetailsEntry = purchaseOrderModel.entryModel.EntryDetailModel;
                            var NewdetailsEntry = _mapper.Map<IEnumerable<EntryDetail>>(NewdDetailsEntry);
                            var OldDetailsEntry = unitOfWork.EntryDetailRepository.Get(filter: m => m.EntryID == Entry.EntryID);

                            if (Check2.Any(m => m.Code != Entry.Code))
                            {
                                unitOfWork.EntryRepository.Update(Entry);
                                if (OldDetailsEntry != null)
                                {
                                    unitOfWork.EntryDetailRepository.RemovRange(OldDetailsEntry);
                                    unitOfWork.Save();
                                }


                                if (NewdetailsEntry != null)
                                {
                                    foreach (var item in NewdetailsEntry)
                                    {
                                        item.EntryID = Entry.EntryID;
                                        item.EntryDetailID = 0;
                                        var details = _mapper.Map<EntryDetail>(item);

                                        unitOfWork.EntryDetailRepository.Insert(details);

                                    }
                                }



                            }



                            else
                            {
                                if (Check2.Any(m => m.Code == Entry.Code && m.EntryID == Entry.EntryID))
                                {
                                    unitOfWork.EntryRepository.Update(Entry);
                                    if (OldDetailsEntry != null)
                                    {
                                        unitOfWork.EntryDetailRepository.RemovRange(OldDetailsEntry);
                                        unitOfWork.Save();
                                    }


                                    if (NewdetailsEntry != null)
                                    {
                                        foreach (var item in NewdetailsEntry)
                                        {
                                            item.EntryID = Entry.EntryID;
                                            item.EntryDetailID = 0;
                                            var details = _mapper.Map<EntryDetail>(item);

                                            unitOfWork.EntryDetailRepository.Insert(details);

                                        }
                                    }


                                }
                            }

                            unitOfWork.Save();
                            return Ok(purchaseOrderModel);

                        }

                    }



                }
                return Ok(purchaseOrderModel);
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