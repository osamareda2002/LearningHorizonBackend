using LearningHorizon.Data;
using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IO;
using static System.Net.WebRequestMethods;

namespace LearningHorizon.Repositories
{
    public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public LessonRepository(ApplicationDbContext context, HttpClient httpClient) : base(context)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<DtoGetLesson> SelectLessonById(int id)
        {
            var lesson = await _context.Lessons.AsNoTracking()
                .Where(l => l.id == id && !l.isDeleted)
                .Select(l => new DtoGetLesson
                {
                    id = l.id,
                    title = l.title,
                    path = GetBunnyVideoUrl(l.libraryId,l.guid),
                    isFree = l.isFree,
                    courseId = l.courseId,
                    duration = l.duration ?? 0
                }).FirstOrDefaultAsync();
            return lesson;
        }
        public async Task<List<DtoGetLesson>> SelectAllLessons(string baseUrl)
        {
            var lessons = await _context.Lessons.Where(x => !x.isDeleted).AsNoTracking()
                .Select(l => new DtoGetLesson
                {
                    id = l.id,
                    title = l.title,
                    path = GetBunnyVideoUrl(l.libraryId,l.guid),
                    isFree = l.isFree,
                    courseId = l.courseId,
                    courseTitle = l.course.title,
                    duration = l.duration ?? 0,
                    durationInMinutes = (int)Math.Round(l.duration.Value / 60.0),
                    arrange = l.lessonOrder,

                }).ToListAsync();
            return lessons;
        }
        public async Task<List<DtoGetLesson>> SelectLessonsByCourseId(int courseId, string baseUrl)
        {
            var lessons = await _context.Lessons.Where(x => x.courseId == courseId && !x.isDeleted).AsNoTracking()
                .Select(l => new DtoGetLesson
                {
                    id = l.id,
                    title = l.title,
                    path = GetBunnyVideoUrl(l.libraryId, l.guid),
                    isFree = l.isFree,
                    courseId = l.courseId,
                    duration = l.duration ?? 0,
                    durationInMinutes = (int)Math.Round(l.duration.Value / 60.0),
                    arrange = l.lessonOrder,
                    mcq = l.lessonExercises.Select(ex => new DtoGetLessonExercise
                    {
                        id = ex.id,
                        questionText = ex.questionText,
                        explanation = ex.explanation,
                        imageLink = $"{baseUrl}/Media/Images/LessonExercises/{Path.GetFileName(ex.imageLink)}",
                        answers = ex.lessonExerciseAnswers.Select(ans => new DtoGetExerciseAnswer
                        {
                            id = ans.id,
                            answerText = ans.answerText,
                            isCorrect = ans.isCorrect
                        }).OrderBy(x => x.id).ToList()
                    }).OrderBy(x => x.id).ToList()
                }).OrderBy(x => x.arrange).ToListAsync();
            return lessons;
        }

        public async Task RemoveCourseLessons(int courseId)
        {
            await _context.Lessons.Where(x => x.courseId == courseId)
                                  .ExecuteUpdateAsync(x => x.SetProperty(l => l.isDeleted, true));
        }

        public async Task<BunnyUploadToken> GetLessonAddToken(string lessonTitle)
        {
            const string libraryId = "600159";
            const string apiKey = "c93235e6-d853-4107-993e6ade1a6d-cd0c-4a5c";

            // Step 1: Create the video on Bunny and get the guid
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("AccessKey", apiKey);
            var json = JsonConvert.SerializeObject(new { title = lessonTitle });
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"https://video.bunnycdn.com/library/{libraryId}/videos", content);
            var raw = await response.Content.ReadAsStringAsync();
            var video = JsonConvert.DeserializeObject<BunnyVideoResponse>(raw);

            // Step 2: Generate a time-limited signed upload token
            var videoId = video.guid;
            var expirationTime = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds(); // 1 hour expiry

            // Signature = SHA256(apiKey + libraryId + expirationTime + videoId)
            var hashInput = $"{libraryId}{apiKey}{expirationTime}{videoId}";
            var signature = ComputeSha256Hash(hashInput);

            return new BunnyUploadToken
            {
                VideoId = videoId,
                LibraryId = libraryId,
                AuthorizationSignature = signature,
                AuthorizationExpire = expirationTime
            };
        }


        private string ComputeSha256Hash(string input)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

    }

    public class BunnyVideoResponse
    {
        public int videoLibraryId { get; set; }
        public string guid { get; set; }
        public string title { get; set; }
    }

    public class BunnyUploadToken
    {
        public string VideoId { get; set; }
        public string LibraryId { get; set; }
        public string AuthorizationSignature { get; set; }
        public long AuthorizationExpire { get; set; }
    }

}
