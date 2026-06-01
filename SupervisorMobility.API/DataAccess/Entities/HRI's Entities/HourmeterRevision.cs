namespace SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities
{
    public class HourmeterRevision
    {
        public int Id {  get; set; }
        public int? HriId { get; set; }
        public HRI? HRI { get; set; }
        public bool? IsActive { get; set; }
        public List<DailyRevisions>? DailyRevisions { get; set; }

    }
}
