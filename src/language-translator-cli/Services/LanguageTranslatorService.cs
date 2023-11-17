using System.Dynamic;
using Amazon.Translate;
using Amazon.Translate.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace language_translator_cli.Services;

public sealed class LanguageTranslatorService(
        IAmazonTranslate translateClient,
        ILogger<ILanguageTranslatorService> logger
    )
    : ILanguageTranslatorService
{
    private readonly IAmazonTranslate _translateClient =
        translateClient ?? throw new ArgumentNullException(nameof(translateClient));

    private readonly ILogger<ILanguageTranslatorService> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    private async Task<bool> IsFileExist(string filePath)
    {
        using StreamReader streamReader = new(filePath);
        try
        {
            await streamReader.ReadToEndAsync();
        }
        catch (FileNotFoundException)
        {
            _logger.LogWarning("file not found");
            return false;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message + e.StackTrace);
            return false;
        }

        return true;
    }

    private async Task<string> TranslateTextAsync(string text, string sourceLanguageCode, string targetLanguageCode)
    {
        var translateRequest = new TranslateTextRequest { Text = text, SourceLanguageCode = sourceLanguageCode, TargetLanguageCode = targetLanguageCode };

        var translateResponse = await _translateClient.TranslateTextAsync(translateRequest);
        var translatedText = translateResponse.TranslatedText;
        return translatedText;
    }


    public async Task<string> GenerateLanguageFileAsync()
    {
        const string sourceLanguageCode = "en";
        const string filepath = "Assets/en.json";
        const string directoryPrefixToGenerateFilesIn = "Assets/Generated";
        using StreamReader streamReader = new(filepath);
        string json = await streamReader.ReadToEndAsync();
#pragma warning disable CS8600
        dynamic anonObject = JsonConvert.DeserializeObject<ExpandoObject>(json, new ExpandoObjectConverter());
#pragma warning restore CS8600
        if (anonObject == null)
        {
            throw new Exception();
        }

        // See https://en.wikipedia.org/wiki/Languages_used_on_the_Internet
        List<string> ALLOW_LIST = new List<string>()
        {
            "zh",
            "zh-TW",
            "es",
            // "es-MX",
            "ja",
            "pt",
            //  "pt-PT",
            "de",
            "ar",
            "fr",
            // "fr-CA",
            "ru",
            "ko",
            "hi",
            "ms",
            "tr",
            "fa",
            "vi",
            "it"
        };


        Directory.CreateDirectory(directoryPrefixToGenerateFilesIn);

        foreach (var language in Constants.LanguageCode)
        {
            if (!ALLOW_LIST.Contains(language))
            {
                continue;
            }

            var languageFilePathToGenerate = $"{directoryPrefixToGenerateFilesIn}/{language}.json";
            await using StreamWriter streamWriter = File.CreateText(languageFilePathToGenerate);
            dynamic languageFileExpandoObject = new ExpandoObject();

            foreach (var property in (IDictionary<String, Object>)anonObject)
            {
                var textToTranslate = (string)property.Value;
                var newTextToTranslate = textToTranslate.Replace("{", "<span translate=no>");
                var textext = newTextToTranslate.Replace("}", "</span>");
                var translatedText = await TranslateTextAsync(textext, sourceLanguageCode, language);

                var newTranslatedText = translatedText.Replace("<span translate=no>", " {").Replace("</span>", "}");

                ((IDictionary<String, Object>)languageFileExpandoObject).Add(property.Key, newTranslatedText);
            }

            var objectToWrite = JsonConvert.SerializeObject(languageFileExpandoObject);
            await streamWriter.WriteAsync(objectToWrite);
        }

        _logger.LogInformation("Generated react-intl language files in {Path}", "/src/language-translator-cli/bin/Debug/net8.0/Assets/Generated");
        return "hello";
    }
}
