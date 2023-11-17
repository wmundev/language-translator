using language_translator_cli.Services;

namespace language_translator_cli;

public sealed class App(ILanguageTranslatorService languageTranslatorService)
{
    private readonly ILanguageTranslatorService _languageTranslatorService = languageTranslatorService ?? throw new ArgumentNullException(nameof(languageTranslatorService));

    public async Task Run(string[] args)
    {
        await _languageTranslatorService.GenerateLanguageFileAsync();
    }
}
