using Web.Constants;

namespace Web.Extensions
{
    public static class HttpResponseHeaderExtension
    {
        public static void AppendTotalCountHeader(this HttpResponse response, int totalCount)
        {
            response.Headers.Append(ApiConstants.TotalCountHeader, totalCount.ToString());
        }
    }
}
