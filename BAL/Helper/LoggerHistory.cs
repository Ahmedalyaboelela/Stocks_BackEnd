using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace BAL.Helper
{
  public static class LoggerHistory
    {
        public static string getUserIdFromRequest(HttpRequest request)
        {

            var TokenAsText = request.Headers["Authorization"].ToString();
            //read token
            var stream = TokenAsText.Replace("Bearer ", "");
            string UserID = "";
            if (stream != "")
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(stream);
                var tokenS = handler.ReadToken(stream) as JwtSecurityToken;
                UserID = tokenS.Claims.First().Value;

            }
            return UserID;
        }


    }
}
