namespace DotNetNote.Models;

/// <summary>
/// 시험 클래스
/// </summary>
public class Exam
{
    /// <summary>
    /// 고유 번호
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 시험 이름
    /// </summary>
    public string Name { get; set; } = string.Empty;

    private IList<Question> _questions = new List<Question>();

    public IList<Question> Questions
    {
        get => _questions;
        set => _questions = value ?? new List<Question>();
    }

    public void AddQuestion(IList<Question> questions)
    {
        ArgumentNullException.ThrowIfNull(questions);

        foreach (var question in questions)
        {
            AddQuestion(question);
        }
    }

    public void AddQuestion(Question question)
    {
        ArgumentNullException.ThrowIfNull(question);

        _questions.Add(question);
    }

    public double TotalPoints =>
        _questions.Sum(question => question.Point);
}