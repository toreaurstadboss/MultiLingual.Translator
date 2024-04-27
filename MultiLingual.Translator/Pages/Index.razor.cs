using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MultiLingual.Translator.Lib;
using MultiLingual.Translator.Lib.Models;
using MultiLingual.Translator.Models;
using System.Text.Json;

namespace MultiLingual.Translator.Pages
{
    public partial class Index
    {

        private Azure.AI.TextAnalytics.TextAnalyticsClient? _client;
        private InputTextArea? inputTextRef;
        public LanguageInputModel Model { get; set; } = new();
        private string FlagIcon => $"images/flags/png100px/{Model.DetectedLanguageCountryCode}.png";      
        private string TargetFlagIcon => $"images/flags/png100px/{Model.TargetLanguage}.png";

        private List<NameValue> LanguageCodes = typeof(LanguageCode).GetFields().Select(f => new NameValue
        {
            Name = f.Name,
            Value = f.GetValue(f)?.ToString(),
        }).OrderBy(f => f.Name).ToList();


        private async Task<string> Submit()
        {
            if (Model.TargetLanguage == null)
            {
                return string.Empty;
            }
            var detectedLanguage = await DetectLangUtil.DetectLanguage(Model.InputText);
            Model.DetectedLanguageInfo = $"{detectedLanguage.Iso6391Name} {detectedLanguage.Name}";
            Model.DetectedLanguageIso6391 = detectedLanguage.Iso6391Name;

            Model.DetectedLanguageCountryCode = await DetectLangUtil.DetectCountryCode(detectedLanguage.Iso6391Name);

            if (_client == null)
            {
                _client = TextAnalyticsClientFactory.CreateClient();
            }
            Model.TranslatedText = await TransUtil.Translate(Model.TargetLanguage, Model.InputText, detectedLanguage.Iso6391Name);

            StateHasChanged();

            return System.Text.Json.JsonSerializer.Serialize(Model);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Model.TargetLanguage = LanguageCode.English;
                await JS.InvokeVoidAsync("exampleJsFunctions.focusElement", inputTextRef?.AdditionalAttributes?.FirstOrDefault(a => a.Key?.ToLower() == "id").Value);
                StateHasChanged();
            }
        }

        private async Task<TextToSpeechLanguage[]> GetActorVoices()
        {
            //https://learn.microsoft.com/en-us/azure/ai-services/speech-service/language-support?tabs=tts
            Stream actorVoicesStream = await FileSystem.OpenAppPackageFileAsync("voicebook.json");
            using StreamReader sr = new StreamReader(actorVoicesStream);
            string actorVoicesJson = string.Empty;
            string? line;

            while ((line = sr.ReadLine()) != null)
            {
                //Console.WriteLine(line);
                actorVoicesJson += line;
            }

            var actorVoices = JsonSerializer.Deserialize<TextToSpeechLanguage[]>(actorVoicesJson);
            return actorVoices ?? Array.Empty<TextToSpeechLanguage>();
        }

        private string? ConvertTargetLanguage(NameValue targetLanguage) => targetLanguage?.Value;

        private async Task<IEnumerable<NameValue>> SearchAvailableLanguages(string searchText)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                return LanguageCodes.Where(l => l?.Name?.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) == true).ToList();
            }
            return LanguageCodes;
        }

        private async void SpeakText()
        {
            await Submit();
            var actorVoices = await GetActorVoices();
            TextToSpeechResult textToSpeechResult = await TextToSpeechUtil.GetSpeechFromText(Model.TranslatedText, Model.TargetLanguage, actorVoices, Model.PreferredVoiceActorId);

            Model.ActiveVoiceActorId = textToSpeechResult.VoiceActorId;
            Model.Transcript = textToSpeechResult.Transcript;
            Model.AvailableVoiceActorIds = textToSpeechResult.AvailableVoiceActorIds;
            Model.AdditionalVoiceDataMetaInformation = $"Byte size voice data: {textToSpeechResult?.VoiceData?.Length}, Audio output format: {textToSpeechResult.OutputFormat}";

            var voiceFolder = Path.Combine(FileSystem.Current.AppDataDirectory, "Resources", "Raw");
            if (!Directory.Exists(voiceFolder))
            {
                Directory.CreateDirectory(voiceFolder);
            }
            if (textToSpeechResult != null)
            {
                string voiceFile = "textToSpeechVoiceOutput_" + Model.TargetLanguage + Guid.NewGuid().ToString("N") + ".mpga";
                string voiceRelativeFile = Path.Combine(voiceFile);

                string voiceFileFullPath = Path.Combine(voiceFolder, voiceFile);
                await File.WriteAllBytesAsync(voiceFileFullPath, textToSpeechResult.VoiceData);
                Stream voiceStream = File.OpenRead(voiceFileFullPath);

                var player = AudioManager.CreatePlayer(voiceStream);
                player.Play();
            }

            StateHasChanged();

          
        }

    }
}
