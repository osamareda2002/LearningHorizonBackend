using LearningHorizon.Data;
using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearningHorizon.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<DtoGetUser>> SelectAllUsers()
        {
            var users = await _context.Users.Where(x => !x.isDeleted).AsNoTracking()
                .Select(u => new DtoGetUser
                {
                    id = u.id,
                    firstName = u.firstName,
                    lastName = u.lastName,
                    email = u.email,
                    country = u.country,
                    city = u.city,
                    university = u.university,
                    major = u.major,
                    acadimcYear = u.acadimcYear,
                    howDidYouKnowUs = u.howDidYouKnowUs,
                    graduationYear = u.graduationYear,
                    isAdmin = u.isAdmin,
                }).ToListAsync();
            return users;
        }

        public async Task<DtoGetUser> EditUser(DtoUpdateUser dtoUser)
        {
            var user = await GetByIdAsync(dtoUser.id);
            if (user != null)
            {
                user.firstName = dtoUser.firstName;
                user.lastName = dtoUser.lastName;
                user.country = dtoUser.country;
                user.city = dtoUser.city;
                user.university = dtoUser.university;
                user.major = dtoUser.major;
                user.acadimcYear = dtoUser.acadimcYear;
                user.howDidYouKnowUs = dtoUser.howDidYouKnowUs;
                user.graduationYear = dtoUser.graduationYear;
                await UpdateAsync(user);
            }

            var result = await GetUserById(dtoUser.id);
            return result;
        }

        public async Task<DtoGetUser> GetUserById(int id)
        {
            var user = await _context.Users
                .Where(u => u.id == id && !u.isDeleted)
                .Select(u => new DtoGetUser
                {
                    id = u.id,
                    firstName = u.firstName,
                    lastName = u.lastName,
                    email = u.email,
                    country = u.country,
                    city = u.city,
                    university = u.university,
                    major = u.major,
                    acadimcYear = u.acadimcYear,
                    howDidYouKnowUs = u.howDidYouKnowUs,
                    graduationYear = u.graduationYear,
                    isAdmin = u.isAdmin,
                    purchasedCourses = u.CoursesPurchased.Select(x => x.id).ToList()
                }).FirstOrDefaultAsync();
            return user;
        }

        public async Task AddPurchasedCourse(int courseId, int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.id == userId && x.isDeleted != true);
            if (user == null)
                return;

            var course = await _context.Courses.FirstOrDefaultAsync(x => x.id == courseId && x.isDeleted != true);
            if (course == null)
                return;

            user.CoursesPurchased.Add(course);

            await _context.SaveChangesAsync();
            return;
        }
    }
}
