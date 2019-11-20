using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.Helper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Stocks.Controllers
{
  //  [Authorize(Roles = "SuperAdmin,Admin,Employee")]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public DashboardController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
        }
        #endregion

        [HttpGet]
        [Route("~/api/Dashboard/GetTotalsellingOrders")]
        public IActionResult GetTotalsellingOrders()
        {
            return Ok(unitOfWork.SellingOrderRepository.Get().Count());
        }

        [HttpGet]
        [Route("~/api/Dashboard/GetTotalPurchaseOrders")]
        public IActionResult GetTotalPurchaseOrders()
        {
            return Ok(unitOfWork.PurchaseOrderRepository.Get().Count());
        }

        [HttpGet]
        [Route("~/api/Dashboard/GetTotalSellingInvoice")]
        public IActionResult GetTotalSellingInvoice()
        {
            return Ok(unitOfWork.SellingInvoiceReposetory.Get().Count());
        }

        [HttpGet]
        [Route("~/api/Dashboard/GetTotalPurchaseInvoice")]
        public IActionResult GetTotalPurchaseInvoice()
        {
            return Ok(unitOfWork.PurchaseInvoiceRepository.Get().Count());
        }

        [HttpGet]
        [Route("~/api/Dashboard/GetPartners")]
        public IActionResult GetPartners()
        {
            return Ok(unitOfWork.PartnerRepository.Get().Count());
        }
        [HttpGet]
        [Route("~/api/Dashboard/GetAllEmployees")]
        public IActionResult GetAllEmployees()
        {
            var employees = unitOfWork.EmployeeRepository.Get().ToList();
            var model = _mapper.Map<IEnumerable<EmployeeModel>>(employees).ToList();

            if (model == null)
            {
                return Ok(0);
            }

            for (int i = 0; i < employees.Count(); i++)
            {
                for (int j = i; j < model.Count(); j++)
                {
                    if (model[j].EmployeeID == employees[i].EmployeeID)
                    {
                        #region Date part
                        if (employees[i].BirthDate != null)
                        {

                            model[j].BirthDate = employees[i].BirthDate.Value.ToString("d/M/yyyy");
                            model[j].BirthDateHijri = DateHelper.GetHijriDate(employees[i].BirthDate);
                        }
                        #endregion

                        #region Cards part
                        if (unitOfWork.EmployeeCardRepository.Count() > 0)
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

    }
}