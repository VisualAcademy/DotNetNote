namespace DotNetNote.Models
{
    public class Grade
    {
        public double TotalPoints { get; set; }
        public double Score { get; set; }
        public Exam Exam { get; set; }
    }
}
