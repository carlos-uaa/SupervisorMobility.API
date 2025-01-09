using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.DataAccess.Entities.ILU;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class HCI
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HCIId { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public ICollection<HCITransaction>? Transactions { get; set; }
        public ICollection<HCICategory>? Categories { get; set; }
        public ICollection<UserCareerPath>? CareerPaths { get; set; }
        public ICollection<Commentary>? Commentaries { get; set; }
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
