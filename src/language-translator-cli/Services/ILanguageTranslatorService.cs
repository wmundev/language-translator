namespace language_translator_cli.Services;

public interface ILanguageTranslatorService
{
    Task<string> GenerateLanguageFileAsync();
}
