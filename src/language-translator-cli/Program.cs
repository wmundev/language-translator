using Amazon.Translate;
using language_translator_cli;
using language_translator_cli.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = CreateHostBuilder(args).Build();

IHostBuilder CreateHostBuilder(string[] strings)
{
    return Host.CreateDefaultBuilder()
        .ConfigureServices((_, services) =>
        {
            services.AddAWSService<IAmazonTranslate>();
            services.AddSingleton<ILanguageTranslatorService, LanguageTranslatorService>();
            services.AddSingleton<App>();
        });
}


using var scope = host.Services.CreateScope();

var serviceProvider = scope.ServiceProvider;


try
{
    await serviceProvider.GetRequiredService<App>().Run(args);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}
