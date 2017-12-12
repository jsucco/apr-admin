using System;
using System.Web.Http.Filters;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace menu.Helpers
{
    public class CustomAuthenication : Attribute, IAuthenticationFilter
    {
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = context.Request;

            AuthenticationHeaderValue authHeader = request.Headers.Authorization;

            if (authHeader == null)
            {
                return;
            }
            if (authHeader.Scheme != "Basic")
            {
                return;
            }

            if (string.IsNullOrEmpty(authHeader.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing Credentials", request);
            }

            Tuple<string, string> userNameAndPasword = ExtractUserNameAndPassword(authHeader.Parameter);
            if (userNameAndPasword == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
            }

            IPrincipal principal = await AuthenicateCredsAsync(userNameAndPasword.Item1, userNameAndPasword.Item2, context);

            if (principal == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request); 
                
            }
            else
            {
                context.Principal = principal;
            }

        }

        private async Task<IPrincipal> AuthenicateCredsAsync(string user, string pass, HttpAuthenticationContext context)
        {
            IPrincipal incomingPrincipal = null;

            if (string.IsNullOrEmpty(user) == false && string.IsNullOrEmpty(pass) == false)
            {
                string Appuser = System.Configuration.ConfigurationManager.AppSettings["user"];
                string Apppass = System.Configuration.ConfigurationManager.AppSettings["pass"];
                if (Appuser == user && Apppass == pass)
                {
                    incomingPrincipal = context.ActionContext.RequestContext.Principal;
                    bool Authenticated = incomingPrincipal.Identity.IsAuthenticated;
                    IPrincipal genericPrincipal = new GenericPrincipal(new GenericIdentity("sa", "CustomIdentification"), new string[] { "Admin", "APIUser" });
                    context.Principal = genericPrincipal;
                }
            }

            return incomingPrincipal;
        }
        private Tuple<string, string> ExtractUserNameAndPassword(string authparameter)
        {
            Tuple<string, string> creds = new Tuple<string, string>("","");

            try
            {
                string headerValue = System.Text.ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(authparameter));
                if (String.IsNullOrEmpty(headerValue) == false)
                {
                    string[] credsArr = headerValue.Split(':');
                    if (credsArr.Length > 1)
                    {
                        creds = Tuple.Create(credsArr[0], credsArr[1]); 

                    }
                }
                
            } catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
            return creds; 
        }
        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                IPrincipal incomingPrincipal = context.ActionContext.RequestContext.Principal;
                bool Authenticated = incomingPrincipal.Identity.IsAuthenticated;
            });
        }

        public bool AllowMultiple
        {
            get { return false; }
        }
    
    }

    internal class AuthenticationFailureResult : IHttpActionResult
    {
        private HttpRequestMessage request;
        private string v;

        public AuthenticationFailureResult(string v, HttpRequestMessage request)
        {
            ReasonPhrase = v;
            Request = request;
        }

        public string ReasonPhrase { get; private set; }

        public HttpRequestMessage Request { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            response.RequestMessage = Request;
            response.ReasonPhrase = ReasonPhrase;
            return response;
        }
    }
}