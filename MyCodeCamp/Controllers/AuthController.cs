using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using MyCodeCamp.Filters;
using MyCodeCamp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MyCodeCamp.Controllers
{
    public class AuthController : Controller
    {
        private SignInManager<CampUser> _signInMgr;
        private CampContext _context;
        private ILogger<AuthController> _logger;
        private UserManager<CampUser> _userManager;
        private IPasswordHasher<CampUser> _passwordHasher;
        private IConfigurationRoot _config;

        public AuthController(CampContext context, SignInManager<CampUser> signInMgr, ILogger<AuthController> logger, UserManager<CampUser> userManager, IPasswordHasher<CampUser> passwordHasher, IConfigurationRoot config)
        {
            _context = context;
            _signInMgr = signInMgr;
            _logger = logger;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _config = config;
        }
        [HttpPost("api/auth/login")]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] CredentialModel model)
        {
            try
            {
                var result = await _signInMgr.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (result.Succeeded)
                {
                    return Ok();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception thrown while loggin in: {e}");
            }
            return BadRequest("Failed to login");

        }

        [ValidateModel]
        [HttpPost("api/auth/token")]
        public async Task<IActionResult> CreateToken([FromBody] CredentialModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if(user != null)
                {
                    if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) ==
                        PasswordVerificationResult.Success)
                    {

                        var userClaims = await _userManager.GetClaimsAsync(user);
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                            new Claim(JwtRegisteredClaimNames.Email, user.Email)  
                        }.Union(userClaims);

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));

                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            issuer: _config["Tokens:Issuer"], 
                            audience: _config["Tokens:Audience"],
                            claims: claims,
                            expires: DateTime.UtcNow.AddMinutes(15),
                            signingCredentials: creds
                        );

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        });
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception thrown while creating JWT: {e}");
            }
            return BadRequest("Failed to login");
        }
    }
}
