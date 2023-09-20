using Azure.AI.TextAnalytics;
using FluentAssertions;

namespace MultiLingual.Translator.Lib.Test
{
    public class DetectLanguageUtilTests
    {

        private DetectLanguageUtil _detectLanguageUtil;

        public DetectLanguageUtilTests()
        {
            _detectLanguageUtil = new DetectLanguageUtil();
        }

        [Theory]
        [InlineData("Donde esta la playa", "es", "Spanish")]
        [InlineData("Jeg er fra Trøndelag og jeg liker brunost", "no", "Norwegian")]
        public async Task DetectLanguageDetailsSucceeds(string text, string expectedLanguageIso6391, string expectedLanguageName)
        {
            string? detectedLangIso6391 = await _detectLanguageUtil.DetectLanguageIso6391(text);
            detectedLangIso6391.Should().Be(expectedLanguageIso6391);
            string? detectedLangName = await _detectLanguageUtil.DetectLanguageName(text);
            detectedLangName.Should().Be(expectedLanguageName);
        }

        [Theory]
        [InlineData("Du hast mich", "de", "German")]
        public async Task DetectLanguageSucceeds(string text, string expectedLanguageIso6391, string expectedLanguageName)
        {
            DetectedLanguage detectedLanguage = await _detectLanguageUtil.DetectLanguage(text);
            detectedLanguage.Iso6391Name.Should().Be(expectedLanguageIso6391);            
            detectedLanguage.Name.Should().Be(expectedLanguageName);
        }

    }
}