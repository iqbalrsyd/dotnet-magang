namespace be_magang.Models
{
    public class Profile
    {
        public string Id { get; set; }
        public string Bio { get; set; }
        public string PictureUrl { get; set; }

        public int UserId { get; set; }
        public User User{ get; set; }
    }
}