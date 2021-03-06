﻿using Financier.Services;
using System.Net.Http;

namespace Financier.UnitTesting.Services
{
    public class MockHttpClientFactory : IHttpClientFactory
    {
        private HttpClientHandler m_mockHandler;

        public MockHttpClientFactory(HttpClientHandler mockHandler)
        {
            m_mockHandler = mockHandler;
        }

        public HttpClient Create()
        {
            return new HttpClient(m_mockHandler);
        }
    }
}
