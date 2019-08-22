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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Stocks.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
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
        [HttpGet]
        [Route("~/api/Account/FirstOpen")]
        public IActionResult FirstOpen()
        {
            AccountModel model = new AccountModel();
            var count = unitOfWork.AccountRepository.Count();
            if (count > 0)
            {
                model.LastCode = unitOfWork.AccountRepository.Last().Code;
                model.Count = count;
            }

            return Ok(model);
        }


        [Route("~/api/Account/GetLast")]
        public IActionResult GetLastAccount()
        {
            var account = unitOfWork.AccountRepository.Last();

            var model = _mapper.Map<AccountModel>(account);
            if (model == null)
            {
                return Ok(0);
            }
            var ModelParent = unitOfWork.AccountRepository.GetByID(account.AccoutnParentID);
            if (ModelParent != null)
            {
                model.AccoutnParentID = ModelParent.AccountID;
                model.NameArParent = ModelParent.NameAR != null ? ModelParent.NameAR : "";
                model.NameEnParent = ModelParent.NameEN != null ? ModelParent.NameEN : "";

            }


            model.Count = unitOfWork.AccountRepository.Count();

            return Ok(model);



        }

        [HttpGet]
        [Route("~/api/Account/Paging/{pageNumber}")]
        public IActionResult Pagination(int pageNumber)
        {
            if (pageNumber > 0)
            {
                var account = unitOfWork.AccountRepository.Get(page: pageNumber).FirstOrDefault();
                var model = _mapper.Map<AccountModel>(account);
                if (model == null)
                {
                    return Ok(0);
                }

                var ModelParent = unitOfWork.AccountRepository.GetByID(account.AccoutnParentID);
                if (ModelParent != null)
                {
                    model.AccoutnParentID = ModelParent.AccountID;
                    model.NameArParent = ModelParent.NameAR != null ? ModelParent.NameAR : "";
                    model.NameEnParent = ModelParent.NameEN != null ? ModelParent.NameEN : "";

                }


                model.Count = unitOfWork.AccountRepository.Count();

                return Ok(model);

            }
            else
                return Ok(1);

        }

        [HttpGet]
        [Route("~/api/Account/Get/{id}")]

        public IActionResult GetAccountById(int id)
        {
            if (id > 0)
            {
                var account = unitOfWork.AccountRepository.GetByID(id);

                var model = _mapper.Map<AccountModel>(account);
                if (model == null)
                {
                    return Ok(0);
                }
                else
                {
                    var ModelParent = unitOfWork.AccountRepository.GetByID(account.AccoutnParentID);
                    if (ModelParent != null)
                    {
                        model.AccoutnParentID = ModelParent.AccountID;
                        model.NameArParent = ModelParent.NameAR;
                        model.NameEnParent = ModelParent.NameEN;
                    }


                    model.Count = unitOfWork.AccountRepository.Count();


                    return Ok(model);
                }
            }
            else
                return Ok(1);

        }

        [Route("~/api/Account/GetAllMain")]
        public IActionResult GetAllMainAccount()
        {
            var account = unitOfWork.AccountRepository.Get(filter: a => a.AccountType == true);
            var model = _mapper.Map<IEnumerable<AccountModel>>(account);

            if (model == null)
            {
                return Ok(0);
            }

            return Ok(model);
        }

        [Route("~/api/Account/GetAllAccounts")]
        public IActionResult GetAllAccount()
        {
            var account = unitOfWork.AccountRepository.Get();
            var model = _mapper.Map<IEnumerable<AccountModel>>(account);

            if (model == null)
            {
                return Ok(0);
            }

            return Ok(model);
        }


        [Route("~/api/Account/GetAll")]
        public IActionResult GetAllSubAccount()
        {
            var account = unitOfWork.AccountRepository.Get(filter: a => a.AccountType == false);
            var model = _mapper.Map<IEnumerable<AccountModel>>(account);

            if (model == null)
            {
                return Ok(0);
            }

            return Ok(model);
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
                    return Ok(0);
                }
                var Check = unitOfWork.AccountRepository.Get();
                if (Check.Any(m => m.Code == accountModel.Code))
                {

                    return Ok(2);
                }
                else
                {
                    if (Check.Any(m => m.NameAR == accountModel.NameAR))
                    {

                        return Ok(2);
                    }
                    else
                    {
                        unitOfWork.AccountRepository.Insert(model);

                        var Result = unitOfWork.Save();
                        if (Result == 200)
                        {
                            accountModel.Count = unitOfWork.AccountRepository.Count();

                            return Ok(4);
                        }
                        else if (Result == 501)
                        {
                            return Ok(5);
                        }
                        else
                        {
                            return Ok(6);
                        }

                    }
                }




            }
            else
            {
                return Ok(3);
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
                        return Ok(0);

                    }

                    var Check = unitOfWork.AccountRepository.Get(NoTrack: "NoTrack");
                    if (!Check.Any(m => m.Code == accountModel.Code))
                    {

                        unitOfWork.AccountRepository.Update(model);

                        var Result = unitOfWork.Save();
                        if (Result == 200)
                        {
                            accountModel.Count = unitOfWork.AccountRepository.Count();

                            return Ok(4);
                        }
                        else if (Result == 501)
                        {
                            return Ok(5);

                        }
                        else
                        {
                            return Ok(6);
                        }

                    }
                    else
                    {
                        if (Check.Any(m => m.Code == accountModel.Code && m.AccountID == id))
                        {

                            unitOfWork.AccountRepository.Update(model);

                            var Result = unitOfWork.Save();
                            if (Result == 200)
                            {
                                accountModel.Count = unitOfWork.AccountRepository.Count();

                                return Ok(4);
                            }
                            else if (Result == 501)
                            {
                                return Ok(5);
                            }
                            else
                            {
                                return Ok(6);
                            }

                        }
                        else
                        {
                            return Ok(2);
                        }
                    }
                }
                else
                    return Ok(0);


            }
            else
            {
                return Ok(3);
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
                return Ok(1);
            }

            else
            {
                var account = unitOfWork.AccountRepository.GetByID(id);
                if (account == null)
                {
                    return Ok(0);
                }
                else
                {

                    unitOfWork.AccountRepository.Delete(id);
                    var Result = unitOfWork.Save();
                    if (Result == 200)
                    {
                        return Ok(4);
                    }
                    else if (Result == 501)
                    {
                        return Ok(5);
                    }
                    else
                    {
                        return Ok(6);
                    }


                }


                #endregion

            }
        }
    }
}