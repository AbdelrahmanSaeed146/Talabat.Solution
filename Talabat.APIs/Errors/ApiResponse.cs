
namespace Talabat.APIs.Errors
{
    public class ApiResponse
    {
        public int StausCode { get; set; }
        public string? Message { get; set; }

        public ApiResponse(int statuscode , string? mess = null)
        {
            StausCode = statuscode;
            Message = mess ?? GetDefulMessageForStatusCode(statuscode) ;
        }

        private string? GetDefulMessageForStatusCode(int statuscode)
        {
            return statuscode switch
            {
                400 => "Bad Request",
                401 => " Unautorized",
                404 => "Not Found ",
                500 => " Internal Server Error",
                _ => null,
            };
        }
    }
}
