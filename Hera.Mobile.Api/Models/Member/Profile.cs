namespace Hera.Mobile.Api.Models.Member
{
    public class ProfileRequest : BaseRequest
    {
        public int MemberId { get; set; }
    }

    public class ProfileResponse
    {
        public int MemberId { get; set; }
        public string ProfilePhoto { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Birthdate { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }        
        public string Mobile { get; set; }
        public string Job { get; set; }
        public string Address { get; set; }
        public string Lang { get; set; }
    }
}