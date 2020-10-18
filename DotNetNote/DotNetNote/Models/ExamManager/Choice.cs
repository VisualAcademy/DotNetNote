namespace DotNetNote.Models
{
    public class Choice
    {
        public int Id { get; set; }
        public string ChoiceText { get; set; }
        public bool IsAnswer { get; set; }
        public bool IsSelected { get; set; }

        private Question _question = new Question();
        public Question Question
        {
            get { return _question; }
            set { _question = value;}
        }
    }
}
