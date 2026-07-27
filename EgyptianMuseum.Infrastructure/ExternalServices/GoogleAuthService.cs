using EgyptianMuseum.Application.DTOs.Auth;
using EgyptianMuseum.Application.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.JsonWebTokens;

namespace EgyptianMuseum.Infrastructure.ExternalServices
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _configuration;

        public  GoogleAuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }



       // This method validates the Google ID token and returns user information if valid.
        public async Task<GoogleUserInfoDto> ValidateTokenAsync(string idToken)
        {

            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = _configuration
                    .GetSection("GoogleAuth:ClientIds")
                    .Get<string[]>()
            };



            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            return new GoogleUserInfoDto
            {
                GoogleId = payload.Subject,
                Email = payload.Email,
                Name = payload.Name
            };
        }

        //public async Task<GoogleUserInfoDto> ValidateTokenAsync(string idToken)
        //{
        //    if (string.IsNullOrWhiteSpace(idToken))
        //        throw new ArgumentException("ID Token is empty.");

        //    idToken = idToken.Trim();

        //    var clientIds = _configuration
        //        .GetSection("GoogleAuth:ClientIds")
        //        .Get<string[]>();

        //    if (clientIds == null || clientIds.Length == 0)
        //        throw new Exception("Google Client IDs are not configured.");

        //    var settings = new GoogleJsonWebSignature.ValidationSettings
        //    {
        //        Audience = clientIds,

        //        // للتجربة والتشخيص حاليًا
        //        ForceGoogleCertRefresh = true
        //    };

        //    var payload = await GoogleJsonWebSignature.ValidateAsync(
        //        idToken,
        //        settings
        //    );

        //    return new GoogleUserInfoDto
        //    {
        //        GoogleId = payload.Subject,
        //        Email = payload.Email,
        //        Name = payload.Name
        //    };
        //}
    }
}
