namespace Hera.Mobile.Api.Models.Blog
{
    public class BlogLogRequest 
    {
        public int MemberId { get; set; }
        public int ContentId { get; set; }
        public int LogId { get; set; }
        public string Type { get; set; }
    }
}