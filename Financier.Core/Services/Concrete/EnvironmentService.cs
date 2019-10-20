using Microsoft.Extensions.Logging;
using System;

namespace Financier.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        private const string FixerApiKeyEnvironmentVariableName = "FIXER_API";

        private ILogger<EnvironmentService> m_logger;

        public EnvironmentService(ILoggerFactory loggerFactory)
        {
            m_logger = loggerFactory.CreateLogger<EnvironmentService>();
        }

        public string GetFixerKey()
        {
            m_logger.LogTrace("IN EnvironmentService.GetFixerKey()");

            string connectionString = Environment.GetEnvironmentVariable(FixerApiKeyEnvironmentVariableName);

            if (string.IsNullOrEmpty(connectionString))
                throw new ApplicationException(
                    "Unable to get fixer API key from environement variable " +
                    $"'{FixerApiKeyEnvironmentVariableName}'");

            return connectionString;
        }
    }
}
