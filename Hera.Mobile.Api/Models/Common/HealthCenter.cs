namespace Hera.Mobile.Api.Models.Common
{
    public class HealthCenterRequest : BaseRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class HealthCenter
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Distance { get; set; }
    }
}