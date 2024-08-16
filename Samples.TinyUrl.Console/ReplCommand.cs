namespace Samples.TinyUrl.Console;

internal class ReplCommand
{
    public TinyUrlOperation Operation { get; set; }
    public string? TinyUrl { get; set; }
    public string? LongUrl { get; set; }

    public static ReplCommand? TryParse(string? replLine)
    {
        if (string.IsNullOrEmpty(replLine))
        {
            return null;
        }

        var splits = replLine.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (splits.Length < 2)
        {
            return null;
        }

        var operation = splits[0].ToUpperInvariant() switch
                        {
                            "CREATE" => TinyUrlOperation.Create,
                            "DELETE" => TinyUrlOperation.Delete,
                            "GET" => TinyUrlOperation.Get,
                            "STATS" => TinyUrlOperation.Stats,
                            _ => TinyUrlOperation.Unknown,
                        };

        if (operation == TinyUrlOperation.Unknown)
        {
            return null;
        }

        var replCommand = new ReplCommand
                          {
                              Operation = operation
                          };

        if (operation == TinyUrlOperation.Create)
        {
            replCommand.LongUrl = splits[1];

            replCommand.TinyUrl = splits.Length > 2
                                      ? splits[2]
                                      : null;
        }
        else
        {
            replCommand.TinyUrl = splits[1];
        }

        return replCommand;
    }
}
