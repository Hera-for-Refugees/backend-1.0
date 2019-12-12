namespace Hera.Mobile.Api.Models.Blog
{
    public class BlogListRequest : BaseRequest
    {
    }

    public class BlogItem
    {
        public string Title { get; set; }
        public string Date { get; set; }
        public string Picture { get; set; }
        public string Abstract { get; set; }
        public string Content { get; set; }
        public int Id { get; set; }
    }
}