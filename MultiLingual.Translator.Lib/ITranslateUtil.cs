namespace MultiLingual.Translator.Lib
{
    public interface ITranslateUtil
    {
        Task<string?> Translate(string targetLanguage, string inputText, string? sourceLanguage = null);
    }
}