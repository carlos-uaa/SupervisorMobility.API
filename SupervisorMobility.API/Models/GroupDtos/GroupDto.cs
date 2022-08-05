namespace SupervisorMobility.API.Models.GroupDtos
{
    public class GroupDto
    {
        public int GroupId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
