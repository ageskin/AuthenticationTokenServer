using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Exiger.JWT.Authentication
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //Enable Cross-Origin Resource Sharing (CORS) to allow client requests access to a resource from an origin/domain that is different from the domain of the resource itself
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            config.MessageHandlers.Add(new PreflightRequestsHandler());

            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }

    public class PreflightRequestsHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //In the CORS workflow, before sending a DELETE, PUT or POST request, 
            //the client sends an OPTIONS request to check that the domain from which the request originates is the same as the server.
            //If the request domain and server domain are not the same, then the server must include various access headers that describe which domains have access. 
            //To enable access to all domains, we just respond with an origin header(Access-Control - Allow - Origin) with an asterisk to enable access for all.
            //      The Access-Control   - Allow - Headers header describes which headers the API can accept /is expecting to receive.
            //      The Access - Control - Allow - Methods header describes which HTTP verbs are supported / permitted.
              if (request.Headers.Contains("Origin") && request.Method.Method == "OPTIONS")
            {
                var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Headers", "Origin, Content-Type, Accept, Authorization");
                response.Headers.Add("Access-Control-Allow-Methods", "*");
                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);
                return tsc.Task;
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
