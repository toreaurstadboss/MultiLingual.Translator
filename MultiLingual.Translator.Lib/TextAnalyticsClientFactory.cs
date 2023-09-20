using Azure;
using Azure.AI.TextAnalytics;
using Azure.AI.Translation.Text;
using System;

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
            if (key == null)
            {
                throw new ArgumentNullException(nameof(uri), "Could not get system environment variable named 'AZURE_COGNITIVE_SERVICE_KEY' Set this variable first.");
            }
            var client = new TextAnalyticsClient(new Uri(uri!), new AzureKeyCredential(key!));
            return client;
        }

        public static TextTranslationClient CreateTranslateClient()
        {
            string? keyTranslate = Environment.GetEnvironmentVariable("AZURE_TRANSLATION_SERVICE_KEY", EnvironmentVariableTarget.Machine);
            string? regionForTranslationService = Environment.GetEnvironmentVariable("AZURE_TRANSLATION_SERVICE_REGION", EnvironmentVariableTarget.Machine);

            if (keyTranslate == null)
            {
                throw new ArgumentNullException(nameof(keyTranslate), "Could not get system environment variable named 'AZURE_TRANSLATION_SERVICE_KEY' Set this variable first.");
            }
            if (keyTranslate == null)
            {
                throw new ArgumentNullException(nameof(keyTranslate), "Could not get system environment variable named 'AZURE_TRANSLATION_SERVICE_REGION' Set this variable first.");
            }
            var client = new TextTranslationClient(new AzureKeyCredential(keyTranslate!), region: regionForTranslationService);
            return client;
        }

    }
}
