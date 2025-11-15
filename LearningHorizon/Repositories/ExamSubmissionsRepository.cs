using LearningHorizon.Data;
using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearningHorizon.Repositories
{
    public class ExamSubmissionsRepository : GenericRepository<ExamSubmission> , IExamSubmissionsRepository
    {
        private readonly ApplicationDbContext _context;

        public ExamSubmissionsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<DtoReturnExamScore> GetExamResults(DtoGetExamResults dto)
        {

            var submittedAnswers = await (from es in _context.ExamSubmissions.AsNoTracking()
                                      where es.examId == dto.examId && es.userId == dto.userId
                                      select new
                                      {
                                            questionId = es.quesionId,
                                            answerId = es.answerId,
                                            isCorrect = es.isCorrect,
                                            mark = es.question.mark
                                      }).ToListAsync();

            var exam = await _context.Exams
                                           .Where(q => q.id == dto.examId)
                                           .Select(q => new
                                           {
                                               totalQuestions = q.questions.Count(),
                                               totalScore = q.questions.Sum(ques => ques.mark)
                                           }).FirstOrDefaultAsync();

            var result = new DtoReturnExamScore
            {
                totalQuestions = exam.totalQuestions,
                totalSubmittedQuestions = submittedAnswers.Count(),
                totalScore = exam.totalScore,
                obtainedScore = submittedAnswers.Where(x => x.isCorrect).Sum(x => x.mark),
                rightAnswers = submittedAnswers.Where(x => x.isCorrect).Count(),
                wrongAnswers = exam.totalQuestions - submittedAnswers.Where(x => x.isCorrect).Count()
            };
            return result;
        }
    }
}
