using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BAL.Model;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Stocks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {

        #region CTOR & Definitions
        private UserManager<ApplicationUser> _userManager;

        public UserProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
      
        }
        #endregion

        #region GET Methods

        [HttpGet]
        [Authorize]
        [Route("~/api/UserProfile/GetUserProfile")]
        public async Task<object> GetUserProfile()
        {
            string userId = User.Claims.First(m => m.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var model = new UserModel();
            model.UserName = user.UserName;
            model.Email = user.Email;
            model.FullName = user.FullName;
            return Ok(model);
        }

        #endregion
    }
}