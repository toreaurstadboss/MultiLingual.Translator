using Azure.AI.TextAnalytics;

namespace MultiLingual.Translator.Lib
{

    public class DetectLanguageUtil : IDetectLanguageUtil
    {

        private TextAnalyticsClient _client;

        public DetectLanguageUtil()
        {
            _client = TextAnalyticsClientFactory.CreateClient();
        }

        /// <summary>
        /// Detects language of the <paramref name="inputText"/>.
        /// </summary>
        /// <param name="inputText"></param>
        /// <remarks> <see cref="Models.LanguageCode" /> contains the language code list of languages supported</remarks>
        public async Task<DetectedLanguage> DetectLanguage(string inputText)
        {
            DetectedLanguage detectedLanguage = await _client.DetectLanguageAsync(inputText);
            return detectedLanguage;
        }

        /// <summary>
        /// Detects language of the <paramref name="inputText"/>. Returns the language name.
        /// </summary>
        /// <param name="inputText"></param>
        /// <remarks> <see cref="Models.LanguageCode" /> contains the language code list of languages supported</remarks>
        public async Task<string> DetectLanguageName(string inputText)
        {
            DetectedLanguage detectedLanguage = await DetectLanguage(inputText);
            return detectedLanguage.Name;
        }

        /// <summary>
        /// Detects language of the <paramref name="inputText"/>. Returns the language code.
        /// </summary>
        /// <param name="inputText"></param>
        /// <remarks> <see cref="Models.LanguageCode" /> contains the language code list of languages supported</remarks>
        public async Task<string> DetectLanguageIso6391(string inputText)
        {
            DetectedLanguage detectedLanguage = await DetectLanguage(inputText);
            return detectedLanguage.Iso6391Name;
        }

        /// <summary>
        /// Detects language of the <paramref name="inputText"/>. Returns the confidence score
        /// </summary>
        /// <param name="inputText"></param>
        /// <remarks> <see cref="Models.LanguageCode" /> contains the language code list of languages supported</remarks>
        public async Task<double> DetectLanguageConfidenceScore(string inputText)
        {
            DetectedLanguage detectedLanguage = await DetectLanguage(inputText);
            return detectedLanguage.ConfidenceScore;
        }

    }

}
