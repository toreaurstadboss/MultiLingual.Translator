using Azure.AI.Translation.Text;
using MultiLingual.Translator.Lib.Models;

namespace MultiLingual.Translator.Lib
{

    public class TranslateUtil
    {
        private TextTranslationClient _client; 


        public TranslateUtil() { 
            _client = TextAnalyticsClientFactory.CreateTranslateClient(); 
        }

        /// <summary>
        /// Translates text using Azure AI Translate services. 
        /// </summary>
        /// <param name="targetLanguage"><see cref="LanguageCode" for a list of supported languages/></param>
        /// <param name="inputText"></param>
        /// <param name="sourceLanguage">Pass in null here to auto detect the source language</param>
        /// <returns></returns>
        public async Task<string?> Translate(string targetLanguage, string inputText, string? sourceLanguage = null)
        {
            var detectedLanguage = await _client.TranslateAsync(targetLanguage, inputText, sourceLanguage);
            if (detectedLanguage?.Value == null)
            {
                return null;
            }
            var translation = detectedLanguage.Value.SelectMany(l => l.Translations).Select(l => l.Text)?.ToList();
            string? translationText = translation?.FlattenString();
            return translationText;
        }

    }
}
