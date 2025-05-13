using System.Net.Http.Headers;
using System.Text;

namespace CabUserService.Infrastructures.Communications.Http
{
    /// <summary>
	/// Some useful extension methods for HttpClient.
	/// </summary>
	public static class HttpClientExtensions
    {
        /// <summary>
        /// Set the HttpRequestMessage header to use basic authentication.
        /// </summary>
        /// <param name="request">HttpRequestMessage request.</param>
        /// <param name="userName">Username value.</param>
        /// <param name="password">Password value.</param>
        public static void SetBasicAuthentication(this HttpRequestMessage request, string userName, string password) =>
            request.Headers.Authorization = new BasicAuthenticationHeaderValue(userName, password);

        /// <summary>
        /// Set the HttpRequestMessage header to use token authentication.
        /// </summary>
        /// <param name="request">HttpRequestMessage request.</param>
        /// <param name="scheme">Token scheme name.</param>
        /// <param name="token">Token string value.</param>
        public static void SetToken(this HttpRequestMessage request, string scheme, string token) =>
            request.Headers.Authorization = new AuthenticationHeaderValue(scheme, token);

        /// <summary>
        /// Set the HttpRequestMessage header to use JWT token authentication
        /// (A shortcut of SetToken method with scheme name as "JWT").
        /// </summary>
        /// <param name="request">HttpRequestMessage request.</param>
        /// <param name="token">Token string value.</param>
        public static void SetBearerToken(this HttpRequestMessage request, string token) =>
            request.SetToken("Bearer", token);
    }

    public class BasicAuthenticationHeaderValue : AuthenticationHeaderValue
    {
        public BasicAuthenticationHeaderValue(string userName, string password)
            : base("Basic", EncodeCredential(userName, password))
        { }

        private static string EncodeCredential(string userName, string password)
        {
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string credential = string.Format("{0}:{1}", userName, password);

            return Convert.ToBase64String(encoding.GetBytes(credential));
        }
    }
}