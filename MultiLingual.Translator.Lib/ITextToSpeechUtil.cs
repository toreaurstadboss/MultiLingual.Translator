namespace MultiLingual.Translator.Lib
{
    public interface ITextToSpeechUtil
    {
        Task<byte[]> GetSpeechFromText(string text, string language);
    }
}