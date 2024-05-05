using MultiLingual.Translator.Lib.Models;

namespace MultiLingual.Translator.Lib
{
    public interface ITextToSpeechUtil
    {
        Task<TextToSpeechResult> GetSpeechFromText(string text, string language, TextToSpeechLanguage[] actorVoices, 
            string? preferredVoiceActorId, string? preferredVoiceStyle);

        Task<List<string>> GetVoiceStyles();
    }
}