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
namespace Stocks.Controllers
{
    public class SellingOrderController : Controller
    {

        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        public SellingOrderController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
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
        public IEnumerable<SettingModel> GetSetting()
        {

            var setsetting = unitOfWork.SettingRepository.Get(filter: x => x.VoucherType == 1).Select(a => new SettingModel
            {

                SettingID = a.SettingID,
                VoucherType = 1,
                AutoGenerateEntry = a.AutoGenerateEntry,
                Code = a.Code,
                DoNotGenerateEntry = a.DoNotGenerateEntry,
                GenerateEntry = a.GenerateEntry,
                SettingAccs = SettingAccounts(a.SettingID),

            });
            return setsetting;


        }



        [HttpGet]//يولد قيد بشرط
        [Route("~/api/SellingOrder/Generateconstraint")]
        public IActionResult GenerateconstraintWithCon(EntryModel entryModel)
        {
            if (entryModel.SellingOrderID == null)
            {
                return Ok("يلا ياعم من هنا");
            }
            else
            {
                var selling = unitOfWork.SellingOrderReposetory.GetByID(entryModel.SellingOrderID);
                if (selling == null)
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
        [Route("~/api/SellingOrder/GetLastSellingOrder")]
        public IActionResult GetLastSellingOrder()
        {
            var selling = unitOfWork.SellingOrderReposetory.Last();


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

          //  model.Settings = GetSetting();

            var Entry = unitOfWork.EntryRepository.Get(filter: x => x.SellingOrderID == selling.SellingOrderID).SingleOrDefault(); 
            if (Entry !=null)
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

           // model.Settings = GetSetting();

            var Entry = unitOfWork.EntryRepository.Get(filter: x => x.SellingOrderID == selling.SellingOrderID).SingleOrDefault();
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

         //   model.Settings = GetSetting();

            var Entry = unitOfWork.EntryRepository.Get(filter: x => x.SellingOrderID == selling.SellingOrderID).SingleOrDefault();
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
                    if (Details == null)
                    {
                        unitOfWork.SellingOrderReposetory.Insert(modelselling);
                        unitOfWork.Save();
                        return Ok(sellingOrderModel);

                    }
                    else
                    {
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
                        if (sellingOrderModel.entryModel != null)
                        {
                            var CheckEntry = unitOfWork.EntryRepository.Get();
                            if (CheckEntry.Any(m => m.Code == sellingOrderModel.entryModel.Code))
                            {

                                return Ok("كود القيد مكرر");
                            }
                            else
                            {
                                EntryModel entryModel = new EntryModel();
                                entryModel.SellingOrderID = modelselling.SellingOrderID;
                                entryModel.ReceiptID = sellingOrderModel.entryModel.ReceiptID;
                                entryModel.PurchaseOrderID = sellingOrderModel.entryModel.PurchaseOrderID;
                                entryModel.NoticeID = sellingOrderModel.entryModel.NoticeID;
                                entryModel.Code = sellingOrderModel.entryModel.Code;
                                entryModel.Count = sellingOrderModel.entryModel.Count;
                                entryModel.Date = sellingOrderModel.entryModel.Date;
                                
                                var enry = _mapper.Map<Entry>(entryModel);
                                var entryDetails = sellingOrderModel.entryModel.EntryDetailModel;
                                if (entryDetails == null)
                                {
                                    unitOfWork.EntryRepository.Insert(enry);
                                    unitOfWork.Save();
                                    return Ok(sellingOrderModel);
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
                                    return Ok(sellingOrderModel);
                                }
                            }

                        }


                        unitOfWork.Save();



                        return Ok(sellingOrderModel);
                    }


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



                //======================================================================================

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
                    //********************************************

                    if (sellingOrderModel.entryModel != null)
                    {
                        var Check2 = unitOfWork.EntryRepository.Get(NoTrack: "NoTrack");

                        var Entry = _mapper.Map<Entry>(sellingOrderModel.entryModel);
                        var NewdDetailsEntry = sellingOrderModel.entryModel.EntryDetailModel;
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
                        return Ok(sellingOrderModel);
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
                        //********************************************

                        if (sellingOrderModel.entryModel != null)
                        {
                            var Check2 = unitOfWork.EntryRepository.Get(NoTrack: "NoTrack");

                            var Entry = _mapper.Map<Entry>(sellingOrderModel.entryModel);
                            var NewdDetailsEntry = sellingOrderModel.entryModel.EntryDetailModel;
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
                            return Ok(sellingOrderModel);

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

