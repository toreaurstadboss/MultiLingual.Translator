﻿namespace MultiLingual.Translator.Models
{
    public class LanguageInputModel
    {
        public string InputText { get; set; }

        public string DetectedLanguageInfo { get; set; }

        public string DetectedLanguageIso6391 { get; set; }

        public string TargetLanguage { get; set; }

        public string TranslatedText { get; set; }

        public string ActiveVoiceActorId { get; set; }

        public string AdditionalVoiceDataMetaInformation { get; set; }

        /// <summary>
        /// Shows the application/ssml+xml that specifies the voice contents. It is possible to set a lot of customization in the application/ssml+xml , such as pitch, volume, duration and other fine tuning of audio voice
        /// </summary>
        public string Transcript { get; set; }

    }
}
