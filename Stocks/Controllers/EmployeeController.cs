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
            if(employee.BirthDate!=null)
            {

                model.BirthDate = employee.BirthDate.Value.ToString("dd/MM/yyyy");
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
                        IssueDate = m.IssueDate!=null? m.IssueDate.Value.ToString("dd/MM/yyyy"):null,
                        IssueDateHigri = m.IssueDate != null ? DateHelper.GetHijriDate(m.IssueDate): null,
                        EndDate = m.EndDate!=null? m.EndDate.Value.ToString("dd/MM/yyyy"): null,
                        EndDateHigri = m.EndDate != null ? DateHelper.GetHijriDate(m.EndDate): null,
                        RenewalDate = m.RenewalDate !=null? m.RenewalDate.Value.ToString("dd/MM/yyyy"): null,
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
                        if(employees[i].BirthDate!=null)
                        {

                            model[j].BirthDate = employees[i].BirthDate.Value.ToString("dd/MM/yyyy");
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
                               IssueDate = m.IssueDate != null ? m.IssueDate.Value.ToString("dd/MM/yyyy") : null,
                               IssueDateHigri = m.IssueDate != null ? DateHelper.GetHijriDate(m.IssueDate) : null,
                               EndDate = m.EndDate != null ? m.EndDate.Value.ToString("dd/MM/yyyy") : null,
                               EndDateHigri = m.EndDate != null ? DateHelper.GetHijriDate(m.EndDate) : null,
                               RenewalDate = m.RenewalDate != null ? m.RenewalDate.Value.ToString("dd/MM/yyyy") : null,
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

                    var model = _mapper.Map<Employee>(empModel);
                    

                    var empolyeeCard = empModel.emplCards;

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

                var empolyeeCard = empModel.emplCards;
                //var EmpolyeeCard = _mapper.Map<IEnumerable<EmployeeCard>>(empolyeeCard);

                var Check = unitOfWork.EmployeeRepository.Get(NoTrack: "NoTrack");
                var oldcard = unitOfWork.EmployeeCardRepository

                    .Get(NoTrack: "NoTrack", filter: m => m.EmployeeID == model.EmployeeID);


                if (oldcard.Count()>0)
                {
                    unitOfWork.EmployeeCardRepository.RemovRange(oldcard);

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

                if (Check.Any(m => m.Code != empModel.Code))
                {
                    unitOfWork.EmployeeRepository.Update(model);
                    
                    if(empolyeeCard!= null)
                    {
                        foreach (var item in empolyeeCard)
                        {
                            item.EmployeeID = model.EmployeeID;
                            item.EmpCardId = 0;
                            var newcard = _mapper.Map<EmployeeCard>(item);

                            unitOfWork.EmployeeCardRepository.Insert(newcard);

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
                else
                {
                    if (Check.Any(m => m.Code == empModel.Code && m.EmployeeID == id))
                    {

                        unitOfWork.EmployeeRepository.Update(model);

                        if (empolyeeCard != null || empolyeeCard.Count()>0)
                        {
                            foreach (var item in empolyeeCard)
                            {
                                item.EmployeeID = model.EmployeeID;
                                item.EmpCardId = 0;
                                var newcard = _mapper.Map<EmployeeCard>(item);

                                unitOfWork.EmployeeCardRepository.Insert(newcard);

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
         

            if(EmpCard !=null || EmpCard.Count()>0)
            {
                unitOfWork.EmployeeCardRepository.RemovRange(EmpCard);

            }
            unitOfWork.EmployeeRepository.Delete(employee);
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
            //    return Ok("not deleted");
            //}

        }

        #endregion


    }
}