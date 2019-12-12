namespace Hera.Mobile.Api.Models.Account
{
    public class ForgotPasswordRequest : BaseRequest
    {
        public string Mobile { get; set; }
    }

    public class ChangePasswordRequest : BaseRequest
    {
        public string Mobile { get; set; }
        public string SmsCode { get; set; }
        public string NewPassword { get; set; }
    }
}