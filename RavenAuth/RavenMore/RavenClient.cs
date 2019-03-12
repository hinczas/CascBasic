using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Owin.Security.Providers.Raven.RavenMore
{
    public class RavenClient
    {
        #region Constants
        /// <summary>
        /// The URL of the default Raven authentication page.
        /// </summary>
        public const String RAVEN_BASE_URL = "https://demo.raven.cam.ac.uk/auth/";
        /// <summary>
        /// The filename of the authentication page.
        /// </summary>
        public const String RAVEN_AUTHENTICATE = "authenticate.html";
        /// <summary>
        /// The filename of the logout page.
        /// </summary>
        public const String RAVEN_LOGOUT = "logout.html";
        /// <summary>
        /// The name of the parameter used by Raven to send a response.
        /// </summary>
        public const String WLS_RESPONSE_PARAM = "WLS-Response";
        /// <summary>
        /// The default path to where the certificates are stored.
        /// </summary>
        public const String CERTIFICATE_PATH = "~/App_Data/";
        #endregion

        #region Instance members
        /// <summary>
        /// The URL of the Raven authentication page we will use.
        /// </summary>
        private String baseURL = RAVEN_BASE_URL;
        /// <summary>
        /// The path to where the certificates are stored.
        /// </summary>
        private String certificatePath = CERTIFICATE_PATH;
        /// <summary>
        /// The certificate store (this used to be a X509Store).
        /// </summary>
        private Dictionary<String, X509Certificate2> certificates;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the URL to which users will be redirected for authentication.
        /// </summary>
        public String BaseURL
        {
            get { return this.baseURL; }
            set { this.baseURL = value; }
        }
        /// <summary>
        /// Gets or sets the path to where the certificates are stored.
        /// </summary>
        public String CertificatePath
        {
            get { return this.certificatePath; }
            set { this.certificatePath = value; }
        }
        /// <summary>
        /// Gets the URL which users may use to log out of Raven.
        /// </summary>
        public String LogoutURL
        {
            get
            {
                return String.Format("{0}{1}", this.baseURL, RAVEN_LOGOUT);
            }
        }
        #endregion


        public RavenClient()
        {
            this.LoadCertificate();
        }

        public string ProviderName
        {
            get { return "Raven"; }
        }

        public void RequestAuthentication(System.Web.HttpContextBase context, Uri returnUrl)
        {
            RavenRequest ravenRequest = new RavenRequest();
            ravenRequest.Parameters.Add("url", returnUrl.AbsoluteUri);
            //string url = baseUrl + "&redirect_uri=" + HttpUtility.UrlEncode(returnUrl.ToString());
            string url = String.Format("{0}{1}{2}", this.BaseURL, RAVEN_AUTHENTICATE, ravenRequest.ToString());
            context.Response.Redirect(url);
        }

        public ExternalLoginInfo GetExternalLoginInfo(System.Web.HttpContextBase context, RavenCallbackCode ravenCallbackCode)
        {
            HttpRequestBase request = context.Request;
            HttpResponseBase response = context.Response;
            
            // if this is not a POST request, then we can look for a response
            if (!request.HttpMethod.Equals("POST"))
            {
                // try to get the response
                String wlsResponse = request.Params[WLS_RESPONSE_PARAM];

                if (!String.IsNullOrWhiteSpace(wlsResponse))
                {
                    // parse the response data
                    RavenResponse ravenResponse = new RavenResponse(wlsResponse);

                    // if the server has indicated that authentication was successful,
                    // validate the response signature and set an authentication cookie
                    if (ravenResponse.Status == RavenStatus.OK)
                    {
                        if (!this.Validate(ravenResponse))
                            throw new RavenException("Failed to validate response signature.");

                        // create a Forms authentication ticket and cookie
                        //this.CreateTicket(response, ravenResponse);

                        // redirect the user back to where they started
                        //response.Redirect(ravenResponse.URL);

                        //bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(ravenResponse.Principal));
                        //if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(ravenResponse.Principal).Count > 1)
                        //{
                        if (ravenCallbackCode == RavenCallbackCode.Login)
                            this.LoadIdentity(context, ravenResponse);

                        return GetExternalLoginInfo(context, ravenResponse);
                        //this.CreateTicket(response, ravenResponse);
                        //}

                        //return new AuthenticationResult(false, ProviderName, ravenResponse.Principal, ravenResponse.Principal, null);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            // At this point authe=orisation failed. Return null authorisation.
            return null;
        }

        #region LoadCertificate
        /// <summary>
        /// Loads the Raven public key(s).
        /// </summary>
        private void LoadCertificate()
        {
            // initialise a dictionary for certificates
            this.certificates = new Dictionary<String, X509Certificate2>();

            // find the absolute path to the certificates
            String path = HttpContext.Current.Server.MapPath(this.certificatePath);

            // load all certificates from that folder
            foreach (String certFilename in Directory.EnumerateFiles(path, "pubkey*.crt"))
            {
                X509Certificate2 cert = new X509Certificate2(certFilename);

                this.certificates.Add(Path.GetFileNameWithoutExtension(certFilename), cert);
            }
        }
        #endregion

        #region CreateTicket
        /// <summary>
        /// Generate a Forms authentication ticket for the current session.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="ravenResponse"></param>
        private void CreateTicket(HttpResponseBase response, RavenResponse ravenResponse)
        {
            // generate a ticket for the Raven session
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    ravenResponse.Parameters["principal"],
                    DateTime.Now,
                    ravenResponse.Expires,
                    false,
                    ravenResponse.Principal);

            // encrypt it so that the client can't mess with its contents
            String encryptedTicket = FormsAuthentication.Encrypt(ticket);

            // put it in a cookie
            HttpCookie formsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

            response.Cookies.Add(formsCookie);
        }
        #endregion

        #region LoadIdentity
        /// <summary>
        /// Attempts to load the user's identity from a cookie.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Returns true if the user is authenticated or false if not.</returns>
        public bool LoadIdentity(HttpContextBase context, RavenResponse response)
        {
            // try to find the authentication cookie
            //HttpCookie authCookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];
            //HttpCookie authCookie1 = context.Request.Cookies["__RequestVerificationToken"];
            //HttpCookie authCookie2 = context.Request.Cookies["ASP.NET_SessionId"];
            //HttpCookie authCookie3 = context.Request.Cookies["__RavenState"];

            if (response != null)
            {
                // decrypt the contents
                //FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie1.Value);

                var ravenUser = new RavenPrincipal(new RavenIdentity(response.Principal));
                //ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>()
                //{
                //    ravenClaim
                //});

                //IPrincipal principal = claimsPrincipal as IPrincipal;

                context.User = ravenUser;

                // there is an active session
                return true;
            }

            // there is no active session
            return false;
        }
        #endregion

        #region ExternalLoginInfo
        /// <summary>
        /// Attempts to load the user's identity from a cookie.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Returns true if the user is authenticated or false if not.</returns>
        private ExternalLoginInfo GetExternalLoginInfo(HttpContextBase context, RavenResponse ravenResponse)
        {
            var loginInfo = new UserLoginInfo(ProviderName, ravenResponse.Principal);
            ClaimsIdentity claims = context.User.Identity as ClaimsIdentity;
            var externalLogin = new ExternalLoginInfo()
            {
                DefaultUserName = ravenResponse.Principal,
                Email = string.Empty,
                Login = loginInfo,
                ExternalIdentity = claims
            };

            return externalLogin;
        }
        #endregion

        #region Validate
        /// <summary>
        /// Validates the signature of a response.
        /// </summary>
        /// <param name="response"></param>
        private Boolean Validate(RavenResponse response)
        {
            // find the public key corresponding to the key used by the
            // server to generate the response signature
            String key = String.Format("pubkey{0}", response.CertificateID);

            if (!this.certificates.ContainsKey(key))
                throw new RavenException("Couldn't find the public key needed to validate the response.");

            X509Certificate2 cert = this.certificates[key];

            // calculate a hash for the signature data:
            // 1. convert the signature data into bytes using the ASCII encoding
            // 2. calculate a SHA1 hash for the bytes
            SHA1Managed sha1 = new SHA1Managed();
            ASCIIEncoding ascii = new ASCIIEncoding();
            Byte[] asciiHash = sha1.ComputeHash(ascii.GetBytes(response.SignatureData));

            // 3. validate this hash against the signature obtained from the response
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key;
            RSAPKCS1SignatureDeformatter def = new RSAPKCS1SignatureDeformatter(csp);
            def.SetHashAlgorithm("SHA1");

            return def.VerifySignature(asciiHash, response.Signature);
        }
        #endregion
    }
}