namespace mvc_pets.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Content { get; set; }
        public DateTime SentDate { get; set; }
    }

}
