namespace MultiLingual.Translator.Models
{
    public class LanguageInputModel
    {
        public string InputText { get; set; }

        public string DetectedLanguageInfo { get; set; }

        public string DetectedLanguageIso6391 { get; set; }


        public string TranslatedText { get; set; }

    }
}
