namespace Web.Extensions
{
    public static class HttpResponseCookieExtensions
    {
        public static void AppendCookie(this HttpResponse response,
                                         string name,
                                         string value,
                                         TimeSpan? expiryDuration = null,
                                         bool httpOnly = true,
                                         bool secure = true,
                                         string path = "/",
                                         string domain = null)
        {
            var options = new CookieOptions
            {
                HttpOnly = httpOnly,
                Secure = secure,
                SameSite = SameSiteMode.None,
                Path = path,
            };

            if (expiryDuration.HasValue)
            {
                options.Expires = DateTime.UtcNow.Add(expiryDuration.Value);
            }

            if (!string.IsNullOrEmpty(domain))
            {
                options.Domain = domain;
            }

            response.Cookies.Append(name, value, options);
        }

        public static void DeleteCookie(this HttpResponse response, string name, string path = "/", bool httpOnly = true, bool secure = true, string domain = null)
        {
            var options = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
                Path = path,
                SameSite= SameSiteMode.None,
                Secure= secure,
                HttpOnly= httpOnly,
                MaxAge = TimeSpan.Zero
            };

            if (!string.IsNullOrEmpty(domain)) 
                options.Domain = domain;

            response.Cookies.Delete(name, options);
        }
    }
}
