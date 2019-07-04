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
    public class DailyVoucherController : ControllerBase
    {
        #region CTOR & Definitions
        private readonly IMapper _mapper;
        private UnitOfWork unitOfWork;

        public DailyVoucherController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
        }
        #endregion

        #region GET Methods

        public EntryModel GetEntry(Entry entry)
        {
            var model = _mapper.Map<EntryModel>(entry);
            if (model == null)
            {
                return model;
            }
            #region Date part

            model.Date = entry.Date.Value.ToString("dd/MM/yyyy");
            model.DateHijri = DateHelper.GetHijriDate(entry.Date);
            #endregion

            #region Details part
            var details = unitOfWork.EntryDetailRepository

                .Get(filter: m => m.EntryID == entry.EntryID)
                .Select(m => new EntryDetailModel
                {
                    EntryDetailID = m.EntryDetailID,
                    EntryID = m.EntryID,
                    Debit = m.Debit,
                    Credit = m.Credit,
                    AccountID = m.AccountID,
                    AccNameAR = m.Account.NameAR,
                    AccNameEN = m.Account.NameEN,
                    AccCode = m.Account.Code,

                });
            if (details != null)
                model.EntryDetailModel = details;

            #endregion

            model.Count = unitOfWork.EntryRepository.Count();

            return model;
        }

        [HttpGet]
        [Route("~/api/DailyVoucher/FirstOpen")]
        public IActionResult FirstOpen()
        {
            EntryModel model = new EntryModel();
            model.LastCode = unitOfWork.PartnerRepository.Last().Code;
            model.Count = unitOfWork.PartnerRepository.Count();
            return Ok(model);
        }


        [HttpGet]
        [Route("~/api/DailyVoucher/GetLast")]
        public IActionResult GetLast()
        {
            var voucher = unitOfWork.EntryRepository.Last();
            return Ok(GetEntry(voucher));
        }


        [HttpGet]
        [Route("~/api/DailyVoucher/Paging/{pageNumber}")]
        public IActionResult Pagination(int pageNumber)
        {
            if (pageNumber > 0)
            {
                var voucher = unitOfWork.EntryRepository.Get(page: pageNumber).FirstOrDefault();


                return Ok(GetEntry(voucher));
            }
            else
                return Ok(1);
        }


        [HttpGet]
        [Route("~/api/DailyVoucher/Get/{id}")]

        public IActionResult GetById(int id)
        {

            if (id > 0)
            {
                var voucher = unitOfWork.EntryRepository.GetByID(id);


                return Ok(GetEntry(voucher));


            }
            else
                return Ok(1);
        }


        [Route("~/api/DailyVoucher/GetAll")]
        public IActionResult GetAll()
        {
            var vouchers = unitOfWork.EntryRepository.Get().ToList();
            var model = _mapper.Map<IEnumerable<EntryModel>>(vouchers).ToList();

            if (model == null)
            {
                return Ok(0);
            }

            for (int i = 0; i < vouchers.Count(); i++)
            {
                for (int j = i; j < model.Count(); j++)
                {
                    if (model[j].EntryID == vouchers[i].EntryID)
                    {
                        #region Date part

                        model[j].Date = vouchers[i].Date.Value.ToString("dd/MM/yyyy");
                        model[j].DateHijri = DateHelper.GetHijriDate(vouchers[i].Date);
                        #endregion

                        #region Detail part
                        var details = unitOfWork.EntryDetailRepository

                            .Get(filter: m => m.EntryID == vouchers[i].EntryID)
                            .Select(m => new EntryDetailModel
                            {
                                EntryDetailID = m.EntryDetailID,
                                EntryID = m.EntryID,
                                Debit = m.Debit,
                                Credit = m.Credit,
                                AccountID = m.AccountID,
                                AccNameAR = m.Account.NameAR,
                                AccNameEN = m.Account.NameEN,
                                AccCode = m.Account.Code,

                            });
                        if (details != null)
                            model[j].EntryDetailModel = details;

                        #endregion


                    }
                    else
                        continue;


                }

            }


            return Ok(model);
        }

        #endregion

        #region Insert Methods
        [HttpPost]
        [Route("~/api/DailyVoucher/Add")]
        public IActionResult PostEntry([FromBody] EntryModel entry)
        {

            if (ModelState.IsValid)
            {

                var Check = unitOfWork.EntryRepository.Get();
                //if (entry == null)
                //{
                //    return Ok(0);
                //}
                if (Check.Any(m => m.Code == entry.Code))
                {
                    return Ok(2);
                }
                else
                {

                    var model = _mapper.Map<Entry>(entry);


                    var detail = entry.EntryDetailModel;

                    var Details = _mapper.Map<IEnumerable<EntryDetail>>(detail);
                    try
                    {
                        unitOfWork.EntryRepository.Insert(model);

                        if (Details != null)
                        {
                            foreach (var item in detail)
                            {
                                var obj = _mapper.Map<EntryDetail>(item);
                                
                                obj.EntryID = model.EntryID;

                                unitOfWork.EntryDetailRepository.Insert(obj);
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

                        return Ok(model);

                    }
                    catch (Exception ex)
                    {
                        // unitOfWork.Rollback();
                        return Ok("يوجد خطا بادخال البيانات");
                        //Log, handle or absorbe I don't care ^_^
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
        [Route("~/api/DailyVoucher/Update/{id}")]
        public IActionResult Update(int id, [FromBody] EntryModel entry)
        {
            if (id != entry.EntryID)
            {

                return Ok(1);
            }

            if (ModelState.IsValid)
            {
                var model = _mapper.Map<Entry>(entry);

                var detail = entry.EntryDetailModel;
                //var EmpolyeeCard = _mapper.Map<IEnumerable<EmployeeCard>>(empolyeeCard);

                var Check = unitOfWork.EntryRepository.Get(NoTrack: "NoTrack");
                var oldDetail = unitOfWork.EntryDetailRepository

                    .Get(NoTrack: "NoTrack", filter: m => m.EntryID == model.EntryID);

                if (oldDetail != null)
                {

                    unitOfWork.EntryDetailRepository.RemovRange(oldDetail); 
                }


                if (Check.Any(m => m.Code != entry.Code))
                {
                    unitOfWork.EntryRepository.Update(model);


                    foreach (var item in detail)
                    {
                        item.EntryID = model.EntryID;
                        item.EntryDetailID = 0;
                        var newdetail = _mapper.Map<EntryDetail>(item);

                        unitOfWork.EntryDetailRepository.Insert(newdetail);

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
                    return Ok(entry);

                }
                else
                {
                    if (Check.Any(m => m.Code == entry.Code && m.EntryID == id))
                    {

                        unitOfWork.EntryRepository.Update(model);

                        foreach (var item in detail)
                        {
                            item.EntryID = model.EntryID;
                            item.EntryDetailID = 0;
                            var newdetail = _mapper.Map<EntryDetail>(item);

                            unitOfWork.EntryDetailRepository.Insert(newdetail);

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
                        return Ok(entry);
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
        [Route("~/api/DailyVoucher/Delete/{id}")]
        public IActionResult Delete(int? id)
        {

            if (id>0)
            {
                var Entry = unitOfWork.EntryRepository.GetByID(id);
                if (Entry == null)
                {
                    return Ok(0);
                }
                var detail = unitOfWork.EntryDetailRepository.Get(filter: m => m.EntryID == id);

                unitOfWork.EntryDetailRepository.RemovRange(detail);
                unitOfWork.EntryRepository.Delete(Entry);

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
                //    return NotFound("Not found !");
                //} 
            }
            else
                return Ok(1);

        }

        #endregion

    }
}