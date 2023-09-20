using Azure.AI.TextAnalytics;

namespace MultiLingual.Translator.Lib
{

    public class DetectLanguageUtil
    {

        private TextAnalyticsClient _client;

        public DetectLanguageUtil()
        {
            _client = TextAnalyticsClientFactory.CreateClient();
        }

        public async Task<DetectedLanguage> DetectLanguage(string inputText)
        {
            DetectedLanguage detectedLanguage = await _client.DetectLanguageAsync(inputText);
            return detectedLanguage;
        }

        public async Task<string> DetectLanguageName(string inputText)
        {
            DetectedLanguage detectedLanguage = await DetectLanguage(inputText);
            return detectedLanguage.Name;
        }

        public async Task<string> DetectLanguageIso6391(string inputText)
        {
            DetectedLanguage detectedLanguage = await DetectLanguage(inputText);
            return detectedLanguage.Iso6391Name;
        }

        public async Task<double> DetectLanguageConfidenceScore(string inputText)
        {
            DetectedLanguage detectedLanguage = await DetectLanguage(inputText);
            return detectedLanguage.ConfidenceScore;
        }
      
    }

}
