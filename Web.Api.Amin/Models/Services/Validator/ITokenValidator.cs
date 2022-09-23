using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Web.Api.Amin.Models.Services.Validator
{
    public interface ITokenValidator
    {
        Task Execute(TokenValidatedContext context);
    }
    public class TokenValidate : ITokenValidator
    {
        private UserRipository userRepository;
        private readonly UserTokenRipository userTokenRipository;

        public TokenValidate(UserRipository userRepository , UserTokenRipository userTokenRipository)
        {
            this.userRepository = userRepository;
            this.userTokenRipository = userTokenRipository;
        }
        public async Task Execute(TokenValidatedContext context)
        {
            var claimsidentity = context.Principal.Identity as ClaimsIdentity;
            if (claimsidentity?.Claims == null || !claimsidentity.Claims.Any())
            {
                context.Fail("claims not found....");
                return;
            }

            var userId = claimsidentity.FindFirst("UserId").Value;
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                context.Fail("claims not found....");
                return;
            }

            var user = userRepository.GetUser(userGuid);

            if (user.IsActive == false)
            {
                context.Fail("User not Active");
                return;
            }

            if (!(context.SecurityToken is JwtSecurityToken Token)
                || !userTokenRipository.CheckExistToken(Token.RawData))
            {
                context.Fail("توکن در دیتابیس وجود ندارد");
                return;
            }

        }
    }
}
