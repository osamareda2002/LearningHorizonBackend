using LearningHorizon.Data;
using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;
using Microsoft.AspNetCore.Routing.Tree;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace LearningHorizon.Repositories
{
    public class MeetingRepository : GenericRepository<Meeting>, IMeetingRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _http;

        public MeetingRepository(ApplicationDbContext context, IConfiguration configuration, HttpClient http) : base(context)
        {
            _context = context;
            _configuration = configuration;
            _http = http;
        }


        public async Task<object> AddNewMeeting(DtoAddNewMeeting dto)
        {
            try
            {
                var token = await GetAccessTokenAsync();

                var meetingPayload = new
                {
                    topic = dto.topic,
                    type = 2, // Scheduled meeting
                    start_time = dto.startTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    duration = dto.durationInMinutes ?? 1440,
                    timeZone = "UTC",
                    settings = new
                    {
                        host_video = true,
                        participant_video = true,
                        join_before_host = false
                    }
                };

                var request = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["Zoom:baseUrl"]}/users/me/meetings");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var jsonPayload = JsonSerializer.Serialize(meetingPayload);
                request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var resp = await _http.SendAsync(request);
                var body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    throw new Exception($"Zoom Create Meeting failed. Status: {resp.StatusCode}, Body: {body}");
                }

                var zoomResponse = JsonSerializer.Deserialize<ZoomMeetingResponse>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (zoomResponse == null)
                    throw new Exception("Failed to parse Zoom response.");

                var meeting = new Meeting
                {
                    meetingId = zoomResponse.id,
                    topic = zoomResponse.topic,
                    startTime = dto.startTime.AddHours(2),
                    hostId = dto.hostId,
                    hostEmail = dto.hostEmail,
                    createdAt = zoomResponse.created_at,
                    startUrl = zoomResponse.start_url,
                    joinUrl = zoomResponse.join_url,
                    passCode = zoomResponse.password,
                    numericPassword = zoomResponse.h323_password
                };

                await AddAsync(meeting);

                return new { status = 200, data = new 
                {
                    meetingId = meeting.meetingId,
                    topic = meeting.topic,
                    startTime = meeting.startTime,
                    hostId = dto.hostId,
                    hostEmail = dto.hostEmail,
                    createdAt = meeting.createdAt,
                    startUrl = meeting.startUrl,
                    joinUrl = meeting.joinUrl,
                    passCode = meeting.passCode,
                    numericPassword = meeting.numericPassword
                } };
            }
            catch (Exception ex)
            {
                return new {status = 400, data = ex.Message};
                throw;
            }
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var clientId = _configuration["Zoom:clientId"]?.Trim();
            var clientSecret = _configuration["Zoom:clientSecret"]?.Trim();
            var accountId = _configuration["Zoom:accountId"]?.Trim();

            var tokenUrl = $"https://zoom.us/oauth/token?grant_type=account_credentials&account_id={accountId}";

            var authString = $"{clientId}:{clientSecret}";
            var authBytes = Encoding.UTF8.GetBytes(authString);
            var base64Auth = Convert.ToBase64String(authBytes);

            var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Auth);

            var resp = await _http.SendAsync(request);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                throw new Exception($"Zoom Token request failed: {resp.StatusCode}, Body: {body}");
            }

            var json = JsonDocument.Parse(body);
            var accessToken = json.RootElement.GetProperty("access_token").GetString()!;
            var expiresIn = json.RootElement.GetProperty("expires_in").GetInt32();

            return accessToken;
        }

        public async Task<List<DtoGetMeetingInfo>> DtoGetAllMeetingsInfo()
        {
            var meetings = await (from q in _context.Meetings.AsNoTracking()
                            
                            join u in _context.Users
                            on q.hostId equals u.id
                            into hostInfo
                            from meetingHost in hostInfo.DefaultIfEmpty()

                            where q.isDeleted != true && q.isFinished != true
                            select new DtoGetMeetingInfo
                            {
                                id = q.id,
                                meetingId = q.meetingId,
                                topic = q.topic,
                                startTime = q.startTime,
                                hostName = meetingHost != null ? $"{meetingHost.firstName} {meetingHost.lastName}" : null,
                                startUrl = q.startUrl,
                                joinUrl = q.joinUrl,
                                adminJoined = q.adminJoined,
                                isFinished = q.isFinished
                            }).OrderBy(x=>x.startTime).ToListAsync();

            return meetings;
        }

    }
}
