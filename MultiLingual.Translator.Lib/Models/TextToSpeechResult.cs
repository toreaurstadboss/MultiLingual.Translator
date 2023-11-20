namespace MultiLingual.Translator.Lib.Models
{
    public class TextToSpeechResult
    {

        /// <summary>
        /// Voice data , the audible text to speech output
        /// </summary>
        public byte[]? VoiceData { get; set; }

        /// <summary>
        /// The resolved VoiceActorId
        /// 
        /// </summary>
        public string? VoiceActorId { get; set; }

        /// <summary>
        /// The language of the voice data
        /// </summary>
        public string? LanguageCode { get; set; }

        /// <summary>
        /// The output format of specified voice data
        /// </summary>
        public string? OutputFormat { get; set; }

        /// <summary>
        /// User agent for the voice data
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// The transcript of the voice data, the Speech XML
        /// </summary>
        public string? Transcript { get; set; }

        /// <summary>
        /// The content type of the voice data
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        ///  A list of voice actors available for given language
        /// </summary>
        public List<string> AvailableVoiceActorIds { get; set; } = new List<string>();


    }
}
