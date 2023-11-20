namespace MultiLingual.Translator.Lib.Models
{
  
    public class TextToSpeechLanguage
    {
       
        public string LanguageKey { get; set; }
        public List<TextToSpeechLanguageVoiceItem> VoiceActors { get; set; } = new();

    }

}
