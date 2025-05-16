namespace SupervisorMobility.API.Models.ProductiveCalendarDtos
{
    public class HolidayForUpdateDto
    {
        public int HolidayId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool IsNationalHoliday { get; set; } // opcional, por si luego necesitas distinguir
        public bool IsActive { get; set; } = true;
    }
}
