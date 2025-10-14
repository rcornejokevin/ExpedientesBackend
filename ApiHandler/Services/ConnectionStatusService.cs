using System.Data;
using Microsoft.EntityFrameworkCore;
using DBHandler.Context;

namespace ApiHandler.Services;

public record ConnectionStatus(string name, bool connected);

public static class ConnectionStatusService
{
    public static async Task<ConnectionStatus> CheckConnectionAsync(string name, DbContext context)
    {
        var connection = context.Database.GetDbConnection();
        var originalState = connection.State;
        try
        {
            if (originalState != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1 FROM DUAL";
            await command.ExecuteScalarAsync();

            return new ConnectionStatus(name, true);
        }
        catch
        {
            return new ConnectionStatus(name, false);
        }
        finally
        {
            if (originalState != ConnectionState.Open && connection.State == ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }
    }

    public static Task<ConnectionStatus[]> CheckAllAsync(DBHandlerContext primaryContext, LoginDbContext loginContext)
    {
        return Task.WhenAll(
            CheckConnectionAsync("OracleDB", primaryContext),
            CheckConnectionAsync("OracleDBUser", loginContext)
        );
    }
}
