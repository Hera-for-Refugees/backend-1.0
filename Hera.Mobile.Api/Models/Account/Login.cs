namespace Hera.Mobile.Api.Models.Account
{
    public class LoginRequest : BaseRequest
    {
        public string Mobile { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public int MemberId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}