using System.Net.Http;

namespace Financier.Services
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient Create()
        {
            return new HttpClient();
        }
    }
}
