﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using roleDemo.Data;
using roleDemo.Repositories;
using roleDemo.ViewModels;


namespace roleDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private IConfiguration _config;
        private IServiceProvider _serviceProvider;
        private ApplicationDbContext _context;

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public RegisterController(UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
                                IConfiguration config,
                                IServiceProvider serviceProvider,
                                ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _serviceProvider = serviceProvider;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync([FromBody] RegisterAttendeeVM RegisterAttendeeVM)
        {
            AttendeeRepo a = new AttendeeRepo(_context);
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                if (a.isExist(RegisterAttendeeVM.Email))
                {
                    var res = new
                    {
                        errorMessage = "User Already Exist",
                        StatusCode = "Invalid Register."
                    };
                    return new ObjectResult(res);
                }
                var user = new IdentityUser { UserName = RegisterAttendeeVM.Email, Email = RegisterAttendeeVM.Email, };
                var result = await _userManager.CreateAsync(user, RegisterAttendeeVM.Password);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var enCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                    await _userManager.ConfirmEmailAsync(user, enCode);
                    bool isNewAttendee = a.Create(RegisterAttendeeVM.LastName, RegisterAttendeeVM.FirstName, RegisterAttendeeVM.Email);

                    if (isNewAttendee)
                    {
                        var tokenString = GenerateJSONWebToken(user);
                        var jsonOK = new
                        {
                            tokenString = tokenString,
                            StatusCode = "OK",
                            currentUser = RegisterAttendeeVM.Email
                        };

                        return new ObjectResult(jsonOK);
                    }
                }
            }
            var jsonInvalid = new { tokenString = "", StatusCode = "Invalid register." };
            return new ObjectResult(jsonInvalid);
        }




        List<Claim> AddUserRoleClaims(List<Claim> claims, string userId)
        {
            // Get current user's roles. 
            var userRoleList = _context.UserRoles.Where(ur => ur.UserId == userId);
            var roleList = from ur in userRoleList
                           from r in _context.Roles
                           where r.Id == ur.RoleId
                           select new { r.Name };


            // Add each of the user's roles to the claims list.
            foreach (var roleItem in roleList)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleItem.Name));
            }
            return claims;
        }


        string GenerateJSONWebToken(IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim> {


            new Claim(JwtRegisteredClaimNames.Sub, user.Email),


            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),


            new Claim(ClaimTypes.NameIdentifier, user.Id)


            };


            claims = AddUserRoleClaims(claims, user.Id);


            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}