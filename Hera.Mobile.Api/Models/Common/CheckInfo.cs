namespace Hera.Mobile.Api.Models.Common
{
    public class CheckInfoRequest : BaseRequest
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}