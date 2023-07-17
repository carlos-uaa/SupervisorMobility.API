using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Models.SOSReviewDtos
{
    public class SOSReviewForCreateDto
    {
        public int Status { get; set; }

        public int? UserAid { get; set; }

        public int? UserBid { get; set; }

        public int? UserCid { get; set; }


        public int? PlantId { get; set; }


        public int? AreaId { get; set; }




        public DateTime? CreationDate { get; set; }
        public int? AplicationYear
        {
            get { return CreationDate?.Year; }
            set
            {
                if (value != null)
                {
                    int year = value.Value;
                    if (!(year >= 1 && year <= 9999))
                    {
                       
                        AplicationYear = CreationDate?.Year;
                    }
                }
                else
                {
                    AplicationYear = null;
                }
            }

        }


        public DateTime? EditionDate { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public bool IsActive { get; set; }
    }
}
