namespace LanguageLearner.Views;

// Only user actions and display-oriented values cross the View/Controller boundary.
// The View does not access Expression or FileManager directly.
/// <summary>
/// Defines the user actions and display operations exposed by the language learner view.
/// </summary>
public interface ILanguageLearnerView
{
    event EventHandler? LoadRequested;
    event EventHandler? ExpressionSelected;
    event EventHandler? CheckRequested;
    event EventHandler? ShowAnswerRequested;

    int SelectedExpressionIndex { get; }
    string UserAnswer { get; }

    /// <summary>
    /// Displays the source expressions available for selection.
    /// </summary>
    /// <param name="sourceExpressions">The source expressions to display.</param>
    void DisplayExpressions(IReadOnlyList<string> sourceExpressions);

    /// <summary>
    /// Displays the details of the selected expression.
    /// </summary>
    /// <param name="source">The source expression.</param>
    /// <param name="description">A description of the expression.</param>
    /// <param name="example">An example of the expression in use.</param>
    void DisplaySelectedExpression(string source, string description, string example);

    /// <summary>
    /// Displays feedback about an answer or other learner action.
    /// </summary>
    /// <param name="message">The feedback text to display.</param>
    /// <param name="correct">Whether the answer was correct, or <see langword="null"/> if not applicable.</param>
    void ShowFeedback(string message, bool? correct = null);

    /// <summary>
    /// Displays an error message to the user.
    /// </summary>
    /// <param name="message">The error text to display.</param>
    void ShowError(string message);

    /// <summary>
    /// Sets whether the view is busy processing an operation.
    /// </summary>
    /// <param name="busy"><see langword="true"/> to indicate an operation is in progress.</param>
    void SetBusy(bool busy);
}
