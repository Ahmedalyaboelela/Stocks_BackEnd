﻿using System;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Stocks.Controllers
{
    //[Authorize(Roles = "SuperAdmin,Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
        }

        #endregion

        #region Upload Image Methods

        [HttpPost, DisableRequestSizeLimit]
        //[Consumes("multipart/form-data")]
        [Route("~/api/Employee/Upload")]
        public async Task<IActionResult> Upload()
        {
            var files = Request.Form.Files;
            List<object> save = new List<object>();
            foreach (var file in files)
            {
                save.Add(UploadHelper.SaveFile(file, "Employee"));
            }

            object[] pathes = save.ToArray();
            return Ok(pathes);

        }

        [HttpPost, DisableRequestSizeLimit]
        //[Consumes("multipart/form-data")]
        [Route("~/api/Employee/UploadImage")]
        public IActionResult UploadImage()
        {
            var files = Request.Form.Files[0];
            return Ok(UploadHelper.SaveFile(Request.Form.Files[0], "Employee"));

        }
        #endregion

        #region GET Methods

        public EmployeeModel GetEmployee(Employee employee)
        {
            var model = _mapper.Map<EmployeeModel>(employee);
            if (model == null)
            {
                return model;
            }
            #region Date part
            if(employee.BirthDate!=null)
            {

                model.BirthDate = employee.BirthDate.Value.ToString("d/M/yyyy");
                model.BirthDateHijri = DateHelper.GetHijriDate(employee.BirthDate);
            }
            #endregion

            #region Cards part
            var empCard = unitOfWork.EmployeeCardRepository

                .Get(filter: m => m.EmployeeID == employee.EmployeeID);
                if(empCard !=null)
                {
                    var EmpCard=empCard.Select(m => new EmployeeCardModel()
                    {
                        EmpCardId = m.EmpCardId,
                        CardType = m.CardType,
                        IssuePlace = m.IssuePlace,
                        Code = m.Code,
                        IssueDate = m.IssueDate!=null? m.IssueDate.Value.ToString("d/M/yyyy"):null,
                        IssueDateHigri = m.IssueDate != null ? DateHelper.GetHijriDate(m.IssueDate): null,
                        EndDate = m.EndDate!=null? m.EndDate.Value.ToString("d/M/yyyy"): null,
                        EndDateHigri = m.EndDate != null ? DateHelper.GetHijriDate(m.EndDate): null,
                        RenewalDate = m.RenewalDate !=null? m.RenewalDate.Value.ToString("d/M/yyyy"): null,
                        RenewalDateHigri = m.RenewalDate != null ? DateHelper.GetHijriDate(m.RenewalDate): null,
                        Notes = m.Notes,
                        EmployeeID = m.EmployeeID,
                        Fees = m.Fees,

                    });
                    model.emplCards = EmpCard;
            }
                
         

            #endregion

            model.Count = unitOfWork.EmployeeRepository.Count();

            return model;
        }

        [HttpGet]
        [Route("~/api/Employee/FirstOpen")]
        public IActionResult FirstOpen()
        {
            EmployeeModel model = new EmployeeModel();
            var count = unitOfWork.EmployeeRepository.Count();
            if(count>0)
            {
                model.LastCode = unitOfWork.EmployeeRepository.Last().Code;
                model.Count = count;
            }
           
            return Ok(model);
        }

        [HttpGet]
        [Route("~/api/Employee/GetLast")]
        public IActionResult GetLastEmp()
        {
            var employee = unitOfWork.EmployeeRepository.Last();
            return Ok(GetEmployee(employee));
        }


        [HttpGet]
        [Route("~/api/Employee/Paging/{pageNumber}")]
        public IActionResult Pagination(int pageNumber)
        {
            if (pageNumber > 0)
            {
                var employee = unitOfWork.EmployeeRepository.Get(page: pageNumber).FirstOrDefault();
                return Ok(GetEmployee(employee));
            }
            else
                return Ok(1);
        }


        [HttpGet]
        [Route("~/api/Employee/Get/{id}")]

        public IActionResult GetEmployeeById(int id)
        {

            if (id > 0)
            {
                var employee = unitOfWork.EmployeeRepository.GetByID(id);


                return Ok(GetEmployee(employee));


            }
            else
                return Ok(1);
        }


        [Route("~/api/Employee/GetAll")]
        public IActionResult GetAllEmployees()
        {
            var employees = unitOfWork.EmployeeRepository.Get(filter: x=> x.IsActive==true).ToList();
            var model = _mapper.Map<IEnumerable<EmployeeModel>>(employees).ToList();

            if (model == null)
            {
                return Ok( 0);
            }

            for (int i = 0; i < employees.Count(); i++)
            {
                for (int j = i; j < model.Count(); j++)
                {
                    if (model[j].EmployeeID == employees[i].EmployeeID)
                    {
                        #region Date part
                        if(employees[i].BirthDate!=null)
                        {

                            model[j].BirthDate = employees[i].BirthDate.Value.ToString("d/M/yyyy");
                            model[j].BirthDateHijri = DateHelper.GetHijriDate(employees[i].BirthDate);
                        }
                        #endregion

                        #region Cards part
                        if(unitOfWork.EmployeeCardRepository.Count()>0)
                        {
                            var EmpCard = unitOfWork.EmployeeCardRepository

                           .Get(filter: m => m.EmployeeID == employees[i].EmployeeID)
                           .Select(m => new EmployeeCardModel
                           {
                               EmpCardId = m.EmpCardId,
                               CardType = m.CardType,
                               IssuePlace = m.IssuePlace,
                               Code = m.Code,
                               IssueDate = m.IssueDate != null ? m.IssueDate.Value.ToString("d/M/yyyy") : null,
                               IssueDateHigri = m.IssueDate != null ? DateHelper.GetHijriDate(m.IssueDate) : null,
                               EndDate = m.EndDate != null ? m.EndDate.Value.ToString("d/M/yyyy") : null,
                               EndDateHigri = m.EndDate != null ? DateHelper.GetHijriDate(m.EndDate) : null,
                               RenewalDate = m.RenewalDate != null ? m.RenewalDate.Value.ToString("d/M/yyyy") : null,
                               RenewalDateHigri = m.RenewalDate != null ? DateHelper.GetHijriDate(m.RenewalDate) : null,
                               Notes = m.Notes,
                               EmployeeID = m.EmployeeID,
                               Fees = m.Fees,

                           });
                            if (EmpCard != null)
                                model[j].emplCards = EmpCard;

                        }

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
        [Route("~/api/Employee/AddEmployee")]
        public IActionResult PostEmp([FromBody] EmployeeModel empModel)
        {

            if (ModelState.IsValid)
            {

                var Check = unitOfWork.EmployeeRepository.Get();
                //if (empModel == null)
                //{
                //    return Ok(0);
                //}
                if (Check.Any(m => m.Code == empModel.Code))
                {
                    return Ok(2);
                }
                else
                { 
                    if (empModel.BirthDate ==null)
                    {
                        empModel.BirthDate = DateTime.Now.ToString("d/M/yyyy");
                    }

                    var model = _mapper.Map<Employee>(empModel);
                    
                    var empolyeeCard = empModel.emplCards;
                    
                    unitOfWork.EmployeeRepository.Insert(model);
                        
                    if (empolyeeCard != null)
                    {
                        foreach (var item in empolyeeCard)
                        {
                            var obj = _mapper.Map<EmployeeCard>(item);
                                   
                            obj.EmployeeID = model.EmployeeID;

                            unitOfWork.EmployeeCardRepository.Insert(obj);
                                   

                        }
                    }


                  
               var result=unitOfWork.Save();
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

                

            }
            else
            {
                return Ok(3);
            }
        }
        #endregion


        #region Update Methods
        [HttpPut]
        [Route("~/api/Employee/Update/{id}")]
        public IActionResult Update( int id,[FromBody] EmployeeModel empModel)
        {
            //if (id != empModel.EmployeeID)
            //{

            //    return Ok(1);
            //}

            if (ModelState.IsValid)
            {
                var model = _mapper.Map<Employee>(empModel);

                
                var newCards = empModel.emplCards;

                var Check = unitOfWork.EmployeeRepository.Get(NoTrack: "NoTrack");

                if (!Check.Any(m => m.Code == empModel.Code))
                {
                    unitOfWork.EmployeeRepository.Update(model);

                    

                    // cards
                    var oldcards = unitOfWork.EmployeeCardRepository


                    .Get(filter: m => m.EmployeeID == model.EmployeeID);

                    if (oldcards != null)
                    {

                        unitOfWork.EmployeeCardRepository.RemovRange(oldcards);


                    } 
                    if (newCards != null)
                    {
                        foreach (var item in newCards)
                        {
                            item.EmployeeID = model.EmployeeID;
                            var obj = _mapper.Map<EmployeeCard>(item);

                            unitOfWork.EmployeeCardRepository.Insert(obj);
                        }
                    }

                    var result = unitOfWork.Save();
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
                else
                {
                     
                    if (Check.Any(m => m.Code == empModel.Code && m.EmployeeID== empModel.EmployeeID))
                    {
                        unitOfWork.EmployeeRepository.Update(model);



                        // cards
                        var oldcards = unitOfWork.EmployeeCardRepository


                        .Get(filter: m => m.EmployeeID == model.EmployeeID);

                        if (oldcards != null)
                        {

                            unitOfWork.EmployeeCardRepository.RemovRange(oldcards);

                        }
                        if (newCards != null)
                        {
                            foreach (var item in newCards)
                            {
                                item.EmployeeID = model.EmployeeID;
                                var obj = _mapper.Map<EmployeeCard>(item);

                                unitOfWork.EmployeeCardRepository.Insert(obj);
                            }
                        }
                        var result = unitOfWork.Save();
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

                    }
                return Ok();
                  
                }

            else
            {
                return Ok(3);
            }
        }
        #endregion


        #region Delete Methods

        [HttpDelete]
        [Route("~/api/Employee/Delete/{id}")]
        public IActionResult DeleteEmployee(int? id)
        {

            if (id == null)
            {

                return Ok(1);
            }
            var employee = unitOfWork.EmployeeRepository.GetByID(id);
            if (employee == null)
            {
                return Ok(0);
            }
            var EmpCard = unitOfWork.EmployeeCardRepository.Get(filter: m => m.EmployeeID == id);
         

            if(EmpCard !=null || EmpCard.Count()>0)
            {
                unitOfWork.EmployeeCardRepository.RemovRange(EmpCard);

            }
            unitOfWork.EmployeeRepository.Delete(employee);





            var result = unitOfWork.Save();
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