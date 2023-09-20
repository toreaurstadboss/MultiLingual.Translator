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
        [InlineData("Jeg er fra Norge og jeg liker brunost", "i'm from norway and i like brown cheese")]
        public async Task TranslationReturnsExpected(string input, string expectedTranslation)
        {
            string? translation = await _translateUtil.Translate(LanguageCode.English, input, LanguageCode.Norwegian);
            translation.Should().NotBeNull();
            translation.Should().BeEquivalentTo(expectedTranslation);
        }

    }
}
