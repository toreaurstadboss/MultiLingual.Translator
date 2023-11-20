using Microsoft.Extensions.Configuration;
using MultiLingual.Translator.Lib.Models;
using System.Security;
using System.Text;

namespace MultiLingual.Translator.Lib
{
    public class TextToSpeechUtil : ITextToSpeechUtil
    {

        public TextToSpeechUtil(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<TextToSpeechResult> GetSpeechFromText(string text, string language, TextToSpeechLanguage[] actorVoices)
        {
            var result = new TextToSpeechResult();

            result.Transcript = GetSpeechTextXml(text, language, actorVoices, result);
            result.ContentType = _configuration[TextToSpeechSpeechContentType];
            result.OutputFormat = _configuration[TextToSpeechSpeechXMicrosoftOutputFormat];
            result.UserAgent = _configuration[TextToSpeechSpeechUserAgent];
            result.AvailableVoiceActorIds = ResolveAvailableActorVoiceIds(language, actorVoices);
            result.LanguageCode = language;

            string? token = await GetUpdatedToken();

            HttpClient httpClient = GetTextToSpeechWebClient(token);

            string ttsEndpointUrl = _configuration[TextToSpeechSpeechEndpoint];
            var response = await httpClient.PostAsync(ttsEndpointUrl, new StringContent(result.Transcript, Encoding.UTF8, result.ContentType));

            using (var memStream = new MemoryStream()) {
                var responseStream = await response.Content.ReadAsStreamAsync();
                responseStream.CopyTo(memStream);
                result.VoiceData = memStream.ToArray();
            }

            return result;
        }

        private async Task<string?> GetUpdatedToken()
        {
            string? token = _token?.ToNormalString();
            if (_lastTimeTokenFetched == null || DateTime.Now.Subtract(_lastTimeTokenFetched.Value).Minutes > 8)
            {
                token = await GetIssuedToken();
            }

            return token;
        }

        private HttpClient GetTextToSpeechWebClient(string? token)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Add("X-Microsoft-OutputFormat", _configuration[TextToSpeechSpeechXMicrosoftOutputFormat]);
            httpClient.DefaultRequestHeaders.Add("User-Agent", _configuration[TextToSpeechSpeechUserAgent]);
            return httpClient;
        }
       
        private string GetSpeechTextXml(string text, string language, TextToSpeechLanguage[] actorVoices, TextToSpeechResult result)
        {
            result.VoiceActorId = ResolveVoiceActorId(language, actorVoices);
            string speechXml = $@"
            <speak version='1.0' xml:lang='en-US'>
                <voice xml:lang='en-US' xml:gender='Male' name='Microsoft Server Speech Text to Speech Voice {result.VoiceActorId}'>
                    <prosody rate='1'>{text}</prosody>
                </voice>
            </speak>";
            return speechXml;               
        }

        private List<string> ResolveAvailableActorVoiceIds(string language, TextToSpeechLanguage[] actorVoices)
        {
            if (actorVoices?.Any() == true)
            {
                var voiceActorIds = actorVoices.Where(v => v.LanguageKey == language || v.LanguageKey.Split("-")[0] == language).SelectMany(v => v.VoiceActors).Select(v => v.VoiceId).ToList();
                return voiceActorIds;
            }
            return new List<string>();
        }

        private string ResolveVoiceActorId(string language, TextToSpeechLanguage[] actorVoices)
        {
            string actorVoiceId = "(nb-NO, FinnNeural)"; //default to a voice actor id 
            if (actorVoices?.Any() == true)
            {
                var matchingLanguageFirstActorVoice = actorVoices.FirstOrDefault(v => v.LanguageKey == language || v.LanguageKey.Split("-")[0] == language);
                if (matchingLanguageFirstActorVoice != null)
                {
                    if (matchingLanguageFirstActorVoice.VoiceActors.Any() == true)
                    {
                        actorVoiceId = matchingLanguageFirstActorVoice.VoiceActors.First().VoiceId;
                    }
                }
            }
            return actorVoiceId;
        }

        private async Task<string> GetIssuedToken()
        {
            var httpClient = new HttpClient();
            string? textToSpeechSubscriptionKey = Environment.GetEnvironmentVariable("AZURE_TEXT_SPEECH_SUBSCRIPTION_KEY", EnvironmentVariableTarget.Machine);
            httpClient.DefaultRequestHeaders.Add(OcpApiSubscriptionKeyHeaderName, textToSpeechSubscriptionKey);
            string tokenEndpointUrl = _configuration[TextToSpeechIssueTokenEndpoint];
            var response = await httpClient.PostAsync(tokenEndpointUrl, new StringContent("{}"));
            _token = (await response.Content.ReadAsStringAsync()).ToSecureString();
            _lastTimeTokenFetched = DateTime.Now;
            return _token.ToNormalString();
        }

        private const string OcpApiSubscriptionKeyHeaderName = "Ocp-Apim-Subscription-Key";
        private const string TextToSpeechIssueTokenEndpoint = "TextToSpeechIssueTokenEndpoint";
        private const string TextToSpeechSpeechEndpoint = "TextToSpeechSpeechEndpoint";        
        private const string TextToSpeechSpeechContentType = "TextToSpeechSpeechContentType";
        private const string TextToSpeechSpeechUserAgent = "TextToSpeechSpeechUserAgent";
        private const string TextToSpeechSpeechXMicrosoftOutputFormat = "TextToSpeechSpeechXMicrosoftOutputFormat";

        private readonly IConfiguration _configuration;

        private DateTime? _lastTimeTokenFetched = null;
        private SecureString _token = null;

    }
}
