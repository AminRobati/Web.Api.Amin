using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Web.Api.Amin.Models.Dto;
using Web.Api.Amin.Models.Entities;
using Web.Api.Amin.Models.Helpers;
using Web.Api.Amin.Models.Services;

namespace Web.Api.Amin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly UserRipository userRipository;
        private readonly UserTokenRipository userTokenRipository;

        public AccountsController(IConfiguration configuration , UserRipository userRipository
            ,UserTokenRipository userTokenRipository)
        {
            this.configuration = configuration;
            this.userRipository = userRipository;
            this.userTokenRipository = userTokenRipository;
        }

        [HttpPost]
        public IActionResult Post(string PhoneNumber, string SmsCode)
        {
            var loginResult = userRipository.Login(PhoneNumber, SmsCode);
            if (loginResult.IsSuccess == false)
            {
                return Ok(new LoginResultDto()
                {
                    IsSuccess = false,
                    Message = loginResult.Message
                });
            }
            var token = CreateToken(loginResult.User);

            return Ok(new LoginResultDto()
            {
                IsSuccess = true,
                Data = token,
            });
        }



        [Authorize]
        [HttpGet]
        [Route("Logout")]
        public IActionResult Logout()
        {
            var user = User.Claims.First(p => p.Type == "UserId").Value;
            userRipository.Logout(Guid.Parse(user));
            return Ok();
        }



        [HttpPost]
        [Route("RefreshToken")]
        public IActionResult RefreshToken(string Refreshtoken)
        {
            var usertoken = userTokenRipository.FindRefreshToken(Refreshtoken);
            if (usertoken == null)
            {
                return Unauthorized();
            }
            if (usertoken.RefreshTokenExp < DateTime.Now)
            {
                return Unauthorized("Token Expire");
            }

            var token = CreateToken(usertoken.User);
            userTokenRipository.DeleteToken(Refreshtoken);

            return Ok(token);
        }


        [Route("GetSmsCode")]
        [HttpGet]
        public IActionResult GetSmsCode(string PhoneNumber)
        {
            var smsCode = userRipository.GetCode(PhoneNumber);
            //smsCode پیامک کنید به همین شماره
            return Ok();
        }




        private LoginDataDto CreateToken(User user)
        {
            SecurityHelper securityHelper = new SecurityHelper();


            var claims = new List<Claim>
                {
                    new Claim ("UserId", user.Id.ToString()),
                    new Claim ("Name",  user?.Name??""),
                };
            string key = configuration["JWtConfig:Key"];
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokenexp = DateTime.Now.AddMinutes(int.Parse(configuration["JWtConfig:expires"]));
            var token = new JwtSecurityToken(
                issuer: configuration["JWtConfig:issuer"],
                audience: configuration["JWtConfig:audience"],
                expires: tokenexp,
                notBefore: DateTime.Now,
                claims: claims,
                signingCredentials: credentials
                );
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = Guid.NewGuid();

            userTokenRipository.SaveToken(new Models.Entities.UserToken()
            {
                MobileModel = "Iphone pro max",
                TokenExp = tokenexp,
                TokenHash = securityHelper.Getsha256Hash(jwtToken),
                User = user,
                RefreshToken = securityHelper.Getsha256Hash(refreshToken.ToString()),
                RefreshTokenExp = DateTime.Now.AddDays(30)
            });

            return new LoginDataDto()
            {
                Token = jwtToken,
                RefreshToken = refreshToken.ToString()
            };


        }
    }
}
