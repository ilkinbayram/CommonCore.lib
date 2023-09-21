using System.Collections.Generic;

namespace CommonCore.Utilities.Results
{
    public interface IResult
    {
        bool Success { get; }
        bool IsProcessBroken { get; }
        List<Response> Responses { get; }
    }
}
