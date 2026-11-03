using LanguageLearner.Controllers;
using LanguageLearner.Models;
using LanguageLearner.Views;

namespace LanguageLearner;

/// <summary>
/// Provides the application entry point and initializes the main form.
/// </summary>
internal static class Program
{
    [STAThread]
    /// <summary>
    /// Configures and runs the Windows Forms language learner application.
    /// </summary>
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        var view = new MainForm();
        var fileManager = new FileManager();
        _ = new LanguageLearnerController(view, fileManager);

        Application.Run(view);
    }
}
