using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Configuration;
using System.Collections.Generic;
using Exiger.JWT.Core.Data.EF;
using System.Linq.Expressions;
using Exiger.JWT.Core.Models;

namespace Exiger.JWT.Authentication.Core
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        public object ConfigurationHelper { get; private set; }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            using (var unitOfWork = new UnitOfWorkFactory().Create())
            {
                string requestIPAddress = GetIpAddress();

                var auditLog = new AuditActivityLog()
                {
                    SenderUserID = context.UserName,
                    SenderRequestURL = HttpContext.Current.Request.Url.OriginalString,
                    SenderIP = requestIPAddress,
                    AuditDateTime = DateTime.Now
                };
                var auditId = unitOfWork.AuditRepository.CreateAuditActivityLog(auditLog);
                unitOfWork.Commit();

                try
                {
                    context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

                    var clientUser = unitOfWork.UserRepository.GetUserByName(x => x, context.UserName,
                        new List<Expression<Func<ClientUser, object>>> {
                            x => x.Client,
                            x => x.UserIPAddresses
                        });
                    if (clientUser == null || !(await unitOfWork.UserRepository.IsPasswordValidAsync(clientUser, context.Password)))
                    {
                        throw new AuthenticationException("The user name or password is incorrect.");
                    }

                    auditLog.LoginUser = clientUser;
                    auditLog.InsightEmail = clientUser.Email;

                    if (!clientUser.Active)
                    {
                        throw new AuthenticationException("The user account is disabled.");
                    }
                    if (clientUser.Client == null)
                    {
                        throw new AuthenticationException("The user account doesn't have a client associated with it.");
                    }

                    DateTime nowDateTime = DateTime.UtcNow;

                    //Check whitelisting of IP addresses
                    if (clientUser.Client.WhitelistIPs)
                    {
                        List<ClientIPAddresses> userIPWhiteList = clientUser.UserIPAddresses == null ? null : clientUser.UserIPAddresses.Where(x => (x.IPAddress4 == requestIPAddress)).ToList();
                        if (userIPWhiteList == null || userIPWhiteList.Count() == 0)
                        {
                            throw new AuthenticationException("Connection denied.");
                        }
                    }

                    //Check number of open tokens limit
                    int openTokensLimit = clientUser.Client.OpenTokensLimit;
                    if (openTokensLimit == 0 && !Int32.TryParse(ConfigurationManager.AppSettings["opentokenslimit"], out openTokensLimit))
                    {
                        openTokensLimit = 0;
                    }

                    if (openTokensLimit > 0)
                    {
                        List<ClientConnections> openConnections = unitOfWork.AuditRepository.GetConnectionsByClient(x => x, clientUser.Client.id)
                            .Where(x => nowDateTime >= x.TokenStartDateTimeUTC && nowDateTime <= x.TokenEndDateTimeUTC).ToList();
                        if (openConnections != null && openConnections.Count() > openTokensLimit)
                        {
                            throw new AuthenticationException("Total number of open connection per client exceeds the limit.");
                        }
                    }

                    //Set timeouts
                    int tokenTimespanSec = clientUser.Client.TokenTimeOut;
                    if (tokenTimespanSec == 0 && !Int32.TryParse(ConfigurationManager.AppSettings["timeoutseconds"], out tokenTimespanSec))
                    {
                        tokenTimespanSec = 900;
                    }

                    DateTime tokenExpiration = nowDateTime.AddSeconds(tokenTimespanSec);

                    auditLog.TokenStartDateTimeUTC = nowDateTime;
                    auditLog.TokenEndDateTimeUTC = tokenExpiration;
                    unitOfWork.Commit();

                    //record new connection
                    var newConnection = new ClientConnections()
                    {
                        LoginUser = clientUser,
                        ConnectionClientAccount = clientUser.Client,
                        ConnectionAuditLog = auditLog,
                        TokenStartDateTimeUTC = nowDateTime,
                        TokenEndDateTimeUTC = tokenExpiration
                    };
                    unitOfWork.AuditRepository.CreateClientConnectionLog(newConnection);

                    //Generate claims for the token
                    var identity = new ClaimsIdentity("JWT");
                    identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                    identity.AddClaim(new Claim(ClaimTypes.Email, clientUser.Email));
                    identity.AddClaim(new Claim(ClaimTypes.Expiration, tokenExpiration.ToString()));
                    identity.AddClaim(new Claim("LogId", auditId.ToString()));
                    var tokenEnd = DateTime.UtcNow.AddSeconds(tokenTimespanSec);

                    var ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
                    context.Validated(ticket);
                }
                catch (Exception ex)
                {
                    if (ex is AuthenticationException)
                    {
                        context.SetError("invalid_grant", ex.Message);
                    }
                    else
                    {
                        context.SetError("invalid_grant", String.Format("Internal Error (Please contact Exiger support with reference ID: {0})", auditLog.Id));
                    }
                    auditLog.ErrorMessage = ex.Message;
                }
                finally
                {
                    unitOfWork.Commit();
                }
            }
            return; 
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        private string GetIpAddress()
        {
            var request = HttpContext.Current.Request;
            // Look for a proxy address first
            var ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            // If there is no proxy, get the standard remote address
            if (string.IsNullOrWhiteSpace(ip) || string.Equals(ip, "unknown", StringComparison.OrdinalIgnoreCase))
                ip = request.ServerVariables["REMOTE_ADDR"];
            else
            {
                //extract first IP
                var index = ip.IndexOf(',');
                if (index > 0)
                    ip = ip.Substring(0, index);

                //remove port
                index = ip.IndexOf(':');
                if (index > 0)
                    ip = ip.Substring(0, index);
            }

            if (string.IsNullOrWhiteSpace(ip))
                ip = "unknown";

            return ip;
        }
    }
}