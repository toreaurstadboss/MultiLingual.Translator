using Azure.AI.TextAnalytics;

namespace MultiLingual.Translator.Lib
{
    public interface IDetectLanguageUtil
    {
        Task<DetectedLanguage> DetectLanguage(string inputText);
        Task<double> DetectLanguageConfidenceScore(string inputText);
        Task<string> DetectLanguageIso6391(string inputText);
        Task<string> DetectLanguageName(string inputText);
    }
}