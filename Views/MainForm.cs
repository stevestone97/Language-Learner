using System.Drawing;
using System.Windows.Forms;

namespace LanguageLearner.Views;

public sealed class MainForm : Form, ILanguageLearnerView
{
    private readonly ListBox _expressionList = new();
    private readonly TextBox _answerBox = new();
    private readonly Label _sourceLabel = new();
    private readonly Label _descriptionLabel = new();
    private readonly Label _exampleLabel = new();
    private readonly Label _feedbackLabel = new();
    private readonly Label _statusLabel = new();
    private readonly Button _checkButton = new();
    private readonly Button _showAnswerButton = new();

    public event EventHandler? LoadRequested;
    public event EventHandler? ExpressionSelected;
    public event EventHandler? CheckRequested;
    public event EventHandler? ShowAnswerRequested;

    public int SelectedExpressionIndex => _expressionList.SelectedIndex;
    public string UserAnswer => _answerBox.Text;

    public MainForm()
    {
        Text = "LanguageLearner - English to Swedish";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(970, 690);
        MinimumSize = new Size(820, 650);
        BackColor = Color.FromArgb(244, 247, 251);
        Font = new Font("Segoe UI", 10F);

        BuildInterface();

        _expressionList.SelectedIndexChanged += (_, _) =>
            ExpressionSelected?.Invoke(this, EventArgs.Empty);
        _checkButton.Click += (_, _) =>
            CheckRequested?.Invoke(this, EventArgs.Empty);
        _showAnswerButton.Click += (_, _) =>
            ShowAnswerRequested?.Invoke(this, EventArgs.Empty);
        _answerBox.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter && _checkButton.Enabled)
            {
                e.SuppressKeyPress = true;
                CheckRequested?.Invoke(this, EventArgs.Empty);
            }
        };

        SetBusy(true);
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        LoadRequested?.Invoke(this, EventArgs.Empty);
    }

    public void DisplayExpressions(IReadOnlyList<string> sourceExpressions)
    {
        _expressionList.BeginUpdate();
        try
        {
            _expressionList.Items.Clear();
            foreach (string sourceExpression in sourceExpressions)
            {
                _expressionList.Items.Add(sourceExpression);
            }
        }
        finally
        {
            _expressionList.EndUpdate();
        }

        _statusLabel.Text = $"{sourceExpressions.Count} expressions loaded";
        if (sourceExpressions.Count > 0)
        {
            _expressionList.SelectedIndex = 0;
        }
    }

    public void DisplaySelectedExpression(string source, string description,
                                          string example)
    {
        _sourceLabel.Text = source;
        _descriptionLabel.Text = description;
        _exampleLabel.Text = example;
        _answerBox.Clear();
        ShowFeedback(string.Empty);
    }

    public void ShowFeedback(string message, bool? correct = null)
    {
        _feedbackLabel.Text = message;
        _feedbackLabel.ForeColor = correct switch
        {
            true => Color.FromArgb(21, 128, 61),
            false => Color.FromArgb(185, 28, 28),
            _ => Color.FromArgb(55, 65, 81)
        };
    }

    public void ShowError(string message)
    {
        _statusLabel.Text = "Could not load the vocabulary file.";
        MessageBox.Show(this, message, "Vocabulary file error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public void SetBusy(bool busy)
    {
        bool hasSelection = _expressionList.SelectedIndex >= 0;
        _expressionList.Enabled = !busy;
        _answerBox.Enabled = !busy && hasSelection;
        _checkButton.Enabled = !busy && hasSelection;
        _showAnswerButton.Enabled = !busy && hasSelection;
    }

    private void BuildInterface()
    {
        var page = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(20),
        };
        page.RowStyles.Add(new RowStyle(SizeType.Absolute, 75));
        page.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        page.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        Controls.Add(page);

        var heading = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
        };
        heading.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        heading.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        heading.Controls.Add(new Label
        {
            Text = "LanguageLearner",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font("Segoe UI", 21F, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 58, 94),
        }, 0, 0);
        heading.Controls.Add(new Label
        {
            Text = "Practice English → Swedish vocabulary",
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(100, 116, 139),
        }, 0, 1);
        page.Controls.Add(heading, 0, 0);

        var columns = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
        };
        columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
        page.Controls.Add(columns, 0, 1);

        var left = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(16),
            Margin = new Padding(0, 0, 12, 0),
            ColumnCount = 1,
            RowCount = 2,
        };
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        left.Controls.Add(new Label
        {
            Text = "Expressions",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 58, 94),
        }, 0, 0);
        _expressionList.Dock = DockStyle.Fill;
        _expressionList.Font = new Font("Segoe UI", 11F);
        _expressionList.IntegralHeight = false;
        _expressionList.BorderStyle = BorderStyle.FixedSingle;
        left.Controls.Add(_expressionList, 0, 1);
        columns.Controls.Add(left, 0, 0);

        var right = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(18),
            Margin = new Padding(0),
            ColumnCount = 1,
            RowCount = 6,
        };
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 43));
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 75));
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 93));
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 93));
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        columns.Controls.Add(right, 1, 0);

        right.Controls.Add(new Label
        {
            Text = "Vocabulary quiz",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 58, 94)
        }, 0, 0);

        _sourceLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        _sourceLabel.ForeColor = Color.FromArgb(30, 58, 94);
        right.Controls.Add(CreateTextSection("Selected expression", _sourceLabel), 0, 1);

        right.Controls.Add(CreateTextSection("Description", _descriptionLabel), 0, 2);
        right.Controls.Add(CreateTextSection("Example sentence", _exampleLabel), 0, 3);

        var quizPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 4,
            Margin = new Padding(0, 6, 0, 0),
        };
        quizPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        quizPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        quizPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 27));
        quizPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 37));
        quizPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 47));
        quizPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        right.Controls.Add(quizPanel, 0, 4);

        var answerPrompt = new Label
        {
            Text = "Your Swedish translation",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(51, 65, 85)
        };
        quizPanel.Controls.Add(answerPrompt, 0, 0);
        quizPanel.SetColumnSpan(answerPrompt, 2);

        _answerBox.Dock = DockStyle.Fill;
        _answerBox.Font = new Font("Segoe UI", 11F);
        _answerBox.Margin = new Padding(0, 0, 0, 8);
        quizPanel.Controls.Add(_answerBox, 0, 1);
        quizPanel.SetColumnSpan(_answerBox, 2);

        ConfigureButton(_checkButton, "Check answer", true);
        _checkButton.Margin = new Padding(0, 1, 7, 5);
        quizPanel.Controls.Add(_checkButton, 0, 2);

        ConfigureButton(_showAnswerButton, "Show answer", false);
        _showAnswerButton.Margin = new Padding(7, 1, 0, 5);
        quizPanel.Controls.Add(_showAnswerButton, 1, 2);

        _feedbackLabel.Dock = DockStyle.Fill;
        _feedbackLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        quizPanel.Controls.Add(_feedbackLabel, 0, 3);
        quizPanel.SetColumnSpan(_feedbackLabel, 2);

        right.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Text = "Select a word on the left, enter a translation, and check your answer.",
            ForeColor = Color.FromArgb(100, 116, 139),
            Padding = new Padding(0, 14, 0, 0),
        }, 0, 5);

        _statusLabel.Text = "Loading vocabulary...";
        _statusLabel.Dock = DockStyle.Fill;
        _statusLabel.ForeColor = Color.FromArgb(100, 116, 139);
        _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        page.Controls.Add(_statusLabel, 0, 2);
    }

    private static Control CreateTextSection(string title, Label content)
    {
        var section = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(0, 0, 0, 10),
        };
        section.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
        section.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        section.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(71, 85, 105)
        }, 0, 0);

        content.Dock = DockStyle.Fill;
        content.ForeColor = content.Font.Bold
            ? Color.FromArgb(30, 58, 94)
            : Color.FromArgb(55, 65, 81);
        section.Controls.Add(content, 0, 1);
        return section;
    }

    private static void ConfigureButton(Button button, string title, bool primary)
    {
        button.Text = title;
        button.Dock = DockStyle.Fill;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = primary ? 0 : 1;
        button.FlatAppearance.BorderColor = Color.FromArgb(190, 204, 223);
        button.BackColor = primary ? Color.FromArgb(37, 99, 235) : Color.FromArgb(241, 245, 249);
        button.ForeColor = primary ? Color.White : Color.FromArgb(30, 58, 94);
        button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        button.UseVisualStyleBackColor = false;
    }
}
