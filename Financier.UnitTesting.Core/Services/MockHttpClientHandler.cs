using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Financier.UnitTesting.Services
{
    public class MockHttpClientHandler : HttpClientHandler
    {
        private IDictionary<string, HttpResponseMessage> m_mockedResponsesByRequestUri;

        public MockHttpClientHandler(IDictionary<string, HttpResponseMessage> mockedResponsesByRequestUri)
        {
            m_mockedResponsesByRequestUri = mockedResponsesByRequestUri;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                string requestUri = request.RequestUri.ToString();
                if (m_mockedResponsesByRequestUri.ContainsKey(requestUri))
                {
                    return m_mockedResponsesByRequestUri[requestUri];
                }

                return new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            });
        }
    }
}
