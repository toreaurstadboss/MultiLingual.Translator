using Microsoft.Extensions.Configuration;
using MultiLingual.Translator.Lib;
using Plugin.Maui.Audio;

namespace MultiLingual.Translator;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif

		builder.Services.AddSingleton(AudioManager.Current);
		builder.Services.AddTransient<MainPage>();

		builder.Services.AddScoped<IDetectLanguageUtil, DetectLanguageUtil>();
        builder.Services.AddScoped<ITranslateUtil, TranslateUtil>();
		builder.Services.AddScoped<ITextToSpeechUtil, TextToSpeechUtil>();

		var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
		builder.Configuration.AddConfiguration(config);

        return builder.Build();
	}
}
