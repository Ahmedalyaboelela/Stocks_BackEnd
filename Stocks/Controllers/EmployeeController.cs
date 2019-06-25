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

            model.BirthDate = employee.BirthDate.Value.ToString("dd/MM/yyyy");
            model.BirthDateHijri = DateHelper.GetHijriDate(employee.BirthDate);
            #endregion

            #region Cards part
            var EmpCard = unitOfWork.EmployeeCardRepository

                .Get(filter: m => m.EmployeeID == employee.EmployeeID)
                .Select(m => new EmployeeCardModel
                {
                    EmpCardId = m.EmpCardId,
                    CardType = m.CardType,
                    IssuePlace = m.IssuePlace,
                    Code = m.Code,
                    IssueDate = m.IssueDate.Value.ToString("dd/MM/yyyy"),
                    IssueDateHigri = DateHelper.GetHijriDate(m.IssueDate),
                    EndDate = m.EndDate.Value.ToString("dd/MM/yyyy"),
                    EndDateHigri = DateHelper.GetHijriDate(m.EndDate),
                    RenewalDate = m.RenewalDate.Value.ToString("dd/MM/yyyy"),
                    RenewalDateHigri = DateHelper.GetHijriDate(m.RenewalDate),
                    Notes = m.Notes,
                    EmployeeID = m.EmployeeID,
                    Fees = m.Fees,

                });
            if (EmpCard != null)
                model.EmployeeCards = EmpCard;

            #endregion

            model.Count = unitOfWork.EmployeeRepository.Count();

            return model;
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
            var employees = unitOfWork.EmployeeRepository.Get().ToList();
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

                        model[j].BirthDate = employees[i].BirthDate.Value.ToString("dd/MM/yyyy");
                        model[j].BirthDateHijri = DateHelper.GetHijriDate(employees[i].BirthDate);
                        #endregion

                        #region Cards part
                        var EmpCard = unitOfWork.EmployeeCardRepository

                            .Get(filter: m => m.EmployeeID == employees[i].EmployeeID)
                            .Select(m => new EmployeeCardModel
                            {
                                EmpCardId = m.EmpCardId,
                                CardType = m.CardType,
                                IssuePlace = m.IssuePlace,
                                Code = m.Code,
                                IssueDate = m.IssueDate.Value.ToString("dd/MM/yyyy"),
                                IssueDateHigri = DateHelper.GetHijriDate(m.IssueDate),
                                EndDate = m.EndDate.Value.ToString("dd/MM/yyyy"),
                                EndDateHigri = DateHelper.GetHijriDate(m.EndDate),
                                RenewalDate = m.RenewalDate.Value.ToString("dd/MM/yyyy"),
                                RenewalDateHigri = DateHelper.GetHijriDate(m.RenewalDate),
                                Notes = m.Notes,
                                EmployeeID = m.EmployeeID,
                                Fees = m.Fees,

                            });
                        if (EmpCard != null)
                            model[j].EmployeeCards = EmpCard;

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

                    var model = _mapper.Map<Employee>(empModel);
                    

                    var empolyeeCard = empModel.EmployeeCards;

                    var EmpolyeeCard = _mapper.Map<IEnumerable<EmployeeCard>>(empolyeeCard);
                  
                    unitOfWork.EmployeeRepository.Insert(model);
                        
                    if (EmpolyeeCard != null)
                    {
                        foreach (var item in empolyeeCard)
                        {
                            var obj = _mapper.Map<EmployeeCard>(item);
                                   
                            obj.EmployeeID = model.EmployeeID;

                            unitOfWork.EmployeeCardRepository.Insert(obj);
                                   

                        }
                    }


                    bool CheckSave = unitOfWork.Save();



                    if (CheckSave == true)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return Ok("يوجد خطا بادخال البيانات ");
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
        public IActionResult Update(int id, [FromBody] EmployeeModel empModel)
        {
            if (id != empModel.EmployeeID)
            {

                return Ok(1);
            }

            if (ModelState.IsValid)
            {
                var model = _mapper.Map<Employee>(empModel);

                var empolyeeCard = empModel.EmployeeCards;
                //var EmpolyeeCard = _mapper.Map<IEnumerable<EmployeeCard>>(empolyeeCard);

                var Check = unitOfWork.EmployeeRepository.Get(NoTrack: "NoTrack");
                var oldcard = unitOfWork.EmployeeCardRepository

                    .Get(NoTrack: "NoTrack", filter: m => m.EmployeeID == model.EmployeeID);


                if (oldcard.Count()>0)
                {
                    unitOfWork.EmployeeCardRepository.RemovRange(oldcard);

                }

                if (Check.Any(m => m.Code != empModel.Code))
                {
                    unitOfWork.EmployeeRepository.Update(model);
                    

                    foreach (var item in empolyeeCard)
                    {
                        item.EmployeeID = model.EmployeeID;
                        item.EmpCardId = 0;
                        var newcard = _mapper.Map<EmployeeCard>(item);

                        unitOfWork.EmployeeCardRepository.Insert(newcard);

                    }

                    var Result = unitOfWork.Save();
                    if (Result == true)
                        return Ok(empModel);
                    else
                        return Ok("حدث خطا");

                }
                else
                {
                    if (Check.Any(m => m.Code == empModel.Code && m.EmployeeID == id))
                    {

                        unitOfWork.EmployeeRepository.Update(model);
                     
                        foreach (var item in empolyeeCard)
                        {
                            item.EmployeeID = model.EmployeeID;
                            item.EmpCardId = 0;
                            var newcard = _mapper.Map<EmployeeCard>(item);

                            unitOfWork.EmployeeCardRepository.Insert(newcard);

                        }

                        var Result = unitOfWork.Save();
                        if (Result == true)
                            return Ok(empModel);
                        else
                            return Ok("حدث خطا");
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
         


            unitOfWork.EmployeeCardRepository.RemovRange(EmpCard);
            unitOfWork.EmployeeRepository.Delete(employee);
            var Result = unitOfWork.Save();
            if (Result == true)
            {
                return Ok(4);
            }
            else
            {
                return Ok("not deleted");
            }

        }

        #endregion


    }
}