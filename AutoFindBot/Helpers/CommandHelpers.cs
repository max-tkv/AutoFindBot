namespace AutoFindBot.Helpers;

public static class CommandHelpers
{
    public static string GetErrorMessage(string commandName, string? updateMessageTest = null)
    {
        return commandName switch
        {
            _ => throw new ArgumentOutOfRangeException(nameof(commandName), commandName, null)
        };
    }
}