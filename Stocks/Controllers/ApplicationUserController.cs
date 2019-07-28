using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BAL.Model;
using DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Stocks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        #region CTOR & Definitions
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private IPasswordHasher<ApplicationUser> passwordHasher;
        public ApplicationUserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, 
            IPasswordHasher<ApplicationUser> passwordHash)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            passwordHasher = passwordHash;
        }
        #endregion

        #region GET Methods
        [HttpGet]
        [Route("~/api/ApplicationUser/FirstOpen")]
        public IActionResult FirstOpen()
        {
            ApplicationUserModel model = new ApplicationUserModel();
            if (_userManager.Users.Count() == 0)
                return Ok(0);
            else
            {
                model.Count = _userManager.Users.Count();
                return Ok(model);
            }  
        }


        [HttpGet]
        [Route("~/api/ApplicationUser/GetLastUser")]
        public IActionResult GetLastUser()
        {

            if (_userManager.Users.Last() == null)
            {
                return Ok(0);
            }
            var LastUser = _userManager.Users.OrderBy(m=> m.Creationdate).Last();
            var model = new ApplicationUserModel();
            model.Id = LastUser.Id;
            model.UserName = LastUser.UserName;
            model.Email = LastUser.Email;
            model.FullName = LastUser.FullName;
            model.Password = LastUser.PasswordHash;
            return Ok(model);
        }

        [HttpGet]
        [Route("~/api/ApplicationUser/Paging/{pageNumber}")]
        public IActionResult Pagination(int pageNumber)
        {
            if (pageNumber > 0)
            {
                var User = _userManager.Users.OrderBy(m => m.Creationdate).Skip(pageNumber - 1).Take(1).FirstOrDefault();
                if (User == null)
                {
                    return Ok(0);
                }
                var model = new ApplicationUserModel();
                model.Id = User.Id;
                model.UserName = User.UserName;
                model.Email = User.Email;
                model.FullName = User.FullName;
                model.Password = User.PasswordHash;
                model.ConfirmPassword= User.PasswordHash;
                model.Count = _userManager.Users.Count();
                return Ok(model);
            }
            else
                return Ok(1);

        }
        #endregion

        #region Insert Method
        [HttpPost]
        [Route("Register")]
        //Post: /api/ApplicationUser/Register
        public async Task<object> PostApplicationUser(ApplicationUserModel model)
        {

            var applicationuser = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                Creationdate = DateTime.Now
            };

            try
            {
                var result = await _userManager.CreateAsync(applicationuser, model.Password);
                if(result.Succeeded)
                {
                    return Ok(result);
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        if(item.Code== "DuplicateUserName")
                        {
                            return Ok(2);
                        }
                        if (item.Code == "InvalidUserName")
                        {
                            return Ok(1);
                        }
                    }
                    return Ok(result);
                }
             
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
                return Ok(6);

            }
        }
        #endregion


        #region Update Method
        [HttpPut]
        [Route("~/api/ApplicationUser/PutUser/{id}")]
        public async Task<object> PutUser(string id, [FromBody] ApplicationUserModel model)
        {
            ApplicationUser user =await _userManager.FindByIdAsync(id);

            if(user !=null)
            {
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PasswordHash = passwordHasher.HashPassword(user, model.Password);
                user.FullName = model.FullName;
                try
                {
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            if (item.Code == "DuplicateUserName")
                            {
                                return Ok(2);
                            }
                            if (item.Code == "InvalidUserName")
                            {
                                return Ok(1);
                            }
                        }
                        return Ok(result);
                    }
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
                    return Ok(6);

                }
            }
            else
            {
                return Ok(0);
            }



        }

        #endregion


        #region Delete Method
        [HttpDelete]
        [Route("~/api/ApplicationUser/DeleteUser/{id}")]

        public async Task<object> DeleteUser(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            if(user !=null)
            {
                try
                {
                    var result = await _userManager.DeleteAsync(user);
                        return Ok(result);
                    
                }
                catch(DbUpdateException  ex)
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
                    return Ok(6);

                }

            }
            else
            {
                return Ok(0);
            }


        }
        #endregion

    }
}