namespace Hera.Mobile.Api.Models
{
    public class Error
    {
        /// <summary>
        /// Error title, for multi language
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Error message, for multi language
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Error constructor
        /// </summary>
        /// <param name="title">String error title</param>
        /// <param name="message">String error message</param>
        public Error(string title, string message)
        {
            this.Title = title;
            this.Message = message;
        }
    }
}