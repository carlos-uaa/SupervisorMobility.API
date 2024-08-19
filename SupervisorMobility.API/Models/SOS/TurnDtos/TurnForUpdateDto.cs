using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.Models.SOS.TurnDtos
{
    public class TurnForUpdateDto
    {
        public int TurnId { get; set; }

        public string? TurnType { get; set; }

        public int? OperatorId { get; set; }

        public int? SupervisorId { get; set; }
    }
}
