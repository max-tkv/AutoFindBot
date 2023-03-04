using System.Diagnostics.CodeAnalysis;

namespace AutoFindBot.Helpers;

/// <summary>
/// Helper to guard from invalid arguments
/// </summary>
public static class Guard
{
    /// <summary>
    /// Raises GuardArgumentException on failed condition
    /// </summary>
    /// <param name="condition">Condition to check</param>
    /// <param name="message">Error message</param>
    public static void IsTrue(bool condition, string message)
    {
        if (condition == false)
            throw new GuardArgumentException(message);
    }

    /// <summary>
    /// Raises GuardArgumentException when value is null
    /// </summary>
    /// <param name="value">Object value</param>
    /// <param name="message">Error message</param>
    public static void NotNull([NotNull] object? value, string message)
    {
        if (value == null)
            throw new GuardArgumentException(message);
    }

    /// <summary>
    /// Raises GuardArgumentException when string value is null or empty
    /// </summary>
    /// <param name="value">String value</param>
    /// <param name="message">Error message</param>
    public static void NotNull([NotNull] string? value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new GuardArgumentException(message);
    }

    /// <summary>
    /// Raises GuardArgumentException when string value is null or empty
    /// </summary>
    /// <param name="value">String value</param>
    /// <param name="message">Error message</param>
    public static void NotEmpty(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new GuardArgumentException(message);
    }

    /// <summary>
    /// Raises GuardArgumentException when numeric value equal to 0
    /// </summary>
    /// <param name="value">Numeric value</param>
    /// <param name="message">Error message</param>
    public static void NotEmpty(long value, string message)
    {
        if (value == 0)
            throw new GuardArgumentException(message);
    }
}

/// <summary>
/// Typed exception for the Guard-helper
/// </summary>
public class GuardArgumentException : Exception
{
    public GuardArgumentException(string message) : base(message)
    {
    }
}