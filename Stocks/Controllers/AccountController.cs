using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
    public class AccountController : ControllerBase
    {
        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public AccountController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
        }
        #endregion

        #region GET Methods
        [Route("~/api/Account/GetLast")]
        public AccountModel GetLastAccount()
        {
            var account = unitOfWork.AccountRepository.Last();

            var model = _mapper.Map<AccountModel>(account);
            if (model == null)
            {
                return model;
            }
            var ModelParent = unitOfWork.AccountRepository.GetByID(account.AccoutnParentID);
            if (ModelParent != null)
            {
                model.AccoutnParentID = ModelParent.AccountID;
                model.NameArParent = ModelParent.NameAR != null ? ModelParent.NameAR : "";
                model.NameEnParent = ModelParent.NameEN != null ? ModelParent.NameEN : "";

            }


            model.Count = unitOfWork.AccountRepository.Count();
            if (model.Count == 0)
            {
                return model;

            }

            return model;



        }

        [HttpGet]
        [Route("~/api/Account/Paging/{pageNumber}")]
        public AccountModel Pagination(int pageNumber)
        {
            if (pageNumber > 0)
            {
                var account = unitOfWork.AccountRepository.Get(page: pageNumber).FirstOrDefault();
                var model = _mapper.Map<AccountModel>(account);
                if (model == null)
                {
                    return model;
                }

                var ModelParent = unitOfWork.AccountRepository.GetByID(account.AccoutnParentID);
                if (ModelParent != null)
                {
                    model.AccoutnParentID = ModelParent.AccountID;
                    model.NameArParent = ModelParent.NameAR != null ? ModelParent.NameAR : "";
                    model.NameEnParent = ModelParent.NameEN != null ? ModelParent.NameEN : "";

                }


                model.Count = unitOfWork.AccountRepository.Count();
                if (model.Count == 0)
                {
                    return model;

                }
                return model;

            }
            else
                return null;
           
        }

        [HttpGet]
        [Route("~/api/Account/Get/{id}")]

        public AccountModel GetAccountById(int id)
        {
            var account = unitOfWork.AccountRepository.GetByID(id);

            var model = _mapper.Map<AccountModel>(account);
            if (model == null)
            {
                return model;
            }
            else
            {
                var ModelParent = unitOfWork.AccountRepository.GetByID(account.AccoutnParentID);
                if (ModelParent!= null)
                {
                    model.AccoutnParentID = ModelParent.AccountID;
                    model.NameArParent = ModelParent.NameAR;
                    model.NameEnParent = ModelParent.NameEN;
                }


                model.Count = unitOfWork.AccountRepository.Count();
                if (model.Count == 0)
                {
                    return model;

                }

                return model;
            }

        }



        [Route("~/api/Account/GetAll")]
        public IEnumerable<AccountModel> GetAllAccount()
        {
            var account = unitOfWork.AccountRepository.Get();
            var model = _mapper.Map<IEnumerable<AccountModel>>(account);

            if (model == null)
            {
                return model;
            }

            return (model);
        }
        #endregion

        #region Insert Method

        [HttpPost]
        [Route("~/api/Account/AddAccont/")]
        public IActionResult PostAccount([FromBody] AccountModel accountModel)
        {
            if (ModelState.IsValid)
            {
                var model = _mapper.Map<Account>(accountModel);
                if (model == null)
                {
                    return Ok(model);
                }
                var Check = unitOfWork.AccountRepository.Get();
                if (Check.Any(m => m.Code == accountModel.Code))
                {

                    return Ok("الرمز موجود مسبقا");
                }
                else
                {
                    if (Check.Any(m => m.NameAR == accountModel.NameAR))
                    {

                        return Ok("الاسم موجود مسبقا");
                    }
                    else
                    {
                        unitOfWork.AccountRepository.Insert(model);
                        var Result = unitOfWork.Save();
                        if (Result == true)
                            return Ok(accountModel);
                        else
                            return NotFound();
                    }
                }




            }
            else
            {
                return BadRequest();
            }
        }
        #endregion

        #region Update Method
        [HttpPut]
        [Route("~/api/Account/EditAccont/{id}")]

        public IActionResult PutAccount(int id, [FromBody] AccountModel accountModel)
        {
          

            if (ModelState.IsValid)
            {
                var checkAccount = unitOfWork.AccountRepository.Get(NoTrack: "NoTrack", filter: a => a.AccountID == id);
                if (checkAccount != null)
                {
                    var model = _mapper.Map<Account>(accountModel);
                    if (model == null)
                    {
                        return Ok(model);

                    }

                    var Check = unitOfWork.AccountRepository.Get(NoTrack: "NoTrack");
                    if (!Check.Any(m => m.Code == accountModel.Code))
                    {

                        unitOfWork.AccountRepository.Update(model);
                        var Result = unitOfWork.Save();
                        if (Result == true)
                            return Ok(accountModel);
                        else
                            return Ok("حدث خطأ غير متوقع");

                    }
                    else
                    {
                        if (Check.Any(m => m.Code == accountModel.Code && m.AccountID == id))
                        {

                            unitOfWork.AccountRepository.Update(model);
                            var Result = unitOfWork.Save();
                            if (Result == true)
                                return Ok(accountModel);
                            else
                                return NotFound();
                        }
                        else
                        {
                            return Ok("الرمز او الاسم  موجود مسبقا");
                        }
                    }
                }
                else
                    return BadRequest("Invalid Account !");
               

            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        #endregion

        #region Delete Method
        [HttpDelete]
        [Route("~/api/Account/DeleteAccont/{id}")]

        public IActionResult DeleteAccount(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            else
            {
                var account = unitOfWork.AccountRepository.GetByID(id);
                if (account == null)
                {
                    return BadRequest();
                }
                else
                {

                    unitOfWork.AccountRepository.Delete(id);
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
                                return Ok("Item related with another data .");

                            }
                        }
                    }
                    return Ok("Item deleted successfully .");

                }
            }
        }
        #endregion

    }
}