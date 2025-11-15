namespace LearningHorizon.Data.DTO
{
    public class DtoAddExamQuestions
    {
        public int examId { get; set; }
        public List<DtoExamQuestion> questions { get; set; }
    }
    public class DtoExamQuestion
    {
        public int? questionId { get; set; }
        public string questionText { get; set; }
        public double Mark { get; set; }
        public List<DtoAnswer> options { get; set; }
    }
    public class DtoAnswer
    {
        public int answerId { get; set; }
        public string answerText { get; set; }
        public bool isCorrect { get; set; }
    }

    public class DtoGetExamQuestions
    {
        public string examTitle { get; set; }
        public List<DtoExamQuestion> questions { get; set; }
    }
    public class DtoSubmitExamAnswers
    {
        public int examId { get; set; }
        public int questionId { get; set; }
        public int answerId { get; set; }
    }
    public class DtoReturnExamScore
    {
        public double totalScore { get; set; }
        public double obtainedScore { get; set; }
        public int totalQuestions { get; set; }
        public int totalSubmittedQuestions { get; set; }
        public int rightAnswers { get; set; }
        public int wrongAnswers { get; set; }
    }
    public class DtoQuestionAnswer
    {
        public int questionId { get; set; }
        public int answerId { get; set; }
    }
    public class DtoGetExamResults
    {
        public int userId { get; set; }
        public int examId { get; set; }
    }
}
