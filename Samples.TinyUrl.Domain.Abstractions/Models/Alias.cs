using Samples.TinyUrl.Common.Interfaces;

namespace Samples.TinyUrl.Domain.Abstractions.Models;

public class Alias : IHasStringId
{
    public required string Id { get; set; }
    public string? RootId { get; set; }
    public required string OriginalValue { get; set; }
}
