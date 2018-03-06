using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Text;
using Exiger.JWT.Core.Models;
using Exiger.JWT.Core.Data.EF;

namespace Exiger.JWT.Authentication.Core
{
    public class MSIdentityJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private static readonly string _secret = ConfigurationManager.AppSettings["secret"];
        private static readonly string _tokenIssuer = ConfigurationManager.AppSettings["issuer"];
        private static readonly string _tokenTarget = ConfigurationManager.AppSettings["targeturl"];
        private const string UNKNOWN = "unknown";


        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            AuditActivityLog auditLog = null;
            var signedAndEncodedToken = "";

            using (var unitOfWork = new UnitOfWorkFactory().Create())
            {
                var logIdClaim = data.Identity.FindFirst("LogId");
                if (logIdClaim != null)
                {
                    int logId = Int32.Parse(logIdClaim.Value);
                    data.Identity.RemoveClaim(logIdClaim);
                    auditLog = unitOfWork.AuditRepository.GetAuditActivityLogById(x => x, logId);
                }
                //The following case should never happen, but just in case, to ensure theer is always a log object
                if (auditLog == null)
                {
                    auditLog = new AuditActivityLog()
                    {
                        SenderUserID = data.Identity.FindFirst(ClaimTypes.Name).Value,
                        SenderRequestURL = "unknown",
                        SenderIP = "unkbnown",
                        AuditDateTime = DateTime.Now
                    };
                    unitOfWork.AuditRepository.CreateAuditActivityLog(auditLog);
                }

                var expClaim = data.Identity.FindFirst(ClaimTypes.Expiration);
                var tikenExpDateTime = expClaim == null ? DateTime.SpecifyKind(DateTime.UtcNow.AddSeconds(900), DateTimeKind.Utc) : DateTime.SpecifyKind(DateTime.Parse(expClaim.Value), DateTimeKind.Utc);
                data.Identity.RemoveClaim(expClaim);

                //Get identity's claims passed from CustomOAuthProvider
                var claimsIdentity = new ClaimsIdentity(data.Identity.Claims);

                //Currently using a symetrical key algorithm for signing
                var signingKey = new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
                var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

                //AppliesToAddress and TokenIssuerName must be valid URI’s. Not in the sense that they should be resolvable, but they must be in a valid URI format
                //The AppliesToAddress should contain the token’s audience, i.e. the website or application that will receive te token. 
                //The TokenIssuerName is the application issuing the token 
                var securityTokenDescriptor = new SecurityTokenDescriptor()
                {
                    AppliesToAddress = _tokenTarget,
                    TokenIssuerName = _tokenIssuer,
                    Subject = claimsIdentity,
                    SigningCredentials = signingCredentials,
                };

                securityTokenDescriptor.Lifetime = new System.IdentityModel.Protocols.WSTrust.Lifetime(DateTime.UtcNow, tikenExpDateTime);
                var tokenHandler = new JwtSecurityTokenHandler();
                var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
                signedAndEncodedToken = tokenHandler.WriteToken(plainToken);

                auditLog.AuthenticationToken = signedAndEncodedToken.ToString();

                unitOfWork.Commit();
            }
            return signedAndEncodedToken;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}