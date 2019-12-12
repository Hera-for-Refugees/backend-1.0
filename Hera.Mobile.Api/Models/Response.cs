namespace Hera.Mobile.Api.Models
{
    public class Response<T>
    {
        /// <summary>
        /// Action has error ? True : False
        /// </summary>
        public bool HasError { get; set; }
        /// <summary>
        /// Error Detail
        /// </summary>
        public Error Error { get; set; }
        /// <summary>
        /// Response object
        /// </summary>
        public T Result { get; set; }
    }
}