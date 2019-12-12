namespace Hera.Mobile.Api.Models.Member
{
    public class HealthRecordRequest : BaseRequest
    {
        public int MemberId { get; set; }
    }

    public class HealthRecordItem
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Date { get; set; }
    }
}