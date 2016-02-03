using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace Mondo
{
    /// <summary>
    /// Represents an error response from the Mondo API.
    /// </summary>
    public sealed class MondoException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MondoClient"/> class.
        /// </summary>
        /// <param name="statusCode">HTTP status code.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="response">The error response returned from the API.</param>
        public MondoException(HttpStatusCode statusCode, string message, ErrorResponse response = null) :base(message)
        {
            StatusCode = statusCode;
            Response = response;
        }

        /// <summary>
        /// The HTTP status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// The error response returned from the API.
        /// </summary>
        public ErrorResponse Response { get; }

        internal static MondoException CreateFromApiResponse(HttpResponseMessage response, string body)
        {
            try
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(body);
                return new MondoException(response.StatusCode, body, errorResponse);
            }
            catch
            {
                return new MondoException(response.StatusCode, body);
            }
        }
    }
}