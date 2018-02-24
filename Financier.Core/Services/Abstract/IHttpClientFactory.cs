using System.Net.Http;

namespace Financier.Services
{
    public interface IHttpClientFactory
    {
        HttpClient Create();
    }
}
