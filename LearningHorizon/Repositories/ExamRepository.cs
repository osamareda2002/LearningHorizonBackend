using LearningHorizon.Data;
using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearningHorizon.Repositories
{
    public class ExamRepository : GenericRepository<Exam> , IExamRepository
    {
        private readonly ApplicationDbContext _context;

        public ExamRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<DtoGetExam>> GetUpcomingExams(int userId)
        {
            var exams = await (from q in _context.Exams.AsNoTracking()
                               let userExam = _context.UserExams.Where(x => x.userId == userId && x.examId == q.id).FirstOrDefault()
                               where q.isDeleted != true && 
                                     q.startTime.AddMinutes(q.durationInMinutes) >= DateTime.UtcNow.AddHours(2) && 
                                     (userExam != null ? userExam.currentQuestionId != -1 : true)

                               select new DtoGetExam
                               {
                                   id = q.id,
                                   title = q.title,
                                   startTime = q.startTime,
                                   durationInMinutes = q.durationInMinutes,
                                   courseId = q.courseId,
                                   courseName = q.course.title,
                                   currentQuestionId = userExam.currentQuestionId == null ? 0 : userExam.currentQuestionId
                               }).OrderBy(x => x.startTime).ToListAsync();
            return exams;
        }

        public async Task<List<DtoGetExam>> GetAllExams()
        {
            var exams = await (from q in _context.Exams.AsNoTracking()
                               where q.isDeleted != true
                               select new DtoGetExam
                               {
                                   id = q.id,
                                   title = q.title,
                                   startTime = q.startTime,
                                   durationInMinutes = q.durationInMinutes,
                                   courseId = q.courseId,
                                   courseName = q.course.title
                               }).OrderByDescending(x => x.id).ToListAsync();
            return exams;
        }

        public async Task<DtoGetExam> AddExam(DtoAddExam dtoExam)
        {
            var course = await _context.Courses.Where(x => x.id == dtoExam.courseId).FirstOrDefaultAsync();

            if (course == null)
                return new DtoGetExam();

            var exam = new Exam
            {
                title = dtoExam.examTitle,
                startTime = dtoExam.startTime.AddHours(2),
                durationInMinutes = dtoExam.duration,
                courseId = (int)dtoExam.courseId
            };

            await AddAsync(exam);

            return new DtoGetExam
            {
                id = exam.id,
                title = exam.title,
                startTime = exam.startTime,
                durationInMinutes = exam.durationInMinutes,
                courseId = exam.courseId,
                courseName = course.title
            };
        }

        public async Task<DtoGetExamQuestions> GetExamQuestions(int examId)
        {   
            var result = await (from q in _context.Exams.AsNoTracking()
                                where q.id == examId && q.isDeleted != true
                                select new DtoGetExamQuestions
                                {
                                    examTitle = q.title,
                                    questions = q.questions.Select(x => new DtoExamQuestion
                                    {
                                        questionId = x.id,
                                        questionText = x.questionText,
                                        Mark = x.mark,
                                        options = x.answers.Select(a => new DtoAnswer
                                        {
                                            answerId = a.id,
                                            answerText = a.answerText
                                        }).ToList()
                                    }).ToList()
                                }).FirstOrDefaultAsync();
            return result;
        }


    }
}
