using LearningHorizon.Data;
using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LearningHorizon.Repositories
{
    public class CourseCategoryRepository : GenericRepository<CourseCategory> , ICourseCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseCategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<DtoGetCategory>> GetAllCategories(string baseUrl)
        {
            var list = await (from q in _context.CourseCategories.AsNoTracking()
                              where q.isDeleted != true
                              select new DtoGetCategory
                              {
                                  id = q.id,
                                  title = q.title,
                                  about = q.about,
                                  imageUrl = q.imageUrl.IsNullOrEmpty() ? "" :  $"{baseUrl}/Media/Images/Categories/{Path.GetFileName(q.imageUrl)}",
                                  courses = q.courses.Where(x => x.isDeleted != true).Select(x => new DtoGetCourse
                                  {
                                      courseId = x.id,
                                      courseTitle = x.title,
                                      courseCreator = x.creator,
                                      coursePrice = x.price,
                                      lessonsCount = x.Lessons.Count,
                                      courseDurationInSeconds = x.Lessons.Sum(l => l.duration ?? 0)
                                  }).ToList()
                              }).ToListAsync();

            return list;
        }

        public async Task<List<DtoGetCategory>> AddCategory(DtoAddEditCategory dto, string baseUrl)
        {
            string imageUrl = "";
            if (dto.image != null)
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                string categoryImagePath = Path.Combine(currentDirectory, "Media", "Images", "Categories", dto.image.FileName);

                try
                {
                    string directoryName = Path.GetDirectoryName(categoryImagePath);
                    if (!Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);
                    using (FileStream stream = new FileStream(categoryImagePath, FileMode.Create))
                        await dto.image.CopyToAsync((Stream)stream);

                    imageUrl = categoryImagePath;
                }
                catch (Exception ex)
                {
                    
                }
            }

            var category = new CourseCategory
            {
                title = dto.title,
                about = dto.about,
                imageUrl = imageUrl,
                createdTime = DateTime.UtcNow,
                isDeleted = false
            };
            await AddAsync(category);

            var categories = await GetAllCategories(baseUrl);
            return categories;
        }

        public async Task<List<DtoGetCategory>> EditCategory(DtoAddEditCategory dto, string baseUrl)
        {
            var category = await GetByIdAsync((int)dto.id);

            if (category != null && category.isDeleted != true)
            {
                if (dto.title != ""  && !dto.title.IsNullOrEmpty()) category.title = dto.title;
                if (dto.about != ""  && !dto.about.IsNullOrEmpty()) category.about = dto.about;
                if (dto.image != null)
                {
                    string currentDirectory = Directory.GetCurrentDirectory();
                    string categoryImagePath = Path.Combine(currentDirectory, "Media", "Images", "Categories", dto.image.FileName);

                    try
                    {
                        string directoryName = Path.GetDirectoryName(categoryImagePath);
                        if (!Directory.Exists(directoryName))
                            Directory.CreateDirectory(directoryName);
                        using (FileStream stream = new FileStream(categoryImagePath, FileMode.Create))
                            await dto.image.CopyToAsync((Stream)stream);

                        category.imageUrl = categoryImagePath;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                await UpdateAsync(category);
            }

            var categories = await GetAllCategories(baseUrl);
            return categories;
        }

        public async Task<List<DtoGetCategory>> DeleteCategory(int id, string baseUrl)
        {
            var category = await GetByIdAsync(id);
            if (category != null && category.isDeleted != true)
            {
                category.isDeleted = true;
                await UpdateAsync(category);
            }
            var categories = await GetAllCategories(baseUrl);
            return categories;

        }


    }
}
