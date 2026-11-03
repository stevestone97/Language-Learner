using LanguageLearner.Models;
using LanguageLearner.Views;

namespace LanguageLearner.Controllers;

/// <summary>
/// Coordinates vocabulary data and user actions between the view and the model.
/// </summary>
public class LanguageLearnerController
{
    private ILanguageLearnerView _view;
    private FileManager _fileManager;

    /// <summary>
    /// Creates a controller and subscribes to the view's user-action events.
    /// </summary>
    /// <param name="view">The view used to receive user actions and display results.</param>
    /// <param name="fileManager">The service used to load and access vocabulary entries.</param>
    public LanguageLearnerController(ILanguageLearnerView view, FileManager fileManager)
    {
        _view = view;
        _fileManager = fileManager;

        _view.LoadRequested += OnLoadRequested;
        _view.ExpressionSelected += OnExpressionSelected;
        _view.CheckRequested += OnCheckRequested;
        _view.ShowAnswerRequested += OnShowAnswerRequested;
    }

    // An async void method is appropriate for a UI event handler.
    // The actual file operation returns a Task and is awaited here.
    /// <summary>
    /// Loads the vocabulary file and displays its source expressions in the view.
    /// </summary>
    /// <param name="sender">The object that raised the load request.</param>
    /// <param name="e">The event data associated with the load request.</param>
    private async void OnLoadRequested(object? sender, EventArgs e)
    {
        _view.SetBusy(true);
        try
        {
            string filename = Path.Combine(
                AppContext.BaseDirectory, "VocabularyEngSwe.txt");

            IReadOnlyList<Expression> expressions =
                await _fileManager.LoadDataFile(filename);

            _view.DisplayExpressions(
                expressions.Select(expression => expression.SourceExpression).ToList());

            if (expressions.Count == 0)
            {
                _view.ShowFeedback("The vocabulary file is empty.");
            }
        }
        catch (IOException ex)
        {
            _view.ShowError(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _view.ShowError(ex.Message);
        }
        catch (FormatException ex)
        {
            _view.ShowError(ex.Message);
        }
        finally
        {
            _view.SetBusy(false);
        }
    }

    /// <summary>
    /// Displays details for the expression currently selected in the view.
    /// </summary>
    /// <param name="sender">The object that raised the selection event.</param>
    /// <param name="e">The event data associated with the selection.</param>
    private void OnExpressionSelected(object? sender, EventArgs e)
    {
        Expression? selected = GetSelectedExpression();
        if (selected == null)
        {
            return;
        }

        _view.DisplaySelectedExpression(
            selected.SourceExpression, selected.Description, selected.Examples);
    }

    /// <summary>
    /// Checks the user's answer for the currently selected expression and reports the result.
    /// </summary>
    /// <param name="sender">The object that raised the check request.</param>
    /// <param name="e">The event data associated with the check request.</param>
    private void OnCheckRequested(object? sender, EventArgs e)
    {
        Expression? selected = GetSelectedExpression();
        if (selected == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_view.UserAnswer))
        {
            _view.ShowFeedback("Enter a Swedish translation first.");
            return;
        }

        bool correct = AnswerChecker.IsCorrect(
            _view.UserAnswer, selected.Translation);

        _view.ShowFeedback(
            correct ? "Correct! Well done." : "Not quite. Try again.", correct);
    }

    /// <summary>
    /// Displays the translation for the currently selected expression.
    /// </summary>
    /// <param name="sender">The object that raised the show-answer request.</param>
    /// <param name="e">The event data associated with the request.</param>
    private void OnShowAnswerRequested(object? sender, EventArgs e)
    {
        Expression? selected = GetSelectedExpression();
        if (selected != null)
        {
            _view.ShowFeedback($"Answer: {selected.Translation}");
        }
    }

    /// <summary>
    /// Gets the expression corresponding to the view's selected index.
    /// </summary>
    /// <returns>The selected expression, or <see langword="null"/> if the index is invalid.</returns>
    private Expression? GetSelectedExpression()
    {
        int index = _view.SelectedExpressionIndex;
        IReadOnlyList<Expression> expressions = _fileManager.Expressions;

        return index >= 0 && index < expressions.Count
            ? expressions[index]
            : null;
    }
}
