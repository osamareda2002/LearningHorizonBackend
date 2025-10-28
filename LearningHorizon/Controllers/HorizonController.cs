using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;
using LearningHorizon.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MimeKit;
using System;
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
        private readonly JwtTokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private string baseUrl => $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        public HorizonController(IUserRepository userRepository, ICourseRepository courseRepository, ILessonRepository lessonRepository, ISliderRepository sliderRepository, ISuggestRepository suggestRepository, JwtTokenService tokenService, IOrderRepository orderRepository, IConfiguration configuration, IMemoryCache cache, IBookRepository bookRepository)
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
            var user = _userRepository.FindBy(x=>x.id ==id && x.isDeleted != true).FirstOrDefault();

            if(user  == null)
                return Ok(new {status = 400, message = "No user with this id" });
            else if(user.isOwner)
                return Ok(new { status = 400, message = "Can't Delete Owner" });

            user.isDeleted = true;
            await _userRepository.UpdateAsync(user);

            var result = await _userRepository.GetAllAsync();
            return Ok(new {status = 200, data = result });
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

            var contentType = _bookRepository.GetContentType(filePath);
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

            var contentType = _bookRepository.GetContentType(filePath);
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

        [Authorize]
        [HttpPost]
        [Route("AddNewCourse")]
        public async Task<IActionResult> AddNewCourse([FromForm]DtoAddCourse dtoCourse)
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
            };
            var addedCourse = await _courseRepository.AddAsync(course);

            foreach ( var u in await _userRepository.SelectAllUsers())
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
            
            if(dtoCourse.courseTitle!= null) course.title = dtoCourse.courseTitle;
            if(dtoCourse.courseCreator != null) course.creator = dtoCourse.courseCreator;
            if(dtoCourse.coursePrice != null) course.price = (decimal)dtoCourse.coursePrice;
            if(dtoCourse.courseImage != null)
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

            await _courseRepository.UpdateAsync(course);
            var result = await _courseRepository.SelectCourseById(course.id);
            return Ok(new { status = 200 , data = result });
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

        [HttpPost]
        [RequestSizeLimit(10737418240 /*0x0280000000*/)]
        [Route("AddLesson")]
        public async Task <IActionResult> AddLesson([FromForm] DtoAddLesson dtoLesson)
        {
            var course = await _courseRepository.GetByIdAsync(dtoLesson.courseId);
            if (course == null)
                return NotFound("No course with this id");

            string str = Path.Combine(Directory.GetCurrentDirectory(), "Media", "Courses", course.title);
            if (!Directory.Exists(str))
                Directory.CreateDirectory(str);

            string path = Path.Combine(str, dtoLesson.lessonFile.FileName);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            using (FileStream stream = new FileStream(path, FileMode.Create))
                await dtoLesson.lessonFile.CopyToAsync((Stream)stream);

            Lesson lesson = new Lesson
            {
                title = dtoLesson.title,
                isFree = dtoLesson.isFree,
                courseId = dtoLesson.courseId,
                path = path,
                duration = dtoLesson.durationInSeconds,
                lessonOrder = dtoLesson.lessonOrder
            };

            await _lessonRepository.AddAsync(lesson);
            var result = await _lessonRepository.SelectLessonById(lesson.id);
            return Ok(result);
        }

        [HttpPost]
        [RequestSizeLimit(10737418240 /*0x0280000000*/)]
        [Route("EditLesson")]
        public async Task <IActionResult> EditLesson([FromForm] DtoEditLesson dtoLesson)
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

        [HttpPost]
        [Route("DeleteLesson")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null)
                return NotFound("No lesson with this id");
            lesson.isDeleted = true;
            await _lessonRepository.UpdateAsync(lesson);
            return Ok("Lesson deleted successfully");
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

        [HttpDelete]
        [Route("DeleteSuggestVideo")]
        public async Task<IActionResult> DeleteSuggestVideo(int id)
        {
            _cache.Remove("all_suggestions");
            var suggest = await _suggestRepository.GetByIdAsync(id);
            if (suggest == null)
                return BadRequest("Invaild Id");

            suggest.isDeleted= true;
            await _suggestRepository.UpdateAsync(suggest);
            return Ok("Suggest deleted successfully");
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
            if(user.CoursesPurchased.Contains(course))
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
                        return Redirect($"http://localhost:4200/material");
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
    }
}
