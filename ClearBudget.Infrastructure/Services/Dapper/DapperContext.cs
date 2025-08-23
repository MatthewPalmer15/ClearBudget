using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ClearBudget.Infrastructure.Services.Dapper;

public interface IDapperContext
{
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null);
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null);
    Task<T?> QueryFirstAsync<T>(string sql, object? parameters = null);
    Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null);
    Task<T?> QuerySingleAsync<T>(string sql, object? parameters = null);
    Task ExecuteAsync(string sql, object? parameters = null);
    Task<T?> ExecuteAsync<T>(string sql, object? parameters = null);
}

internal class DapperContext(IConfiguration configuration) : IDapperContext
{
    private readonly string _connectionString = configuration.GetConnectionString("Default") ?? throw new NullReferenceException("No connection string defined");

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<T>(sql, parameters);
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync(sql, parameters);
    }

    public async Task<T?> QueryFirstAsync<T>(string sql, object? parameters = null)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstAsync<T>(sql, parameters);
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<T>(sql, parameters);
    }

    public async Task<T?> QuerySingleAsync<T>(string sql, object? parameters = null)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleAsync<T>(sql, parameters);
    }

    public async Task ExecuteAsync(string sql, object? parameters = null)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(sql, parameters);
    }

    public async Task<T?> ExecuteAsync<T>(string sql, object? parameters = null)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<T>(sql, parameters);
    }
}