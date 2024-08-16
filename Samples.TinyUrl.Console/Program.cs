using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Samples.TinyUrl.Common.Exceptions;
using Samples.TinyUrl.Console;
using Samples.TinyUrl.Domain.Abstractions.Services;
using Samples.TinyUrl.Domain.Core;
using Samples.TinyUrl.Domain.Memory;

var builder = Host.CreateApplicationBuilder(args);

register(builder.Services);

using var host = builder.Build();

Console.WriteLine(@"
Press CTRL+C to terminate when complete

Enter any of the following commands when prompted:
    
    create <long url> [custom short url]
        * This will create and return a short url alias to the long url entered
        * If the custom short url value is specified, that will be used (if available) as the short url alias

    delete <tiny url>
        * This will delete the map of the tiny url specified

    get <tiny url>
        * This will return the long url associated with the tiny url

    stats <tiny url>
        * This will return the stats related to the tiny url specified

Enjoy!
");

await repl(host.Services);

return;

void register(IServiceCollection serviceCollection)
{
    // In memory data access
    serviceCollection.AddInMemoryDomainDataAccess();

    // In memory simple sequential id generator
    serviceCollection.AddInMemorySequenceGenerator()
                     //.AddInMemorySequentialTinyIdGenerator();
                     .AddInMemoryBaseConversionTinyIdGenerator();

    // URL Retrieval service
    serviceCollection.AddInMemoryTrackedRetrievalService();

    // Default simply tinyurl service
    serviceCollection.AddDomainCoreTinyUrlService();
}

async Task repl(IServiceProvider serviceProvider)
{
    var tinyUrlSerivce = serviceProvider.GetRequiredService<ITinyUrlService>();

    do
    {
        Console.WriteLine("Please enter a command and arguments, press enter to execute.");

        var line = Console.ReadLine();

        var replCommand = ReplCommand.TryParse(line);

        if (replCommand == null)
        {
            Console.WriteLine("Invalid input, please try again");

            continue;
        }

        switch (replCommand.Operation)
        {
            case TinyUrlOperation.Get:
                var longUrl = await tinyUrlSerivce.GetLongUrlAsync(replCommand.TinyUrl!);

                Console.WriteLine(longUrl == null
                                      ? "Nothing mapped to that tinyurl"
                                      : string.Concat("LongUrl of [", longUrl, "] returned for TinyUrl of [", replCommand.TinyUrl, "]"));

                break;

            case TinyUrlOperation.Create:

                try
                {
                    var tinyUrl = await tinyUrlSerivce.CreateTinyUrlAsync(replCommand.LongUrl!,
                                                                          atTinyUrl: replCommand.TinyUrl);

                    Console.WriteLine(string.Concat("TinyUrl Created: [", tinyUrl, "]"));
                }
                catch(TinyUrlDuplicateKeyException) when(replCommand.TinyUrl != null)
                {
                    Console.WriteLine(string.Concat("The TinyUrl of [", replCommand.TinyUrl, "] is already in use"));
                }

                break;

            case TinyUrlOperation.Delete:
                var deleted = await tinyUrlSerivce.DeleteAsync(replCommand.TinyUrl!);

                Console.WriteLine(string.Concat("TinyUrl of [", replCommand.TinyUrl, "] ",
                                                deleted
                                                    ? "deleted successfully"
                                                    : "does not exist"));

                break;

            case TinyUrlOperation.Stats:
                var statService = serviceProvider.GetRequiredService<ITinyUrlRetrievalStatsService>();

                var stats = await statService.GetRetrievalCountAsync(replCommand.TinyUrl!);

                Console.WriteLine(string.Concat("TinyUrl of [", replCommand.TinyUrl, "] has been retrieved [", stats, "] times"));

                break;
        }
    } while (true);

    // ReSharper disable once FunctionNeverReturns
}
