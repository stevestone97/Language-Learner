namespace LanguageLearner.Models;

/// <summary>
/// Provides methods for validating vocabulary answers.
/// </summary>
public class AnswerChecker
{
    // Case-insensitive comparison is mandatory for Grade D.
    /// <summary>
    /// Determines whether the user's answer matches the expected translation, ignoring
    /// surrounding whitespace and letter casing.
    /// </summary>
    /// <param name="userAnswer">The answer entered by the user.</param>
    /// <param name="translation">The expected translation.</param>
    /// <returns><see langword="true"/> if the answers match; otherwise, <see langword="false"/>.</returns>
    public static bool IsCorrect(string userAnswer, string translation)
    {
        return string.Equals(
            userAnswer.Trim(),
            translation.Trim(),
            StringComparison.OrdinalIgnoreCase);
    }
}
