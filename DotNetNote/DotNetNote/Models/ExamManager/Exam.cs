namespace DotNetNote.Models
{
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
        public string Name { get; set; }




        private IList<Question> _questions = new List<Question>();
        public IList<Question> Questions
        {
            get { return _questions;  }
            set { _questions = value;  }
        }

        public void AddQuestion(IList<Question> questions)
        {
            foreach(var question in questions)
            {
                AddQuestion(question);
            }
        }

        public void AddQuestion(Question question) => _questions.Add(question);

        public double TotalPoints
        {
            get
            {
                return (from q in _questions
                        select q.Point).Sum(); 
            }
        }
       
    }
}
