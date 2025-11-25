using System.Text;
using System.Text.Json;


namespace QuizApp;


public partial class MainPage : ContentPage
{
    private List<QuizQuestion> _questions;
    private int currentQuestionIndex = 0;


    private int currentPlayer = 1;
    private int score1 = 0;
    private int score2 = 0;
    private int questionsAnswered1 = 0;
    private int questionsAnswered2 = 0;


    public MainPage()
    {
        InitializeComponent();
        StartGame();
    }
    private async void StartGame()
    {
        await LoadQuestions();
        ShowQuestion();
    }


    private async Task LoadQuestions()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("pytania.json");
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var json = await reader.ReadToEndAsync();

        _questions = JsonSerializer.Deserialize<List<QuizQuestion>>(json);
    }


    private void ShowQuestion()
    {
        if (currentQuestionIndex >= _questions.Count)
        {
            EndGame();
            return;
        }


        var q = _questions[currentQuestionIndex];


        QuestionLabel.Text = q.Question;
        AnswerButton1.Text = q.Answers[0];
        AnswerButton2.Text = q.Answers[1];
        AnswerButton3.Text = q.Answers[2];
        AnswerButton4.Text = q.Answers[3];


        PlayerTurnLabel.Text = $"Tura gracza {currentPlayer}";
    }

    private void AnswerClicked(object sender, EventArgs e)
    {
        if (_questions == null) return;


        var button = (Button)sender;
        int answerIndex = button == AnswerButton1 ? 0 :
        button == AnswerButton2 ? 1 :
        button == AnswerButton3 ? 2 : 3;


        var correct = _questions[currentQuestionIndex].CorrectIndex;


        if (answerIndex == correct)
        {
            if (currentPlayer == 1) score1++; else score2++;
        }


        if (currentPlayer == 1) questionsAnswered1++; else questionsAnswered2++;


        if (questionsAnswered1 >= 5 && questionsAnswered2 >= 5)
        {
            EndGame();
            return;
        }


        currentPlayer = currentPlayer == 1 ? 2 : 1;
        currentQuestionIndex++;
        ShowQuestion();
    }
    private void EndGame()
    {
        QuestionLabel.IsVisible = false;
        AnswerButton1.IsVisible = false;
        AnswerButton2.IsVisible = false;
        AnswerButton3.IsVisible = false;
        AnswerButton4.IsVisible = false;
        PlayerTurnLabel.IsVisible = false;


        ResultFrame.IsVisible = true;


        if (score1 > score2)
            ResultLabel.Text = $"Wygrał Gracz 1! ({score1} : {score2})";
        else if (score2 > score1)
            ResultLabel.Text = $"Wygrał Gracz 2! ({score2} : {score1})";
        else
            ResultLabel.Text = $"Remis! ({score1} : {score2})";
    }

    private void RestartGame(object sender, EventArgs e)
    {
        currentQuestionIndex = 0;
        currentPlayer = 1;
        score1 = 0;
        score2 = 0;
        questionsAnswered1 = 0;
        questionsAnswered2 = 0;


        ResultFrame.IsVisible = false;
        QuestionLabel.IsVisible = true;
        PlayerTurnLabel.IsVisible = true;
        AnswerButton1.IsVisible = true;
        AnswerButton2.IsVisible = true;
        AnswerButton3.IsVisible = true;
        AnswerButton4.IsVisible = true;


        ShowQuestion();
    }
}
public class QuizQuestion
{
    public int Id { get; set; }
    public string Question { get; set; }
    public List<string> Answers { get; set; }
    public int CorrectIndex { get; set; }
}