namespace Hera.Core.Base
{
    public class LoggedUser
    {
        public bool IsMember { get; set; }
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Fullname { get { return this.Firstname + " " + this.Lastname; } }
        public string Email { get; set; }
    }
}
