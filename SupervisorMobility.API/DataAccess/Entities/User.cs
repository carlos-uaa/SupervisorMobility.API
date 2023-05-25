using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.Entities;
using System.Text.Json.Serialization;
using SupervisorMobility.API.DataAccess.Entities.ILU;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string? ObjectId { get; set; }
        public int? Payroll { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public int UserType { get; set; }

        public int? SuperiorId { get; set; }
        [ForeignKey("SuperiorId")]
        [JsonIgnore]
        public User? Superior { get; set; }
        [InverseProperty("Superior")]
        public ICollection<User>? Subordinates { get; set; }

        public ICollection<ILUOperatorRegister>? ILURegisers { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        [Column(TypeName = "Date")]
        public DateTime LastUpdated { get; set; } 
        [Column(TypeName = "Date")]
        public DateTime? DisabledDate { get; set; }

        public bool? IsActive { get; set; }

        public int? PlantId { get; set; }
        [ForeignKey("PlantId")]

        public Plant? Plant { get; set; }
        public int? AreaId { get; set; }
        [ForeignKey("AreaId")]
        public Area? Area { get; set; }

        [NotMapped]
        public ICollection<Area>? Areas { get; set; }

        public int? DistributionId { get; set; }
        public Distribution? Distribution { get; set; }

        public int? GroupId { get; set; }
        public Group? Group { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            User other = (User)obj;

            if (UserType != other.UserType)
            {
                return false;
            }

            switch (UserType)
            {
               
                case 2:
                    return Name == other.Name &&
                           Email == other.Email &&
                           PlantId == other.PlantId &&
                           GroupId == other.GroupId;
                case 3:
                    return Name == other.Name &&
                           Email == other.Email &&
                           SuperiorId == other.SuperiorId &&
                           AreaId == other.AreaId;
                case 4:
                    return Payroll == other.Payroll &&
                           Name == other.Name &&
                           SuperiorId == other.SuperiorId &&
                           DistributionId == other.DistributionId;
                default:
                    return false;
            }
        }


        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                switch (UserType)
                {
                    case 4:
                        hash = hash * 23 + Payroll.GetHashCode();
                        hash = hash * 23 + (Name != null ? Name.GetHashCode() : 0);
                        hash = hash * 23 + (SuperiorId != null ? SuperiorId.GetHashCode() : 0);
                        hash = hash * 23 + (DistributionId != null ? DistributionId.GetHashCode() : 0);
                        break;
                    case 2:
                        hash = hash * 23 + (Name != null ? Name.GetHashCode() : 0);
                        hash = hash * 23 + (Email != null ? Email.GetHashCode() : 0);
                        hash = hash * 23 + (PlantId != null ? PlantId.GetHashCode() : 0);
                        hash = hash * 23 + (AreaId != null ? AreaId.GetHashCode() : 0);
                        hash = hash * 23 + (GroupId != null ? GroupId.GetHashCode() : 0);
                        break;
                    case 3:
                        hash = hash * 23 + (Name != null ? Name.GetHashCode() : 0);
                        hash = hash * 23 + (Email != null ? Email.GetHashCode() : 0);
                        hash = hash * 23 + (SuperiorId != null ? SuperiorId.GetHashCode() : 0);
                        hash = hash * 23 + (AreaId != null ? AreaId.GetHashCode() : 0);
                        break;
                }

                return hash;
            }
        }
    }
}
