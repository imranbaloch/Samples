using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Web;

namespace AspNetWebFormsWithIdentityServer3
{
    public class JsonWebTokenHttpModule : IHttpModule
    {
        private static readonly Lazy<string> _securityKey = new Lazy<string>(() => GetSecurityKey());

        private static string _authority = ConfigurationManager.AppSettings["Authority"];

        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += new EventHandler(JsonWebTokenHandler);
        }

        private void JsonWebTokenHandler(Object source, EventArgs e)
        {
            var context = HttpContext.Current;
            var request = context.Request;            
            var authorizationHeader = request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return;
            }
            var token = authorizationHeader.Substring(7);
            try
            {
                ValidateTokenAndSetIdentity(token);
            }
            catch (SecurityTokenValidationException ex)
            {
                // log error here
            }
            catch
            {
                // log error here
            }
        }

        private void ValidateTokenAndSetIdentity(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();
            SecurityToken validToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validToken);
            Thread.CurrentPrincipal = principal;
            HttpContext.Current.User = principal;
        }

        private static TokenValidationParameters GetValidationParameters()
        {
            var bytes = Convert.FromBase64String(_securityKey.Value);
            var token = new X509SecurityToken(new X509Certificate2(bytes));
            return new TokenValidationParameters
            {
                ValidAudience = _authority + "/resources",
                ValidIssuer = _authority,
                IssuerSigningKeyResolver = (arbitrarily, declaring, these, parameters) => { return token.SecurityKeys.First(); },
                IssuerSigningToken = token
            };
        }

        private static string GetSecurityKey()
        {
            var webClient = new WebClient();
            var endpoint = _authority + "/.well-known/openid-configuration";
            var json = webClient.DownloadString(endpoint);
            dynamic metadata = JsonConvert.DeserializeObject<dynamic>(json);
            var jwksUri = metadata.jwks_uri.Value;
            json = webClient.DownloadString(jwksUri);
            var key = JsonConvert.DeserializeObject<dynamic>(json).keys[0];
            return (string)key.x5c[0];
        }

        public void Dispose()
        {
            
        }
    }
}