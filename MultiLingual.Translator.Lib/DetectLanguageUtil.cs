using Azure.AI.TextAnalytics;
using MultiLingual.Translator.Lib.Models;
using System.Globalization;

namespace MultiLingual.Translator.Lib
{

    public class DetectLanguageUtil : IDetectLanguageUtil
    {

        private TextAnalyticsClient _client;

        private CultureInfo[] _allCultures;

        public DetectLanguageUtil()
        {
            _client = TextAnalyticsClientFactory.CreateClient();
            _allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
        }

        /// <summary>
        /// Detects language of the <paramref name="inputText"/>.
        /// </summary>
        /// <param name="inputText"></param>
        /// <remarks> <see cref="Models.LanguageCode" /> contains the language code list of languages supported</remarks>
        public async Task<DetectedLanguage> DetectLanguage(string inputText)
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                return await Task.FromResult(new DetectedLanguage { });
            }
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

        public async Task<string?> GetLanguageCode(string? detectedLanguageName)
        {
            if (string.IsNullOrWhiteSpace(detectedLanguageName))
            {
                return null;
            }
            var languages = typeof(LanguageCode).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            return await Task.FromResult(languages.FirstOrDefault(l => l.Name.Equals(detectedLanguageName, StringComparison.InvariantCultureIgnoreCase))?.GetValue(null)?.ToString());
        }

        /// <summary>
        /// Detects country code from iso6391 code
        /// </summary>
        /// <param name="iso6391"></param>
        /// <returns></returns>
        public async Task<string?> GetCountryCode(string? iso6391)
        {
            if (iso6391 == null)
            {
                return null;
            }
            var cultureMatchedViaCultureInfoNameSecondPart = _allCultures.FirstOrDefault(c => 
                !string.IsNullOrWhiteSpace(c.Name) && c.Name.Contains("-") && c.Name.ToLower()
                .Split("-").Any(x => x.Contains(iso6391, StringComparison.InvariantCultureIgnoreCase)));
            return await Task.FromResult(cultureMatchedViaCultureInfoNameSecondPart?.Name?.Split("-")[1]?.ToLower());
        }

    }

}
