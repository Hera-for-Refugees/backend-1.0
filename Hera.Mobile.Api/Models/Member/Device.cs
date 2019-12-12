namespace Hera.Mobile.Api.Models.Member
{
    public class SaveDeviceRequest:BaseRequest
    {
        public int MemberId { get; set; }
        public string Platform { get; set; }
        public string NotificationUserId { get; set; }
        public string DeviceToken { get; set; }
    }
}