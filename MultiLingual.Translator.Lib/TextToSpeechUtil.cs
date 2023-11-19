using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using System.Runtime.InteropServices;
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

        public async Task<byte[]> GetSpeechFromText(string text, string language)
        {
            string xml = GetSpeechTextXml(text, language);
            string? token = await GetUpdatedToken();

            HttpClient httpClient = GetTextToSpeechWebClient(token);

            string ttsEndpointUrl = _configuration[TextToSpeechSpeechEndpoint];
            var response = await httpClient.PostAsync(ttsEndpointUrl, new StringContent(xml, Encoding.UTF8, _configuration[TextToSpeechSpeechContentType]));

            using (var memStream = new MemoryStream()) {
                var responseStream = await response.Content.ReadAsStreamAsync();
                responseStream.CopyTo(memStream);
                return memStream.ToArray();
            }
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

        private string GetSpeechTextXml(string text, string language)
        {
            string speechXml = $@"
            <speak version='1.0' xml:lang='en-US'>
                <voice xml:lang='en-US' xml:gender='Male' name='Microsoft Server Speech Text to Speech Voice (nb-NO, FinnNeural)'>
                    <prosody rate='1'>{text}</prosody>
                </voice>
            </speak>";
            return speechXml;               
        }

        private async Task<string> GetIssuedToken()
        {
            var httpClient = new HttpClient();
            string? textToSpeechSubscriptionKey = Environment.GetEnvironmentVariable("AZURE_TEXT_SPEECH_SUBSCRIPTION_KEY", EnvironmentVariableTarget.Machine);
            httpClient.DefaultRequestHeaders.Add(OcpApiSubscriptionKeyHeaderName, textToSpeechSubscriptionKey);
            string tokenEndpointUrl = _configuration[TextToSpeechIssueTokenEndpoint];
            var response = await httpClient.PostAsync(tokenEndpointUrl, new StringContent("{}"));
            _token = (await response.Content.ReadAsStringAsync()).ToSecureString();
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
