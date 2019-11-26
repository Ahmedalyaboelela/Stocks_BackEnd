using AutoMapper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace BAL.Helper
{
  public   class LoggerHistory
    {
        private  UnitOfWork unitOfWork;
        private  IMapper _mapper;

        public LoggerHistory(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
        }
        public  string getUserIdFromRequest(HttpRequest request)
        {

            var TokenAsText = request.Headers["Authorization"].ToString();
            //read token
            var stream = TokenAsText.Replace("Bearer ", "");
            string UserID = "";
            if (stream != "")
            {
                var handler = new JwtSecurityTokenHandler();
               
                var tokenS = handler.ReadToken(stream) as JwtSecurityToken;
                UserID = tokenS.Claims.First().Value;

            }
            return UserID;
        }

        public  void InsertUserLog(string UserID, string PageName, string OperationName, bool MobileView)
        {
            
            UserLogModel userLogModel = new UserLogModel();
            userLogModel.MobileView = MobileView;
            userLogModel.OperationName = OperationName;
            userLogModel.PageName = PageName;
            userLogModel.UserId = UserID;

            var tempmapper =_mapper;
            var model = _mapper.Map<UserLog>(userLogModel);
            model.OperationDate = DateTime.Now;
            unitOfWork.UserLogRepository.Insert(model);
            unitOfWork.Save();

        }
    }
}
