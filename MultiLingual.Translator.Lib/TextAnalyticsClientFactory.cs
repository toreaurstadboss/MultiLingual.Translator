using Azure;
using Azure.AI.TextAnalytics;

namespace MultiLingual.Translator.Lib
{
    public static class TextAnalyticsClientFactory
    {

        public static TextAnalyticsClient CreateClient()
        {
            string? uri = Environment.GetEnvironmentVariable("AZURE_COGNITIVE_SERVICE_ENDPOINT", EnvironmentVariableTarget.Machine);
            string? key = Environment.GetEnvironmentVariable("AZURE_COGNITIVE_SERVICE_KEY", EnvironmentVariableTarget.Machine);
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri), "Could not get system environment variable named 'AZURE_COGNITIVE_SERVICE_ENDPOINT' Set this variable first.");
            }
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri), "Could not get system environment variable named 'AZURE_COGNITIVE_SERVICE_KEY' Set this variable first.");
            }
            var client = new TextAnalyticsClient(new Uri(uri!), new AzureKeyCredential(key!));
            return client;
        }

    }
}
