using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Api.Amin.Models.Contexts;
using Web.Api.Amin.Models.Entities;
using Web.Api.Amin.Models.Helpers;

namespace Web.Api.Amin.Models.Services
{
    public class UserTokenRipository
    {
        private readonly DataBaseContext context;

        public UserTokenRipository(DataBaseContext context)
        {
            this.context = context;
        }

        public void SaveToken(UserToken userToken)
        {
            context.UserTokens.Add(userToken);
            context.SaveChanges();
        }

        public UserToken FindRefreshToken(string RefreshToken)
        {
            SecurityHelper securityHelper = new SecurityHelper();

            string RefreshTokenHash = securityHelper.Getsha256Hash(RefreshToken);
            var usertoken = context.UserTokens.Include(p => p.User)
                .SingleOrDefault(p => p.RefreshToken == RefreshTokenHash);
            return usertoken;


        }



        public void DeleteToken(string RefreshToken)
        {
            var token = FindRefreshToken(RefreshToken);
            if (token != null)
            {
                context.UserTokens.Remove(token);
                context.SaveChanges();
            }
        }

        public bool CheckExistToken(string Token)
        {
            SecurityHelper securityHelper = new SecurityHelper();
            string tokenHash = securityHelper.Getsha256Hash(Token);
            var userToken = context.UserTokens.Where(p => p.TokenHash == tokenHash).FirstOrDefault();
            return userToken == null ? false : true;
        }


    }
}
