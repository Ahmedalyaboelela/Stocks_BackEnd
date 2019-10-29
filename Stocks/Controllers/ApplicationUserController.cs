using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BAL.Model;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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
        private readonly ApplicationSettings _appSettings;
        private RoleManager<IdentityRole> _roleManager;
        public ApplicationUserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, 
            IPasswordHasher<ApplicationUser> passwordHash,IOptions<ApplicationSettings>appSettings,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            passwordHasher = passwordHash;
            _appSettings = appSettings.Value;
            _roleManager = roleManager;
        }
        #endregion

        #region GET Methods
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        [Route("~/api/ApplicationUser/GetAllRoles")]
        public IEnumerable<RoleModel> GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();
            var model = new List<RoleModel>();
            roles.ForEach(item => model.Add(
                new RoleModel
                {
                    Id=item.Id,
                    Name=item.Name
                }
                ));
            return (model);
        }
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        [Route("~/api/ApplicationUser/FirstOpen")]
        public IActionResult FirstOpen()
        {
            UserModel model = new UserModel();
            if (_userManager.Users.Count() == 0)
            {
                model.RoleModels = GetAllRoles();
                return Ok(model);
            }
            
            else
            {
                model.Count = _userManager.Users.Count();
                model.RoleModels = GetAllRoles();
                return Ok(model);
            }  
        }


        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        [Route("~/api/ApplicationUser/GetLastUser")]
        public IActionResult GetLastUser()
        {
            
            if (_userManager.Users.Last() == null)
            {
                return Ok(0);
            }
            var LastUser = _userManager.Users.OrderBy(m=> m.Creationdate).Last();
            var model = new UserModel();
            model.Id = LastUser.Id;
            model.UserName = LastUser.UserName;
            model.Email = LastUser.Email;
            model.FullName = LastUser.FullName;
            model.Password = LastUser.PasswordHash;
            return Ok(model);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        [Route("~/api/ApplicationUser/Paging/{pageNumber}")]
        public async Task<IActionResult> Pagination(int pageNumber)
        {
            if (pageNumber > 0)
            {
            
                var User = _userManager.Users.OrderBy(m => m.Creationdate).Skip(pageNumber - 1).Take(1).FirstOrDefault();
                if (User == null)
                {
                    return Ok(0);
                }
                var model = new UserModel();
                model.Id = User.Id;
                model.UserName = User.UserName;
                model.Email = User.Email;
                model.FullName = User.FullName;
                model.Password = User.PasswordHash;
                model.ConfirmPassword= User.PasswordHash;
                model.Count = _userManager.Users.Count();
                var UserRoles = await _userManager.GetRolesAsync(User);
                model.Role = UserRoles[0];
                model.RoleModels = GetAllRoles();
         
                return Ok(model);
            }
            else
                return Ok(1);

        }
        #endregion

        #region Insert Method
        [HttpPost]
   //     [Authorize(Roles = "SuperAdmin")]
        [Route("Register")]
        //Post: /api/ApplicationUser/Register
        public async Task<object> PostApplicationUser(UserModel model)
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
                       await _userManager.AddToRoleAsync(applicationuser, model.Role);
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

        [HttpPost]
        [Route("Login")]
        //Post: /api/ApplicationUser/Login
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.LoginUserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.LoginPassword))
            {
                // Get Role assigned to the user 
                var UserRoles = await _userManager.GetRolesAsync(user);
                IdentityOptions _options = new IdentityOptions();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim(_options.ClaimsIdentity.RoleClaimType,UserRoles.FirstOrDefault())
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)),
                    SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token});
            }
            else
                return BadRequest(new {message="Username or Password is incorrect."});
        }
        #endregion


        #region Update Method
        [HttpPut]
        [Authorize(Roles = "SuperAdmin")]
        [Route("~/api/ApplicationUser/PutUser/{id}")]
        public async Task<object> PutUser(string id, [FromBody] UserModel model)
        {
            ApplicationUser user =await _userManager.FindByIdAsync(id);
            var UserRoles = await _userManager.GetRolesAsync(user);
            string OldRole = UserRoles[0];
            if (user !=null)
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
                        await _userManager.RemoveFromRoleAsync(user, OldRole);
                        await _userManager.AddToRoleAsync(user, model.Role);
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
        [Authorize(Roles = "SuperAdmin")]
        [Route("~/api/ApplicationUser/DeleteUser/{id}")]

        public async Task<object> DeleteUser(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            var UserRoles = await _userManager.GetRolesAsync(user);
            string UserRole = UserRoles[0];
            if (user !=null)
            {
                try
                {
                    await _userManager.RemoveFromRoleAsync(user, UserRole);
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