using System.Text;

namespace LanguageLearner.Models;

/// <summary>
/// Loads and saves vocabulary entries in the application's text-file format.
/// </summary>
public class FileManager
{
    private List<Expression> _expressions = new();

    // The dynamic list held in memory after loading.
    public IReadOnlyList<Expression> Expressions => _expressions;

    /// <summary>
    /// Asynchronously loads and validates vocabulary entries from a file.
    /// </summary>
    /// <param name="filename">The path of the vocabulary file to load.</param>
    /// <returns>The entries loaded from the file.</returns>
    /// <exception cref="FileNotFoundException">The specified file does not exist.</exception>
    /// <exception cref="FormatException">A non-empty line does not contain a valid vocabulary entry.</exception>
    /// <exception cref="IOException">An I/O error occurs while reading the file.</exception>
    public async Task<IReadOnlyList<Expression>> LoadDataFile(string filename)
    {
        if (!File.Exists(filename))
        {
            throw new FileNotFoundException($"File not found: {filename}", filename);
        }

        var loadedExpressions = new List<Expression>();

        // StreamReader.ReadLineAsync reads the file without blocking the UI.
        //Provide access to file bytes
        await using var stream = new FileStream(
            filename, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true);

        // Reads those bytes as text
        using var reader = new StreamReader(stream, Encoding.UTF8);

        string? line;
        int lineNumber = 0;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            string[] parts = line.Split(';');
            if (parts.Length != 4 ||
                string.IsNullOrWhiteSpace(parts[0]) ||
                string.IsNullOrWhiteSpace(parts[1]))
            {
                throw new FormatException(
                    $"Invalid vocabulary entry on line {lineNumber}. " +
                    "Expected source;translation;description;examples.");
            }

            loadedExpressions.Add(new Expression(
                parts[0].Trim(),
                parts[1].Trim(),
                parts[2].Trim(),
                parts[3].Trim()));
        }

        // Replace the existing list only after the whole file has loaded correctly.
        _expressions = loadedExpressions;
        return _expressions;
    }

    /// <summary>
    /// Asynchronously writes the current vocabulary entries to a file.
    /// </summary>
    /// <param name="filename">The path of the file to create or overwrite.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    /// <exception cref="IOException">An I/O error occurs while writing the file.</exception>
    public async Task SaveDataFile(string filename)
    {
        await using var stream = new FileStream(
            filename, FileMode.Create, FileAccess.Write, FileShare.None,
            bufferSize: 4096, useAsync: true);
 
        await using var writer = new StreamWriter(stream, new UTF8Encoding(false));

        foreach (Expression expression in _expressions)
        {
            string line = string.Join(";",
                expression.SourceExpression,
                expression.Translation,
                expression.Description,
                expression.Examples);

            await writer.WriteLineAsync(line);
        }
    }
}
