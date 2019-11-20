using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Stocks.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserLogsController : ControllerBase
    {
        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;
       
        public UserLogsController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
           
        }
        #endregion

        [Route("~/api/UserLogs/GetHistory")]
        public IActionResult GetHistory()
        {
            var History = unitOfWork.UserLogRepository.Get().Select(z=> new UserLogModel {
                UserName=z.User.UserName,
                OperationDate=z.OperationDate.ToString(),
                OperationName=z.OperationName,
                PageName=z.PageName,
                MobileView=z.MobileView,
                UserId=z.UserId,
                UserLogID=z.UserLogID
                


            });

           

            if (History == null)
            {
                return Ok(0);
            }

            return Ok(History);
        }
    }
}