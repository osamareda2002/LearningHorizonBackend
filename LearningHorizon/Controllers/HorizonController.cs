using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;
using LearningHorizon.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LearningHorizon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HorizonController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly ISliderRepository _sliderRepository;
        private readonly ISuggestRepository _suggestRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMeetingRepository _meetingRepository;
        private readonly IExamRepository _examRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IUserExamRepository _userExamRepository;
        private readonly IExamSubmissionsRepository _examSubmissionsRepository;
        private readonly ILessonExerciseRepository _lessonExerciseRepository;
        private readonly ILessonExerciseAnswerRepository _lessonExerciseAnswerRepository;
        private readonly ICourseCategoryRepository _courseCategoryRepository;
        private readonly IInstructorRepository _instructorRepository;
        private readonly JwtTokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private string baseUrl => $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        public HorizonController(IUserRepository userRepository, ICourseRepository courseRepository, ILessonRepository lessonRepository, ISliderRepository sliderRepository, ISuggestRepository suggestRepository, JwtTokenService tokenService, IOrderRepository orderRepository, IConfiguration configuration, IMemoryCache cache, IBookRepository bookRepository, IMeetingRepository meetingRepository, IExamRepository examRepository, IQuestionRepository questionRepository, IAnswerRepository answerRepository, IExamSubmissionsRepository examSubmissionsRepository, IUserExamRepository userExamRepository, ILessonExerciseRepository lessonExerciseRepository, ILessonExerciseAnswerRepository lessonExerciseAnswerRepository, ICourseCategoryRepository courseCategoryRepository, IInstructorRepository instructorRepository)
        {
            _userRepository = userRepository;
            _courseRepository = courseRepository;
            _lessonRepository = lessonRepository;
            _sliderRepository = sliderRepository;
            _suggestRepository = suggestRepository;
            _tokenService = tokenService;
            _orderRepository = orderRepository;
            _configuration = configuration;
            _cache = cache;
            _bookRepository = bookRepository;
            _meetingRepository = meetingRepository;
            _examRepository = examRepository;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _examSubmissionsRepository = examSubmissionsRepository;
            _userExamRepository = userExamRepository;
            _lessonExerciseRepository = lessonExerciseRepository;
            _lessonExerciseAnswerRepository = lessonExerciseAnswerRepository;
            _courseCategoryRepository = courseCategoryRepository;
            _instructorRepository = instructorRepository;
        }

        #region User


        [HttpGet]
        [Route("GetUserById")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound("No user with id : " + id);
            }
            return Ok(user);
        }

        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.SelectAllUsers();
            return Ok(users);
        }
        [HttpPost]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUser(DtoUpdateUser dtoUser)
        {
            var user = await _userRepository.EditUser(dtoUser);
            return Ok(user);
        }

        [HttpPost]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = _userRepository.FindBy(x => x.id == id && x.isDeleted != true).FirstOrDefault();

            if (user == null)
                return Ok(new { status = 400, message = "No user with this id" });
            else if (user.isOwner)
                return Ok(new { status = 400, message = "Can't Delete Owner" });

            user.isDeleted = true;
            await _userRepository.UpdateAsync(user);

            var result = await _userRepository.GetAllAsync();
            return Ok(new { status = 200, data = result });
        }

        [HttpPost]
        [Route("AssignCourseToUser")]
        public async Task<IActionResult> AssignCourseToUser(DtoAssignCourseToUser dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.userId);
            if (user == null)
                return NotFound("No user with this id");

            foreach (var id in dto.courseIds)
            {
                var course = _courseRepository.FindBy(x => x.id == id).FirstOrDefault();
                if (course == null)
                    return NotFound("No user with this id");

                if (!user.CoursesShowed.Contains(course))
                    user.CoursesShowed.Add(course);
            }
            await _userRepository.UpdateAsync(user);
            var result = await _userRepository.GetUserById(dto.userId);
            return Ok(result);
        }

        [HttpPost]
        [Route("RemoveCourseFromUser")]
        public async Task<IActionResult> RemoveCourseFromUser(DtoAssignCourseToUser dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.userId);
            if (user == null)
                return NotFound("No user with this id");

            foreach (var id in dto.courseIds)
            {
                var course = _courseRepository.FindBy(x => x.id == id).FirstOrDefault();
                if (course == null)
                    return NotFound("No user with this id");

                if (user.CoursesShowed.Contains(course))
                    user.CoursesShowed.Remove(course);

                if (user.CoursesPurchased.Contains(course))
                    user.CoursesPurchased.Remove(course);
            }
            await _userRepository.UpdateAsync(user);
            var result = await _userRepository.GetUserById(dto.userId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        [Route("GetPurchasedCourses")]
        public async Task<IActionResult> GetPurchasedCourses()
        {
            if (string.IsNullOrEmpty(User.FindFirst("id")?.Value))
                return Ok(new { status = 400, message = "s Not found" });
            var userId = int.Parse(User.FindFirst("id")?.Value);

            var user = _userRepository.FindBy(x => x.id == userId && !x.isDeleted).FirstOrDefault();
            if (user == null)
                return Ok(new { status = 400, message = "User Not found" });


            var data = await _userRepository.GetUserById(userId);

            return Ok(new { status = 200, data = data });
        }

        #endregion


        #region Book


        [HttpPost]
        [Route("AddNewBook")]
        public async Task<IActionResult> AddNewBook([FromForm] DtoAddBook dtoBook)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string bookFilePath = Path.Combine(currentDirectory, "Media", "Books", dtoBook.bookFile.FileName);
            string coverFilePath = Path.Combine(currentDirectory, "Media", "Images", "Books Cover Images", dtoBook.coverImage.FileName);
            try
            {
                string directoryName = Path.GetDirectoryName(bookFilePath);
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);
                using (FileStream stream = new FileStream(bookFilePath, FileMode.Create))
                    await dtoBook.bookFile.CopyToAsync((Stream)stream);

                directoryName = Path.GetDirectoryName(coverFilePath);
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);
                using (FileStream stream = new FileStream(coverFilePath, FileMode.Create))
                    await dtoBook.coverImage.CopyToAsync((Stream)stream);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            Book book = new Book
            {
                title = dtoBook.title,
                description = dtoBook.description,
                bookPath = bookFilePath,
                posterPath = coverFilePath,
                createdAt = DateTime.UtcNow
            };

            await _bookRepository.AddAsync(book);
            var result = await _bookRepository.GetBookById(book.id);
            return Ok(result);
        }


        [HttpGet]
        [Route("GetAllBooks")]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookRepository.GetAllBooks();
            return Ok(books);
        }

        [HttpGet]
        [Route("GetBookCoverImage")]
        public async Task<IActionResult> GetBookCoverImage(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null || string.IsNullOrEmpty(book.posterPath))
                return NotFound("No book with this id or no file associated with it");

            var filePath = book.posterPath;
            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found");

            var contentType = MediaHelper.GetContentType(filePath);
            var fileInfo = new FileInfo(filePath);
            var fileLength = fileInfo.Length;

            var request = Request;
            var response = Response;

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            if (request.Headers.ContainsKey("Range"))
            {
                var rangeHeader = request.Headers["Range"].ToString();
                var range = rangeHeader.Replace("bytes=", "").Split('-');

                long start = string.IsNullOrEmpty(range[0]) ? 0 : Convert.ToInt64(range[0]);
                long end = string.IsNullOrEmpty(range[1]) ? fileLength - 1 : Convert.ToInt64(range[1]);

                if (start > end || start < 0 || end >= fileLength)
                {
                    stream.Dispose(); // Dispose early on invalid range
                    return BadRequest("Invalid Range");
                }

                stream.Seek(start, SeekOrigin.Begin);
                long length = end - start + 1;

                response.StatusCode = 206; // Partial Content
                response.ContentLength = length;
                response.Headers.Add("Content-Range", $"bytes {start}-{end}/{fileLength}");

                return File(stream, contentType, enableRangeProcessing: true);
            }

            // No Range header - return full content
            response.ContentLength = fileLength;
            return File(stream, contentType, enableRangeProcessing: true);
        }

        [HttpGet]
        [Route("GetBookFile")]
        public async Task<IActionResult> GetBookFile(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null || string.IsNullOrEmpty(book.bookPath))
                return NotFound("No book with this id or no file associated with it");

            var filePath = book.bookPath;
            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found");

            var contentType = MediaHelper.GetContentType(filePath);
            var fileInfo = new FileInfo(filePath);
            var fileLength = fileInfo.Length;

            var request = Request;
            var response = Response;

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            if (request.Headers.ContainsKey("Range"))
            {
                var rangeHeader = request.Headers["Range"].ToString();
                var range = rangeHeader.Replace("bytes=", "").Split('-');

                long start = string.IsNullOrEmpty(range[0]) ? 0 : Convert.ToInt64(range[0]);
                long end = string.IsNullOrEmpty(range[1]) ? fileLength - 1 : Convert.ToInt64(range[1]);

                if (start > end || start < 0 || end >= fileLength)
                {
                    stream.Dispose(); // Dispose early on invalid range
                    return BadRequest("Invalid Range");
                }

                stream.Seek(start, SeekOrigin.Begin);
                long length = end - start + 1;

                response.StatusCode = 206; // Partial Content
                response.ContentLength = length;
                response.Headers.Add("Content-Range", $"bytes {start}-{end}/{fileLength}");

                return File(stream, contentType, enableRangeProcessing: true);
            }

            // No Range header - return full content
            response.ContentLength = fileLength;
            return File(stream, contentType, enableRangeProcessing: true);
        }

        #endregion


        #region Course

        [HttpGet]
        [Route("GetAllCourses")]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _courseRepository.SelectAllCourses(baseUrl);
            return Ok(courses);
        }

        [HttpGet]
        [Route("GetCourseById")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await _courseRepository.SelectCourseById(id);
            return Ok(course);
        }

        [HttpGet]
        [Route("GetCourseByCategory")]
        public async Task<IActionResult> GetCourseByCategory(int categoryId)
        {
            var result = await _courseRepository.SelectCoursesByCategory(categoryId, baseUrl);
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [Route("AddNewCourse")]
        public async Task<IActionResult> AddNewCourse([FromForm] DtoAddCourse dtoCourse)
        {

            if (string.IsNullOrEmpty(User.FindFirst("id")?.Value))
                return Ok(new { status = 400, message = "s Not found" });
            var addminId = int.Parse(User.FindFirst("id")?.Value);
            var admin = _userRepository.FindBy(x => x.id == addminId && !x.isDeleted).FirstOrDefault();
            if (admin == null)
                return Ok(new { status = 400, message = "User Not found" });

            if (!admin.isAdmin)
                return Ok(new { status = 400, message = "Not Have permission to add" });


            string currentDirectory = Directory.GetCurrentDirectory();
            string folderPath = Path.Combine(currentDirectory, "Media", "Courses", dtoCourse.courseTitle);
            string courseImagePath = Path.Combine(currentDirectory, "Media", "Images", "CourseImages", dtoCourse.courseImage.FileName);

            try
            {
                string directoryName = Path.GetDirectoryName(courseImagePath);
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);
                using (FileStream stream = new FileStream(courseImagePath, FileMode.Create))
                    await dtoCourse.courseImage.CopyToAsync((Stream)stream);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var course = new Course
            {
                title = dtoCourse.courseTitle,
                creator = dtoCourse.courseCreator,
                price = (decimal)dtoCourse.coursePrice,
                path = folderPath,
                imagePath = courseImagePath,
                categoryId = dtoCourse.categoryId
            };
            var addedCourse = await _courseRepository.AddAsync(course);

            foreach (var u in await _userRepository.SelectAllUsers())
            {
                var user = await _userRepository.GetByIdAsync(u.id);
                if (user != null)
                {
                    user.CoursesShowed.Add(addedCourse);
                    _userRepository.Update(user);
                }
            }
            await _userRepository.SaveChangesAsync();

            var result = await _courseRepository.SelectCourseById(addedCourse.id);
            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateCourse")]
        public async Task<IActionResult> UpdateCourse([FromForm] DtoAddCourse dtoCourse)
        {
            var course = await _courseRepository.GetByIdAsync((int)dtoCourse.courseId);
            if (course == null)
                return NotFound("No course with this id");

            if (dtoCourse.courseTitle != null) course.title = dtoCourse.courseTitle;
            if (dtoCourse.courseCreator != null) course.creator = dtoCourse.courseCreator;
            if (dtoCourse.coursePrice != null) course.price = (decimal)dtoCourse.coursePrice;
            if (dtoCourse.courseImage != null)
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                string courseImagePath = Path.Combine(currentDirectory, "Media", "Images", "CourseImages", dtoCourse.courseImage.FileName);
                try
                {
                    string directoryName = Path.GetDirectoryName(courseImagePath);
                    if (!Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);
                    using (FileStream stream = new FileStream(courseImagePath, FileMode.Create))
                        await dtoCourse.courseImage.CopyToAsync((Stream)stream);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                course.imagePath = courseImagePath;
            }
            if (dtoCourse.categoryId != null && dtoCourse.categoryId != 0) course.categoryId = dtoCourse.categoryId;

            await _courseRepository.UpdateAsync(course);
            var result = await _courseRepository.SelectCourseById(course.id);
            return Ok(new { status = 200, data = result });
        }

        [HttpGet]
        [Route("DeleteCourse")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                return NotFound("No course with this id");
            course.isDeleted = true;
            await _courseRepository.UpdateAsync(course);

            // Remove course from all users
            try
            {
                var users = await _userRepository.GetAllUsersIncluding();
                foreach (var user in users)
                {
                    user.CoursesShowed.Remove(course);
                    user.CoursesPurchased.Remove(course);
                    _userRepository.Update(user);
                }
                await _userRepository.SaveChangesAsync();
                await _lessonRepository.RemoveCourseLessons(id);
                return Ok(new { status = 200, data = "Course deleted successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, data = ex.Message });
                throw;
            }

            // Remove Lessons 


        }

        #endregion


        #region Category

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _courseCategoryRepository.GetAllCategories(baseUrl);
            return Ok(categories);
        }

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromForm] DtoAddEditCategory dto)
        {
            var categories = await _courseCategoryRepository.AddCategory(dto,baseUrl);
            return Ok(categories);
        }

        [HttpPost("EditCategory")]
        public async Task<IActionResult> EditCategory([FromForm] DtoAddEditCategory dto)
        {
            bool isExist = _courseCategoryRepository.FindBy(x => x.id == dto.id).AsQueryable().Any();
            if (isExist)
            {
                var categories = await _courseCategoryRepository.EditCategory(dto, baseUrl);
                return Ok(new { status = 200, data = categories });
            }
            else
            {
                return Ok(new { status = 400 });
            }
        }

        [HttpGet("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            bool isExist = _courseCategoryRepository.FindBy(x => x.id == id).AsQueryable().Any();
            if (isExist)
            {
                var categories = await _courseCategoryRepository.DeleteCategory(id, baseUrl);
                return Ok(new { status = 200, data = categories });
            }
            else
            {
                return Ok(new { status = 400 });
            }
        }

        #endregion


        #region Lesson

        [HttpGet]
        [Route("GetAllLessons")]
        public async Task<IActionResult> GetAllLessons()
        {
            var lessons = await _lessonRepository.SelectAllLessons(baseUrl);
            return Ok(lessons);
        }

        [HttpGet]
        [Route("GetLessonById")]
        public async Task<IActionResult> GetLessonById(int id)
        {
            var lesson = await _lessonRepository.SelectLessonById(id);
            if (lesson == null)
                return NotFound("No lesson with this id");
            return Ok(lesson);
        }

        [HttpGet]
        [Route("GetLessonsByCourseId")]
        public async Task<IActionResult> GetLessonsByCourseId(int courseId)
        {
            var lessons = await _lessonRepository.SelectLessonsByCourseId(courseId, baseUrl);
            if (lessons == null || lessons.Count == 0)
                return NotFound("No lessons found for this course");
            return Ok(lessons);
        }

        [HttpGet("GetLessonAddToken")]
        public async Task<IActionResult> GetLessonAddToken(int courseId, string lessonTitle)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                return NotFound("No course with this id");

            var result = await _lessonRepository.GetLessonAddToken(lessonTitle);
            return Ok(new { status = 200, data = result });
        }

        [HttpPost("AddLesson")]
        public async Task<IActionResult> AddLesson([FromForm] DtoAddLesson dtoLesson)
        {
            var lesson = new Lesson
            {
                title = dtoLesson.title,
                isFree = dtoLesson.isFree,
                courseId = dtoLesson.courseId,
                duration = dtoLesson.durationInSeconds,
                lessonOrder = dtoLesson.lessonOrder,
                guid = dtoLesson.guid,
                libraryId = dtoLesson.libraryId,
                path = ""
            };

            await _lessonRepository.AddAsync(lesson);

            #region add mcq
            foreach (var mcq in dtoLesson.lessonExercises)
            {
                var exercise = new LessonExercise
                {
                    lessonId = lesson.id,
                    questionText = mcq.questionText,
                    explanation = mcq.explanation,
                    quoteSubject = mcq.quoteSubject,
                    quoteBody = mcq.quoteBody
                };
                await _lessonExerciseRepository.AddAsync(exercise);

                string ImagePath = "";
                if (mcq.image != null)
                {
                    string currentDirectory = Directory.GetCurrentDirectory();
                    ImagePath = Path.Combine(currentDirectory, "Media", "Images", "LessonExercises", $"{exercise.id}_{mcq.image.FileName}");


                    string directoryName = Path.GetDirectoryName(ImagePath);
                    if (!Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);

                    using (FileStream stream = new FileStream(ImagePath, FileMode.Create))
                        await mcq.image.CopyToAsync((Stream)stream);

                    exercise.imageLink = ImagePath.IsNullOrEmpty() ? null : ImagePath;
                    await _lessonExerciseRepository.UpdateAsync(exercise);
                }

                foreach (var answer in mcq.answers)
                {
                    var answerObj = new LessonExerciseAnswer
                    {
                        answerText = answer.answerText,
                        isCorrect = answer.isCorrect,
                        lessonExerciseId = exercise.id
                    };

                    _lessonExerciseAnswerRepository.Add(answerObj);
                }
                await _lessonExerciseAnswerRepository.SaveChangesAsync();
            }
            #endregion

            var result = await _lessonRepository.SelectLessonById(lesson.id);
            return Ok(result);
        }

        [HttpPost]
        [RequestSizeLimit(10737418240 /*0x0280000000*/)]
        [Route("EditLesson")]
        public async Task<IActionResult> EditLesson([FromForm] DtoEditLesson dtoLesson)
        {
            var lesson = await _lessonRepository.GetByIdAsync(dtoLesson.id);
            if (lesson == null)
                return NotFound("No lesson with this id");

            if (dtoLesson.title != null) lesson.title = dtoLesson.title;
            if (dtoLesson.isFree != lesson.isFree) lesson.isFree = (bool)dtoLesson.isFree;
            if (dtoLesson.courseId != lesson.courseId)
            {
                var course = await _courseRepository.GetByIdAsync(dtoLesson.courseId);
                if (course == null)
                    return NotFound("No course with this id");
                lesson.courseId = dtoLesson.courseId;
                await _lessonRepository.UpdateAsync(lesson);
            }
            if (dtoLesson.lessonFile != null)
            {
                if (!string.IsNullOrEmpty(lesson.path) && System.IO.File.Exists(lesson.path))
                    System.IO.File.Delete(lesson.path);

                string parentDirectory = Directory.GetCurrentDirectory();

                string str = Path.Combine(parentDirectory, "Media", "Courses", lesson.course.title);
                if (!Directory.Exists(str))
                    Directory.CreateDirectory(str);

                string path = Path.Combine(str, dtoLesson.lessonFile.FileName);

                if (!string.IsNullOrEmpty(lesson.path) && System.IO.File.Exists(lesson.path))
                    System.IO.File.Delete(lesson.path);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                    await dtoLesson.lessonFile.CopyToAsync((Stream)stream);
                lesson.path = path;
            }

            await _lessonRepository.UpdateAsync(lesson);
            var result = await _lessonRepository.SelectLessonById(lesson.id);
            return Ok(result);
        }

        [HttpGet]
        [Route("DeleteLesson")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            try
            {
                var lesson = await _lessonRepository.GetByIdAsync(id);
                if (lesson == null)
                    return NotFound("No lesson with this id");
                lesson.isDeleted = true;
                await _lessonRepository.UpdateAsync(lesson);
                return Ok(new { status = 200, data = "Lesson deleted successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, data = ex.Message });
                throw;
            }
        }

        #endregion


        #region Slider


        [HttpGet]
        [Route("GetAllSliders")]
        public async Task<IActionResult> GetAllSliders()
        {
            var sliders = await _sliderRepository.GetAllSliders(baseUrl);
            return Ok(sliders);
        }

        [HttpGet]
        [Route("GetSliderById")]
        public async Task<IActionResult> GetSliderById(int id)
        {
            var slider = await _sliderRepository.GetById(id, baseUrl);
            if (slider == null)
                return NotFound("No slider with this id");
            return Ok(slider);
        }

        [HttpPost]
        [Route("AddSlider")]
        public async Task<IActionResult> AddSlider([FromForm] DtoAddSlider dtoSlider)
        {
            var titleIsExist = _sliderRepository.FindBy(x => x.title == dtoSlider.title && !x.isDeleted);
            if (titleIsExist.Any())
                return BadRequest("Slider title already exists");

            var extension = Path.GetExtension(dtoSlider.file.FileName).ToLower();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Media", "Images", "Sliders", dtoSlider.title + extension);

            try
            {
                string directoryName = Path.GetDirectoryName(path);
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                    await dtoSlider.file.CopyToAsync((Stream)stream);

                var slider = new Slider
                {
                    title = dtoSlider.title,
                    link = dtoSlider.link,
                    path = path,
                };

                await _sliderRepository.AddAsync(slider);
                var result = await _sliderRepository.GetById(slider.id, baseUrl);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("DeleteSlider")]
        public async Task<IActionResult> DeleteSlider(int id)
        {
            var slider = await _sliderRepository.GetByIdAsync(id);

            if (slider == null)
                return NotFound("No slider with this id");
            if (System.IO.File.Exists(slider.path))
                System.IO.File.Delete(slider.path);

            slider.isDeleted = true;
            await _sliderRepository.UpdateAsync(slider);
            return Ok(new { status = 200, data = "Slider deleted successfully" });
        }

        #endregion


        #region Suggest Videos


        [HttpGet]
        [Route("GetAllSuggestions")]
        public async Task<IActionResult> GetAllSuggestions()
        {
            var suggests = await _suggestRepository.GetAllSuggests(baseUrl);
            return Ok(suggests);
        }

        [HttpGet]
        [Route("GetSuggestVideoById")]
        public async Task<IActionResult> GetSuggestVideoById(int id)
        {
            var suggest = await _suggestRepository.GetByIdAsync(id);
            return Ok(suggest);
        }

        [RequestSizeLimit(268435456)]
        [RequestFormLimits(MultipartBodyLengthLimit = 268435456)]
        [HttpPost]
        [Route("AddSuggestVideo")]
        public async Task<IActionResult> AddSuggestVideo([FromForm] DtoAddSuggestVideo dtoSuggestVideo)
        {
            _cache.Remove("all_suggestions");
            bool titleIsExist = _suggestRepository.FindBy(x => x.title == dtoSuggestVideo.title && !x.isDeleted).Any();
            if (titleIsExist)
                return BadRequest("Video title already exists");

            var extension = Path.GetExtension(dtoSuggestVideo.file.FileName).ToLower();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Media", "Suggest", dtoSuggestVideo.title + extension);

            try
            {
                string directoryName = Path.GetDirectoryName(path);
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                    await dtoSuggestVideo.file.CopyToAsync((Stream)stream);

                var suggest = new Suggest
                {
                    title = dtoSuggestVideo.title,
                    path = path,
                    instructorName = dtoSuggestVideo.instructorName
                };

                await _suggestRepository.AddAsync(suggest);
                var result = await _suggestRepository.GetById(suggest.id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("EditSuggestion")]
        public async Task<IActionResult> EditSuggestion(DtoEditSuggestion dto)
        {
            var suggestion = _suggestRepository.FindBy(x => x.id == dto.id && x.isDeleted != true).FirstOrDefault();
            if (suggestion != null)
            {
                if (dto.title != null) suggestion.title = dto.title;
                await _suggestRepository.UpdateAsync(suggestion);
            }
            return Ok();
        }

        [HttpGet]
        [Route("DeleteSuggest")]
        public async Task<IActionResult> DeleteSuggestVideo(int id)
        {
            var suggest = await _suggestRepository.GetByIdAsync(id);
            if (suggest == null)
                return BadRequest("Invaild Id");

            suggest.isDeleted = true;
            await _suggestRepository.UpdateAsync(suggest);
            return Ok(new { status = 200, data = "Suggest deleted successfully" });
        }

        #endregion


        #region Payment


        [Authorize]
        [HttpPost]
        [Route("PurchaseCourse")]
        public async Task<IActionResult> PurchaseCourse(DtoAddOrder dtoOrder)
        {
            if (string.IsNullOrEmpty(User.FindFirst("id")?.Value))
                return Ok(new { status = 400, message = "s Not found" });
            var userId = int.Parse(User.FindFirst("id")?.Value);

            var user = _userRepository.FindBy(x => x.id == userId && !x.isDeleted).FirstOrDefault();
            if (user == null)
                return Ok(new { status = 400, message = "User Not found" });


            var course = _courseRepository.FindBy(x => x.id == dtoOrder.courseId && !x.isDeleted).FirstOrDefault();
            if (course == null)
                return Ok(new { status = 400, message = "Course Not found" });
            if (user.CoursesPurchased.Contains(course))
                return Ok(new { status = 400, message = "Course already purchased" });

            var order = new Order
            {
                userId = userId,
                courseId = course.id,
                createdAt = DateTime.UtcNow,
                totalAmount = course.price,
                status = "Pending"
            };


            try
            {
                var iFrameUrl = await _orderRepository.purchaseCourse(order, user);
                await _orderRepository.AddAsync(order);
                return Ok(new { status = 200, message = iFrameUrl });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }

        }

        [HttpGet]
        [Route("callback")]
        public async Task<IActionResult> PaymobCallback([FromQuery] DtoPaymobCallbackModel callbackData)
        {
            var secret = _configuration["LocalPayment:hmac_secret"];
            var receivedHmac = callbackData.hmac;

            string concatenatedString = callbackData.amount_cents.ToString() +
                                        callbackData.created_at +
                                        callbackData.currency +
                                        callbackData.error_occured.ToString().ToLower() +
                                        callbackData.has_parent_transaction.ToString().ToLower() +
                                        callbackData.id.ToString() +
                                        callbackData.integration_id.ToString() +
                                        callbackData.is_3d_secure.ToString().ToLower() +
                                        callbackData.is_auth.ToString().ToLower() +
                                        callbackData.is_capture.ToString().ToLower() +
                                        callbackData.is_refund.ToString().ToLower() +
                                        callbackData.is_standalone_payment.ToString().ToLower() +
                                        callbackData.is_void.ToString().ToLower() +
                                        callbackData.order.ToString() +
                                        callbackData.owner.ToString() +
                                        callbackData.pending.ToString().ToLower() +
                                        callbackData.source_data.pan +
                                        callbackData.source_data.sub_type +
                                        callbackData.source_data.type +
                                        callbackData.success.ToString().ToLower();


            using var hmac = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString));
            var computedHmac = BitConverter.ToString(hash).Replace("-", "").ToLower();

            if (computedHmac == receivedHmac)
            {
                try
                {
                    var order = _orderRepository.FindBy(x => x.paymobOrderId == callbackData.order.ToString()).FirstOrDefault();
                    if (order == null)
                        return Ok(new { status = 400, message = "Order Not Found" });

                    order.status = callbackData.success == true ? "Success" : "Failed";
                    order.updatedAt = DateTime.Parse(callbackData.updated_at);
                    await _orderRepository.UpdateAsync(order);

                    if (callbackData.success)
                    {
                        await _userRepository.AddPurchasedCourse(order.courseId, order.userId);
                        return Redirect($"https://www.learning-horizon.com/material");
                    }
                    return Ok(new { status = 400, message = "Operation Failed" });
                }
                catch (Exception ex)
                {
                    return Ok(new { status = 400, message = ex.Message });
                }
            }
            else
            {
                return Ok(new { status = 401, message = "UnAuthorized" });
            }
        }

        #endregion


        #region Meeting Sessions

        [HttpPost]
        [Route("AddNewMeeting")]
        public async Task<IActionResult> AddNewMeeting(DtoAddNewMeeting dto)
        {
            if (string.IsNullOrEmpty(User.FindFirst("id")?.Value))
                return Ok(new { status = 400, message = "s Not found" });
            var userId = int.Parse(User.FindFirst("id")?.Value);

            var user = _userRepository.FindBy(x => x.id == userId && !x.isDeleted).Select(u => new
            {
                userId = userId,
                email = u.email,
                isAdmin = u.isAdmin
            }).FirstOrDefault();
            if (user == null || !user.isAdmin)
                return Ok(new { status = 400, message = "User UnAuthorized" });

            dto.hostId = userId;
            dto.hostEmail = user.email;
            var result = await _meetingRepository.AddNewMeeting(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllMeetingsInfo")]
        public async Task<IActionResult> GetAllMeetingsInfo()
        {
            var result = await _meetingRepository.DtoGetAllMeetingsInfo();
            return Ok(result);
        }

        [HttpPost("GenerateZoomSignature")]
        public IActionResult GenerateSignature(ZoomSignatureRequest request)
        {
            var sdkKey = _configuration["Zoom:sdkKey"];
            var sdkSecret = _configuration["Zoom:sdkSecret"];

            if (string.IsNullOrEmpty(request.MeetingNumber))
                return BadRequest("Meeting number is required.");

            try
            {
                long iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                long exp = iat + 60 * 60 * 2; // valid for 2 hours

                var payload = new
                {
                    appKey = sdkKey,
                    mn = request.MeetingNumber,
                    role = request.Role,
                    iat = iat,
                    exp = exp,
                    tokenExp = exp
                };

                var secretBytes = Encoding.UTF8.GetBytes(sdkSecret);

                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                IJsonSerializer serializer = new JsonNetSerializer();
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

                var token = encoder.Encode(payload, secretBytes);

                return Ok(new { signature = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }


        }

        [HttpGet("HostJoined")]
        public async Task<IActionResult> HostJoined(string meetingNumber)
        {
            var meeting = _meetingRepository.FindBy(m => m.meetingId.ToString() == meetingNumber).FirstOrDefault();
            if (meeting == null)
                return NotFound("Meeting not found");
            meeting.adminJoined = true;
            await _meetingRepository.UpdateAsync(meeting);
            return Ok(new { status = 200, data = "Host joined updated successfully" });
        }

        [HttpGet("MeetingFinished")]
        public async Task<IActionResult> MeetingFinished(string meetingNumber)
        {
            var meeting = _meetingRepository.FindBy(m => m.meetingId.ToString() == meetingNumber).FirstOrDefault();
            if (meeting == null)
                return NotFound("Meeting not found");
            meeting.isFinished = true;
            await _meetingRepository.UpdateAsync(meeting);
            return Ok(new { status = 200, data = "Meeting updated successfully" });
        }

        [HttpGet("DeleteMeeting")]
        public async Task<IActionResult> DeleteMeeting(int id)
        {
            var meeting = await _meetingRepository.GetByIdAsync(id);
            if (meeting != null)
            {
                meeting.isDeleted = true;
                await _meetingRepository.UpdateAsync(meeting);
            }
            return Ok();
        }

        #endregion


        #region Exams

        [HttpGet("GetUpcomingExams")]
        public async Task<IActionResult> GetUpcomingExams()
        {
            var userId = getUserId();
            if(userId == -1)
                return Ok(new { status = 400, data = "User not found" });
            var result = await _examRepository.GetUpcomingExams(userId);
            return Ok(result);
        }
        [HttpGet("GetAllExams")]
        public async Task<IActionResult> GetAllExams()
        {
            var result = await _examRepository.GetAllExams();
            return Ok(result);
        }
        [HttpPost("AddExam")]
        public async Task<IActionResult> AddExam(DtoAddExam dtoExam)
        {
            try
            {
                var result = await _examRepository.AddExam(dtoExam);
                return Ok(new { status = 200, data = result });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, data = "something went wrong" });
            }
        }
        [HttpGet("DeleteExam")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return Ok(new { status = 400, data = "something went wrong" });

            exam.isDeleted = true;
            _examRepository.Update(exam);
            await _examRepository.SaveChangesAsync();
            return Ok(new { status = 200, data = "done" });
        }
        [HttpPost("AddExamQuestions")]
        public async Task<IActionResult> AddExamQuestions(DtoAddExamQuestions dtoExamQuestions)
        {
            var exam = await _examRepository.GetByIdAsync(dtoExamQuestions.examId);
            if (exam == null)
                return Ok(new { status = 400, data = "something went wrong" });

            try
            {
                foreach (var question in dtoExamQuestions.questions)
                {
                    var questionObject = new Question
                    {
                        questionText = question.questionText,
                        examId = dtoExamQuestions.examId,
                        mark = question.Mark
                    };
                    await _questionRepository.AddAsync(questionObject);

                    foreach (var option in question.options)
                    {
                        var answerObject = new Answer
                        {
                            answerText = option.answerText,
                            isCorrect = option.isCorrect,
                            questionId = questionObject.id
                        };
                        _answerRepository.Add(answerObject);
                    }
                }
                await _answerRepository.SaveChangesAsync();
                return Ok(new { status = 200, data = "Questions added successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, data = "something went wrong" });
            }
        }

        [HttpGet("GetExamQuestions")]
        public async Task<IActionResult> GetExamQuestions(int examId)
        {
            try
            {
                var result = await _examRepository.GetExamQuestions(examId);
                return Ok(new { status = 200, data = result });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception is : {ex.Message}");
                return Ok(new { status = 400, data = "something went wrong" });
            }
        }

        [HttpPost("SubmitExamAnswers")]
        public async Task<IActionResult> SubmitExamAnswers(DtoSubmitExamAnswers dtoSubmissions)
        {
            try
            {
                var userId = getUserId();
                if (userId == -1)
                    return Ok(new { status = 400, data = "User not found" });

                var question = await _questionRepository.GetByIdAsync(dtoSubmissions.questionId);
                var correctAnswer = question.answers.FirstOrDefault(a => a.isCorrect);
                var userAnswer = await _answerRepository.GetByIdAsync(dtoSubmissions.answerId);
                var userExam = _userExamRepository.FindBy(ue => ue.userId == userId && ue.examId == dtoSubmissions.examId).FirstOrDefault();
                if (question == null || correctAnswer == null || userAnswer == null)
                    return Ok(new { status = 400, data = "something went wrong" });

                var submissionObj = _examSubmissionsRepository.FindBy(x => x.userId == userId &&
                                                                           x.examId == dtoSubmissions.examId &&
                                                                           x.quesionId == dtoSubmissions.questionId).FirstOrDefault();
                if(submissionObj != null)
                {
                    submissionObj.answerId = dtoSubmissions.answerId;
                    _examSubmissionsRepository.Update(submissionObj);
                    await _examSubmissionsRepository.SaveChangesAsync();

                }
                else
                {
                    var submission = new ExamSubmission
                    {
                        examId = dtoSubmissions.examId,
                        userId = userId,
                        quesionId = dtoSubmissions.questionId,
                        answerId = dtoSubmissions.answerId,
                        submissionTime = DateTime.UtcNow,
                        isCorrect = correctAnswer.id == userAnswer.id
                    };

                    await _examSubmissionsRepository.AddAsync(submission);
                }

                
                if (userExam != null)
                {
                    userExam.currentQuestionId = dtoSubmissions.questionId;
                    _userExamRepository.Update(userExam);
                }
                else
                {
                    var userExamObj = new UserExam
                    {
                        userId = userId,
                        examId = dtoSubmissions.examId,
                        currentQuestionId = dtoSubmissions.questionId
                    };
                    _userExamRepository.Add(userExamObj);
                }

                await _userExamRepository.SaveChangesAsync();

                return Ok(new { status = 200, data = "Answer submitted successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception is : {ex.Message}");
                return Ok(new { status = 400, data = "something went wrong" });
            }
        }

        [HttpGet("FinishExam")]
        public async Task<IActionResult> FinishExam(int examId)
        {
            var userId = getUserId();
            if(userId == -1)
            {
                return Ok(new { status = 400, data = "User not found" });
            }

            var userExam = _userExamRepository.FindBy(ue => ue.userId == userId && ue.examId == examId).FirstOrDefault();
            userExam.userFinished = true;
            userExam.currentQuestionId = -1;
            _userExamRepository.Update(userExam);
            await _userExamRepository.SaveChangesAsync();
            return Ok(new { status = 200, data = "Exam Finished Successfully" });
        }

        [HttpGet("GetExamResults")]
        public async Task<IActionResult> GetExamResults(int examId)
        {
            var userId = getUserId();
            if(userId == -1)
                return Ok(new { status = 400, data = "User not found" });

            var result = await _examSubmissionsRepository.GetExamResults(new DtoGetExamResults
            {
                examId = examId,
                userId = userId
            });

            return Ok(new { status = 200, data = result });
        }

        [HttpGet("RemoveQuestion")]
        public async Task<IActionResult> RemoveQuestion(int questionId)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(questionId);
                if (question != null)
                {
                    _questionRepository.Delete(question);
                    await _questionRepository.SaveChangesAsync();
                }

                return Ok(new { status = 200, data = "Question Removed Successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception is : {ex.Message}");
                return Ok(new { status = 400, data = "something went wrong" });
            }
        }
        #endregion


        #region Instructors
        [HttpGet("GetAllInstructors")]
        public async Task<IActionResult> GetAllInstructors()
        {
            var dbInstructors = await _instructorRepository.GetAllAsync();

            var instructors = dbInstructors.Select(x => new
            {
                id = x.id,
                name = x.name,
                specialty = x.specialty,
                description = x.description,
                expertise = x.expertise, 
                imageUrl = $"{baseUrl}/Media/Images/Instructors/{Path.GetFileName(x.imageUrl)}",
                facebookUrl = x.facebookUrl,
                whatsappUrl = x.whatsappUrl,
                instgramUrl = x.instgramUrl,
                isDeveloper = x.isDeveloper,
                tag = x.tag
            }).OrderBy(x => x.id);

            return Ok(new { status = 200, data = instructors});
        }

        [HttpPost("AddNewInstructor")]
        public async Task<IActionResult> AddNewInstructor([FromForm] DtoInstructorAddEdit dto)
        {
            string imageUrl = "";
            if (dto.image != null)
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                string instructorImagePath = Path.Combine(currentDirectory, "Media", "Images", "Instructors", dto.image.FileName);

                try
                {
                    string directoryName = Path.GetDirectoryName(instructorImagePath);
                    if (!Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);
                    using (FileStream stream = new FileStream(instructorImagePath, FileMode.Create))
                        await dto.image.CopyToAsync((Stream)stream);

                    imageUrl = instructorImagePath; 
                }
                catch (Exception ex)
                {
                    return Ok(new { status = 400, data = ex.Message });
                }
            }


            var instructor = new Instructor
            {
                name = dto.name,
                specialty = dto.specialty,
                description = dto.description,
                expertise = dto.expertise,
                imageUrl = imageUrl,
                facebookUrl = dto.facebookUrl ?? "",
                whatsappUrl = dto.whatsappUrl ?? "",
                instgramUrl = dto.instgramUrl ?? "",
                tag = dto.tag,
            };

            await _instructorRepository.AddAsync(instructor);

            return Ok(new { status = 200, data = instructor });
        }

        [HttpPost("EditInstructor")]
        public async Task<IActionResult> EditInstructor([FromForm] DtoInstructorAddEdit dto)
        {
            if (dto.id == 0 || dto.id == null)
                return Ok(new { status = 400 });

            var instructor = await _instructorRepository.GetByIdAsync((int)dto.id);
            if (instructor == null)
                return Ok(new { status = 400 });

            if (dto.name != "" && !dto.name.IsNullOrEmpty()) instructor.name = dto.name;
            if (dto.specialty != "" && !dto.specialty.IsNullOrEmpty()) instructor.specialty = dto.specialty;
            if (dto.description != "" && !dto.description.IsNullOrEmpty()) instructor.description = dto.description;
            if (dto.expertise != "" && !dto.expertise.IsNullOrEmpty()) instructor.expertise = dto.expertise;
            if (dto.facebookUrl != "" && !dto.facebookUrl.IsNullOrEmpty()) instructor.facebookUrl = dto.facebookUrl;
            if (dto.whatsappUrl != "" && !dto.whatsappUrl.IsNullOrEmpty()) instructor.whatsappUrl = dto.whatsappUrl;
            if (dto.instgramUrl != "" && !dto.instgramUrl.IsNullOrEmpty()) instructor.instgramUrl = dto.instgramUrl;
            if (dto.tag != "" && !dto.tag.IsNullOrEmpty()) instructor.tag = dto.tag;

            if (dto.image != null)
            {
                string imageUrl = "";

                string currentDirectory = Directory.GetCurrentDirectory();
                string instructorImagePath = Path.Combine(currentDirectory, "Media", "Images", "Instructors", dto.image.FileName);

                try
                {
                    string directoryName = Path.GetDirectoryName(instructorImagePath);
                    if (!Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);
                    using (FileStream stream = new FileStream(instructorImagePath, FileMode.Create))
                        await dto.image.CopyToAsync((Stream)stream);

                    imageUrl = instructorImagePath;
                }
                catch (Exception ex)
                {
                    return Ok(new { status = 400, data = ex.Message });
                }

                instructor.imageUrl = imageUrl;
            }

            await _instructorRepository.UpdateAsync(instructor);

            return Ok(new { status = 200, data = instructor });
        }

        [HttpGet("DeleteInstructor")]
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            var instructor = await _instructorRepository.GetByIdAsync(id);
            if (instructor == null)
                return Ok(new { status = 200 });
            
            await _instructorRepository.DeleteAsync(instructor);

            var instructors = await _instructorRepository.GetAllAsync();
            return Ok(new { status = 200, data = instructors });
        }
        #endregion


        private int getUserId()
        {
            if (string.IsNullOrEmpty(User.FindFirst("id")?.Value))
                return -1;

            var userId = int.Parse(User.FindFirst("id")?.Value);

            var user = _userRepository.FindBy(x => x.id == userId && !x.isDeleted).FirstOrDefault();
            if (user == null)
                return -1;

            return userId;
        }
    }
}
