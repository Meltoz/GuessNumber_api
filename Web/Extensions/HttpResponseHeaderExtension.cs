using Web.Constants;

namespace Web.Extensions
{
    public static class HttpResponseHeaderExtension
    {
        public static void AddTotalCountHeader(this HttpResponse response, int totalCount)
        {
            response.Headers.Append(ApiConstants.TotalCountHeader, totalCount.ToString());
        }
    }
}
