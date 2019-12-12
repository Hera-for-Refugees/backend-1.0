namespace Hera.Mobile.Api.Models.Member
{
    public class CalendarRequest : BaseRequest
    {
        public int MemberId { get; set; }
    }

    public class UpdateCalendarRequest : BaseRequest
    {
        public int MemberId { get; set; }
        public int RecordId { get; set; }
        public string Type { get; set; }
    }
}