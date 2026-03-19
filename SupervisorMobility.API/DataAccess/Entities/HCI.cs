using SupervisorMobility.API.DataAccess.Entities.ILU;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class HCI
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HCIId { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }
        public int? SOSHubId { get; set; }
        public SOSHub? SOSHub { get; set; }
        public ICollection<HCITransaction>? Transactions { get; set; }
        public ICollection<HCICategory>? Categories { get; set; }
        public ICollection<UserCareerPath>? CareerPaths { get; set; }
        public ICollection<Commentary>? Commentaries { get; set; }
        public ICollection<LocalUserCourses>? Courses { get; set; }
        public bool? IsActive { get; set; }

        public ICollection<ILURegister>? ILUs
        {
            get => User?.ILURegisers;
            set
            {
                if (User != null)
                {
                    User.ILURegisers = value;
                }
            }
        }
    }
}
