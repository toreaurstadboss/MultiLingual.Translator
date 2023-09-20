using FluentAssertions;
using MultiLingual.Translator.Lib.Models;

namespace MultiLingual.Translator.Lib.Test
{

    public class TranslateUtilTests
    {

        private TranslateUtil _translateUtil;

        public TranslateUtilTests()
        {
            _translateUtil = new TranslateUtil();                
        }

        [Theory]
        [InlineData("Jeg er fra Norge og jeg liker brunost", "i'm from norway and i like brown cheese", LanguageCode.Norwegian,  LanguageCode.English)]
        [InlineData("Jeg er fra Norge og jeg liker brunost", "i'm from norway and i like brown cheese", null, LanguageCode.English)] //auto detect language is tested here
        [InlineData("Ich bin aus Hamburg und ich liebe bier", "i'm from hamburg and i love beer", LanguageCode.German, LanguageCode.English)]
        [InlineData("Ich bin aus Hamburg und ich liebe bier", "i'm from hamburg and i love beer", null, LanguageCode.English)] //Auto detect source language is tested here
        [InlineData("tlhIngan maH", "we are klingons", LanguageCode.Klingon, LanguageCode.English)] //Klingon force !
        public async Task TranslationReturnsExpected(string input, string expectedTranslation, string sourceLanguage, string targetLanguage)
        {
            string? translation = await _translateUtil.Translate(targetLanguage, input, sourceLanguage);
            translation.Should().NotBeNull();
            translation.Should().BeEquivalentTo(expectedTranslation);
        }

    }
}
