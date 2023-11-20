namespace MultiLingual.Translator.Lib.Models
{
    public class TextToSpeechLanguageVoiceItem
    {
        public bool IsFemale { get; set; }
        public string VoiceActor { get; set; }
        public string VoiceId
        {
            get
            {
                string[] voiceActorIdsParts = VoiceActor.Split("-");
                string voiceActorParts = voiceActorIdsParts.Count() >= 3 ? $"{voiceActorIdsParts[0].Trim()}-{voiceActorIdsParts[1].Trim()}, {voiceActorIdsParts[2].Trim()}" : "";
                return $"({voiceActorParts})";
            }
        }
    }
}
