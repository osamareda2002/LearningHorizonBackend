namespace LearningHorizon.Data.DTO
{
    public class DtoResetPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
