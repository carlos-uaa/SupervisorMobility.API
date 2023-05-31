

using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.PATDtos
{
    public class PATDto
    {
        public int PATid { get; set; }

        public int SupervisorId { get; set; }

        public UsersWithNavigationDetails? Supervisor
        {
            get { return _supervisor; }
            set
            {
                _supervisor = value;
                if (_supervisor != null)
                {
                    SSVresponsibleID = _supervisor.SuperiorId;
                    AreaId = (int)_supervisor.AreaId;
                }
                else
                {
                    SSVresponsibleID = null;
                }
            }
        }
        private UsersWithNavigationDetails? _supervisor;

        public int? SSVresponsibleID { get; set; }
        public UsersWithNavigationDetails? SSVresponsible { get; set; }


        public int PlantId { get; set; }
        public PlantDto? Plant { get; set; }
        public int AreaId { get; set; }
        public AreaDtos.AreaWithoutNavigationPropertiesDto? Area { get; set; }

        public int DistributionId { get; set; }
        public DistributionDtos.DistributionWithoutNavigationPropertiesDto? Distribution { get; set; }



        public DateTime? AplicationDate { get; set; }
        public int? AplicationYear
        {
            get { return AplicationDate?.Year; }
            set { AplicationDate = value != null ? new DateTime(value.Value, 1, 1) : null; }
        }


        public DateTime? CreationDate { get; set; }

        public DateTime? EditionDate { get; set; }

        public DateTime? ApprovalDate { get; set; }
    }
}
