using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;

namespace LearningHorizon.Interfaces
{
    public interface IMeetingRepository : IGenericRepository<Meeting>
    {
        Task<object> AddNewMeeting(DtoAddNewMeeting dto);
        Task<List<DtoGetMeetingInfo>> DtoGetAllMeetingsInfo();
    }
}
