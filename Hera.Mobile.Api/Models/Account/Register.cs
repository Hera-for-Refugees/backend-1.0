namespace Hera.Mobile.Api.Models.Account
{
    public class RegisterRequest : BaseRequest
    {
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string Photo { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Birthdate { get; set; }
        public string Gender { get; set; }
        public string Job { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Platform { get; set; }
    }

    public class RegisterResponse
    {
        public int MemberId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }

    
}