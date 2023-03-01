using System.Text.Json;

namespace PassageIdentity
{
    public class PassageException : Exception
    {
        public PassageException(string message, HttpResponseMessage response, Exception? innerException = null)
            : base(message, innerException)
        {
            if (response != null)
            {
                StatusCode = (int)response.StatusCode;
                StatusText = response.ReasonPhrase;
                var responseContent = response.Content.ReadAsStringAsync().Result;
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent);
                    Error = errorResponse?.Error;
                }
                catch (JsonException)
                {
                    Error = responseContent;
                }
            }
        }

        public PassageException()
        {
        }

        public PassageException(string message) : base(message)
        {
        }

        public PassageException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public string? Error { get; set; }
        public int? StatusCode { get; set; }
        public string? StatusText { get; set; }

        private sealed class ErrorResponse
        {
            public string? Error { get; set; }
        }
    }
}
