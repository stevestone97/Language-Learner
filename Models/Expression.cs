namespace LanguageLearner.Models;

// One line of the vocabulary file: source;translation;description;example.
/// <summary>
/// Represents a vocabulary entry with its source expression, translation, description, and example.
/// </summary>
public class Expression
{
    public string SourceExpression { get; }
    public string Translation { get; }
    public string Description { get; }
    public string Examples { get; }

    /// <summary>
    /// Creates a vocabulary entry from its four text fields.
    /// </summary>
    /// <param name="sourceExpression">The expression presented to the learner.</param>
    /// <param name="translation">The translation of the source expression.</param>
    /// <param name="description">A description of the expression.</param>
    /// <param name="examples">Example usage of the expression.</param>
    public Expression(string sourceExpression, string translation,
                      string description, string examples)
    {
        SourceExpression = sourceExpression;
        Translation = translation;
        Description = description;
        Examples = examples;
    }
}
