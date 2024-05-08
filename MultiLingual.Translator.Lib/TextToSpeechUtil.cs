using Microsoft.Extensions.Configuration;
using MultiLingual.Translator.Lib.Models;
using System;
using System.Security;
using System.Text;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MultiLingual.Translator.Lib
{
    public class TextToSpeechUtil : ITextToSpeechUtil
    {

        public TextToSpeechUtil(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<TextToSpeechResult?> GetSpeechFromText(string? text, string? language, TextToSpeechLanguage[] actorVoices, 
            string? preferredVoiceActorId, string? preferredVoiceStyle)
        {
            var result = new TextToSpeechResult();

            result.Transcript = GetSpeechTextXml(text, language, actorVoices, preferredVoiceActorId, preferredVoiceStyle, result);
            result.ContentType = _configuration[TextToSpeechSpeechContentType];
            result.OutputFormat = _configuration[TextToSpeechSpeechXMicrosoftOutputFormat];
            result.UserAgent = _configuration[TextToSpeechSpeechUserAgent];
            result.AvailableVoiceActorIds = ResolveAvailableActorVoiceIds(language, actorVoices);
            result.LanguageCode = language;

            string? token = await GetUpdatedToken();

            HttpClient httpClient = GetTextToSpeechWebClient(token);

            string? ttsEndpointUrl = _configuration[TextToSpeechSpeechEndpoint];
            if (ttsEndpointUrl == null || result.ContentType == null)
            {
                return null;
            }
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
       
        public string GetSpeechTextXml(string? text, string? language, TextToSpeechLanguage[] actorVoices, string? preferredVoiceActorId,
              string? preferredVoiceStyle, TextToSpeechResult result)
        {
            result.VoiceActorId = ResolveVoiceActorId(language, preferredVoiceActorId, actorVoices);
            string speechXml = $@"
            <speak version='1.0' xml:lang='en-US' xmlns:mstts='https://www.w3.org/2001/mstts'>
                <voice xml:gender='Male' name='Microsoft Server Speech Text to Speech Voice {result.VoiceActorId}'>
                    <prosody rate='1'>{text}</prosody>
                </voice>
            </speak>";

            speechXml = AddVoiceStyleEffectIfDesired(preferredVoiceStyle, speechXml);

            return speechXml;
        }

        /// <summary>
        /// Adds voice style / expression to the SSML markup for the voice
        /// </summary>
        private static string AddVoiceStyleEffectIfDesired(string? preferredVoiceStyle, string speechXml)
        {
            if (!string.IsNullOrWhiteSpace(preferredVoiceStyle) && preferredVoiceStyle != "normal-neutral")
            {
                var voiceDoc = XDocument.Parse(speechXml); //https://learn.microsoft.com/nb-no/azure/ai-services/speech-service/speech-synthesis-markup-voice#use-speaking-styles-and-roles

                XElement? prosody = voiceDoc.Descendants("prosody").FirstOrDefault();
                if (prosody?.Value != null)
                {
                    // Create the <mstts:express-as> element, for now skip the ':' letter and replace at the end

                    var expressedAsWrappedElement = new XElement("msttsexpress-as",
                        new XAttribute("style", preferredVoiceStyle));
                    expressedAsWrappedElement.Value = prosody!.Value;
                    prosody?.ReplaceWith(expressedAsWrappedElement);
                    speechXml = voiceDoc.ToString().Replace(@"msttsexpress-as", "mstts:express-as");
                }
            }

            return speechXml;
        }

        private List<string> ResolveAvailableActorVoiceIds(string? language, TextToSpeechLanguage[] actorVoices)
        {
            if (actorVoices?.Any() == true)
            {
                var voiceActorIds = actorVoices.Where(v => v.LanguageKey == language || v.LanguageKey?.Split("-")[0] == language).SelectMany(v => v.VoiceActors).Select(v => v.VoiceId).ToList();
                return voiceActorIds;
            }
            return new List<string>();
        }

        private string ResolveVoiceActorId(string? language, string? preferredVoiceActorId, TextToSpeechLanguage[] actorVoices)
        {
            string actorVoiceId = "(en-AU, NatashaNeural)"; //default to a select voice actor id 
            if (actorVoices?.Any() == true)
            {
                var voiceActorsForLanguage = actorVoices.Where(v => v.LanguageKey == language || v.LanguageKey?.Split("-")[0] == language).SelectMany(v => v.VoiceActors).Select(v => v.VoiceId).ToList();
                if (voiceActorsForLanguage != null)
                {
                    if (voiceActorsForLanguage.Any() == true)
                    {
                        var resolvedPreferredVoiceActorId = voiceActorsForLanguage.FirstOrDefault(v => v == preferredVoiceActorId);
                        if (!string.IsNullOrWhiteSpace(resolvedPreferredVoiceActorId))
                        {
                            return resolvedPreferredVoiceActorId!;
                        }
                        actorVoiceId = voiceActorsForLanguage.First();
                    }
                }
            }
            return actorVoiceId;
        }

        private async Task<string?> GetIssuedToken()
        {
            var httpClient = new HttpClient();
            string? textToSpeechSubscriptionKey = Environment.GetEnvironmentVariable("AZURE_TEXT_SPEECH_SUBSCRIPTION_KEY", EnvironmentVariableTarget.Machine);
            httpClient.DefaultRequestHeaders.Add(OcpApiSubscriptionKeyHeaderName, textToSpeechSubscriptionKey);
            string? tokenEndpointUrl = _configuration[TextToSpeechIssueTokenEndpoint];
            if (tokenEndpointUrl == null)
            {
                return null;
            }
            var response = await httpClient.PostAsync(tokenEndpointUrl, new StringContent("{}"));
            _token = (await response.Content.ReadAsStringAsync()).ToSecureString();
            _lastTimeTokenFetched = DateTime.Now;
            return _token.ToNormalString();
        }

        public async Task<List<string>> GetVoiceStyles()
        {
            var voiceStyles = new List<string>
            {
                "normal-neutral",
                "advertisement_upbeat",
                "affectionate",
                "angry",
                "assistant",
                "calm",
                "chat",
                "cheerful",
                "customerservice",
                "depressed",
                "disgruntled",
                "documentary-narration",
                "embarrassed",
                "empathetic",
                "envious",
                "excited",
                "fearful",
                "friendly",
                "gentle",
                "hopeful",
                "lyrical",
                "narration-professional",
                "narration-relaxed",
                "newscast",
                "newscast-casual",
                "newscast-formal",
                "poetry-reading",
                "sad",
                "serious",
                "shouting",
                "sports_commentary",
                "sports_commentary_excited",
                "whispering",
                "terrified",
                "unfriendly"
            };
            return await Task.FromResult(voiceStyles);
        }

        private const string OcpApiSubscriptionKeyHeaderName = "Ocp-Apim-Subscription-Key";
        private const string TextToSpeechIssueTokenEndpoint = "TextToSpeechIssueTokenEndpoint";
        private const string TextToSpeechSpeechEndpoint = "TextToSpeechSpeechEndpoint";        
        private const string TextToSpeechSpeechContentType = "TextToSpeechSpeechContentType";
        private const string TextToSpeechSpeechUserAgent = "TextToSpeechSpeechUserAgent";
        private const string TextToSpeechSpeechXMicrosoftOutputFormat = "TextToSpeechSpeechXMicrosoftOutputFormat";

        private readonly IConfiguration _configuration;

        private DateTime? _lastTimeTokenFetched = null;
        private SecureString? _token = null;

    }
}
