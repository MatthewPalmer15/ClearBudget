// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ClearBudget.Infrastructure.Extensions;
using CsvHelper;
using System.Globalization;
using System.Text;

namespace ClearBudget.Infrastructure.Services.Csv;

public interface ICsvService
{
    /// <summary>
    ///     Reads data from a CSV file and converts it to a list of objects of type T.
    /// </summary>
    /// <typeparam name="T">The type of the objects to create from the CSV data. This type must be a class.</typeparam>
    /// <param name="filePath">The path to the CSV file to read.</param>
    /// <returns>A list of objects of type T created from the CSV data.</returns>
    /// <remarks>
    ///     This method uses the CsvReader class from the CsvHelper library to read the CSV data.
    ///     The CSV data is assumed to be in the UTF-8 encoding and to use the invariant culture.
    /// </remarks>
    public List<T> Read<T>(string filePath) where T : class;

    /// <summary>
    ///     Reads data from a CSV file and converts it to a list of objects of type T.
    /// </summary>
    /// <typeparam name="T">The type of the objects to create from the CSV data. This type must be a class.</typeparam>
    /// <param name="filePath">The path to the CSV file to read.</param>
    /// <returns>A list of objects of type T created from the CSV data.</returns>
    /// <remarks>
    ///     This method uses the CsvReader class from the CsvHelper library to read the CSV data.
    ///     The CSV data is assumed to be in the UTF-8 encoding and to use the invariant culture.
    /// </remarks>
    public Task<List<T>> ReadAsync<T>(string filePath) where T : class;

    /// <summary>
    ///     Writes a collection of items to a CSV file.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the collection. This type must be a class.</typeparam>
    /// <param name="filePath">The path to the CSV file to write to.</param>
    /// <param name="items">The collection of items to write to the CSV file.</param>
    /// <param name="includeHeader">A flag indicating whether to include a header row in the CSV file. The default is true.</param>
    /// <remarks>
    ///     This method uses the CsvWriter class from the CsvHelper library to write the CSV data.
    ///     The CSV data is written in the UTF-8 encoding and uses the invariant culture.
    ///     If includeHeader is true, a header row is written to the CSV file before the items.
    /// </remarks>
    public void Write<T>(string filePath, IEnumerable<T> items, bool includeHeader = true)
        where T : class;

    /// <summary>
    ///     Writes a collection of items to a CSV file.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the collection. This type must be a class.</typeparam>
    /// <param name="filePath">The path to the CSV file to write to.</param>
    /// <param name="items">The collection of items to write to the CSV file.</param>
    /// <param name="includeHeader">A flag indicating whether to include a header row in the CSV file. The default is true.</param>
    /// <remarks>
    ///     This method uses the CsvWriter class from the CsvHelper library to write the CSV data.
    ///     The CSV data is written in the UTF-8 encoding and uses the invariant culture.
    ///     If includeHeader is true, a header row is written to the CSV file before the items.
    /// </remarks>
    public Task WriteAsync<T>(string filePath, IEnumerable<T> items, bool includeHeader = true)
        where T : class;
}

public class CsvService : ICsvService
{
    /// <inheritdoc />
    public List<T> Read<T>(string filePath) where T : class
    {
        using StreamReader? reader = new(filePath, Encoding.UTF8);
        using CsvReader? csv = new(reader, CultureInfo.InvariantCulture);
        return csv.GetRecords<T>().ToList();
    }

    /// <inheritdoc />
    public async Task<List<T>> ReadAsync<T>(string filePath) where T : class
    {
        using StreamReader? reader = new(filePath, Encoding.UTF8);
        using CsvReader? csv = new(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecordsAsync<T>();
        return await records.ToListAsync();
    }

    /// <inheritdoc />
    public void Write<T>(string filePath, IEnumerable<T> items, bool includeHeader = true)
        where T : class
    {
        using StreamWriter? writer = new(File.Create(filePath), Encoding.UTF8);
        using CsvWriter? csv = new(writer, CultureInfo.InvariantCulture);

        if (includeHeader)
        {
            csv.WriteHeader<T>();
            csv.NextRecord();
        }

        csv.WriteRecords(items);
    }

    /// <inheritdoc />
    public async Task WriteAsync<T>(string filePath, IEnumerable<T> items, bool includeHeader = true)
        where T : class
    {
        await using StreamWriter? writer = new(File.Create(filePath), Encoding.UTF8);
        await using CsvWriter? csv = new(writer, CultureInfo.InvariantCulture);

        if (includeHeader)
        {
            csv.WriteHeader<T>();
            await csv.NextRecordAsync();
        }

        await csv.WriteRecordsAsync(items);
    }
}