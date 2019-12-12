namespace Hera.Mobile.Api.Models.Member
{
    public class ChildRequest : BaseRequest
    {
        public int MemberId { get; set; }
    }

    public class ChildItem
    {
        public int Id { get; set; }
        public string NameSurname { get; set; }
    }
}