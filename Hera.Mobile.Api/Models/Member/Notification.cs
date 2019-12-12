namespace Hera.Mobile.Api.Models.Member
{
    public class NotificationListRequest : BaseRequest
    {
        public int MemberId { get; set; }
    }

    public class NotificationItem
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Date { get; set; }
        public bool IsRead { get; set; }
        public string Type { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class NotificationSaveRequest : BaseRequest
    {
        public int MemberId { get; set; }
        public int MessageId { get; set; }
        public string Type { get; set; }
    }
}