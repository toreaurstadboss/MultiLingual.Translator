namespace MultiLingual.Translator.Lib.Models
{
    public class TextToSpeechLanguageVoiceItem
    {
        public bool IsFemale { get; set; }
        public string? VoiceActor { get; set; }
        public string VoiceId
        {
            get
            {
                var voiceActorIdsParts = VoiceActor?.Split("-");
                string voiceActorParts = voiceActorIdsParts?.Length >= 3 ? $"{voiceActorIdsParts[0].Trim()}-{voiceActorIdsParts[1].Trim()}, {voiceActorIdsParts[2].Trim()}" : "";
                return $"({voiceActorParts})";
            }
        }
    }
}
