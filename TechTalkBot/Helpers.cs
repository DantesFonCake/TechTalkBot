using CommandLine;

namespace TechTalkBot;

public static class Helpers
{
    public static async Task<ParserResult<object>> WithParsedAsync<T>(this Task<ParserResult<object>> result,
        Func<T, Task> action)
    {
        var awaited = await result;
        return await awaited.WithParsedAsync(action);
    }

    public static async Task<ParserResult<T>> WithNotParsedAsync<T>(this Task<ParserResult<T>> result,
        Func<IEnumerable<Error>, Task> action)
    {
        var awaited = await result;
        return await awaited.WithNotParsedAsync(action);
    }
}