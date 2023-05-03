using Ardalis.GuardClauses;
using AutoFindBot.Storage.PostgreSql.Invariants;
using Microsoft.Extensions.Options;

namespace AutoFindBot.Storage.PostgreSql.Options;

public static class PostgreSqlOptionsValidator
{
    public static void Validate(this PostgreSqlOptions options)
    {
        Guard.Against.Null(
            options,
            $"Конфигурация {RegisterOptionsPathInvariants.PostgeSQLOptionsPath} должена иметь значение");

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            throw new OptionsValidationException(
                nameof(PostgreSqlOptions), 
                typeof(PostgreSqlOptions),
                new[] { $"{RegisterOptionsPathInvariants.PostgeSQLOptionsPath} обязан иметь значение" });
        }
    }
}