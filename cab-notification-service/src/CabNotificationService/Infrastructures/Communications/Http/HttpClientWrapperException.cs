using System.Net;

namespace CabNotificationService.Infrastructures.Communications.Http
{
    /// <summary>
	/// This exception is thrown if an entity excepted to be found but not found.
	/// </summary>
	[Serializable]
    public class HttpClientWrapperException : Exception
    {
        /// <summary>
        /// Http response status code.
        /// </summary>
        public HttpStatusCode HttpCode { get; set; }

        /// <summary>
        /// Uri of the request.
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Creates a new <see cref="HttpClientWrapperException"/> object.
        /// </summary>
        public HttpClientWrapperException()
        { }

        public HttpClientWrapperException(HttpStatusCode httpCode, Uri uri)
            : this(httpCode, uri, null)
        { }

        /// <summary>
        /// Creates a new <see cref="HttpClientWrapperException"/> object.
        /// </summary>
        public HttpClientWrapperException(HttpStatusCode httpCode, Uri uri, Exception innerException)
            : base($"Failed to access to URL: {uri.AbsoluteUri}, status code: {httpCode}", innerException)
        {
            HttpCode = httpCode;
            Uri = uri;
        }

        /// <summary>
        /// Creates a new <see cref="HttpClientWrapperException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public HttpClientWrapperException(string message)
            : base(message)
        { }

        /// <summary>
        /// Creates a new <see cref="HttpClientWrapperException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public HttpClientWrapperException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}