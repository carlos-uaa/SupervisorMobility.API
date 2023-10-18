namespace SupervisorMobility.API.Models.Users
{
    public class UserNotFoundWithNavigationDetails
    {
        public int UserNotFoundId { get; set; }
        public string? ObjectId { get; set; }
        public string? Name { get; set; }
        public bool? IsActive { get; set; }

    }
}
