using DocumentFormat.OpenXml.Drawing.Charts;
using SupervisorMobility.API.DataAccess.Entities.ILU;
using SupervisorMobility.API.DataAccess.Entities.SOS_Review;
using SupervisorMobility.API.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        
        [JsonIgnore]
        public virtual ICollection<SOSReviewProgram>? SOSReviewPrograms { get; set; }
            = new List<SOSReviewProgram>();

        public ICollection<ILURegister>? ILURegisers { get; set; }
        public ICollection<UserCareerPath>? UserCareerPaths { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        [Column(TypeName = "Date")]
        public DateTime LastUpdated { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? DisabledDate { get; set; }
        [Column(TypeName = "Date")]

        public DateTime? IncomesDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? BirthDate { get; set; }
        public int? ProfilePictureId { get; set; }
        [ForeignKey("ProfilePictureId")]
        public FileUpload? ProfilePicture { get; set; } 

        public bool? IsActive { get; set; }

        public int? PlantId { get; set; }
        [ForeignKey("PlantId")]

        public Plant? Plant { get; set; }
        public int? AreaId { get; set; }
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
                case 1:
                case 6:
                    return Name == other.Name &&
                           ObjectId == other.ObjectId &&
                           Email == other.Email;
                case 2:
                    return Name == other.Name &&
                          ObjectId == other.ObjectId &&
                          Email == other.Email &&
                          SuperiorId == other.SuperiorId &&
                          PlantId == other.PlantId &&
                           GroupId == other.GroupId;
                case 3:
                    return Name == other.Name &&
                           ObjectId == other.ObjectId &&
                           Email == other.Email &&
                           SuperiorId == other.SuperiorId &&
                           PlantId == other.PlantId &&
                           AreaId == other.AreaId &&
                           GroupId == other.GroupId;
                case 4:
                    return Payroll == other.Payroll &&
                           Name == other.Name &&
                           SuperiorId == other.SuperiorId &&
                           PlantId == other.PlantId &&
                           AreaId == other.AreaId &&
                           DistributionId == other.DistributionId &&
                           GroupId == other.GroupId;
                case 5:
                    return Name == other.Name &&
                          ObjectId == other.ObjectId &&
                          Email == other.Email &&
                          PlantId == other.PlantId;
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

                    case 1:
                    case 6:
                        hash = hash * 23 + (Name != null ? Name.GetHashCode() : 0);
                        hash = hash * 23 + (ObjectId != null ? ObjectId.GetHashCode() : 0);
                        hash = hash * 23 + (Email != null ? Email.GetHashCode() : 0);
                        break;
                    case 2:
                        hash = hash * 23 + (Name != null ? Name.GetHashCode() : 0);
                        hash = hash * 23 + (ObjectId != null ? ObjectId.GetHashCode() : 0);
                        hash = hash * 23 + (Email != null ? Email.GetHashCode() : 0);
                        hash = hash * 23 + (SuperiorId!= null ? SuperiorId.GetHashCode() : 0);
                        hash = hash * 23 + (PlantId != null ? PlantId.GetHashCode() : 0);
                        hash = hash * 23 + (GroupId != null ? GroupId.GetHashCode() : 0);
                        break;
                    case 3:

                        hash = hash * 23 + (Name != null ? Name.GetHashCode() : 0);
                        hash = hash * 23 + (ObjectId != null ? ObjectId.GetHashCode() : 0);
                        hash = hash * 23 + (Email != null ? Email.GetHashCode() : 0);
                        hash = hash * 23 + (SuperiorId != null ? SuperiorId.GetHashCode() : 0);
                        hash = hash * 23 + (PlantId != null ? PlantId.GetHashCode() : 0);
                        hash = hash * 23 + (AreaId != null ? AreaId.GetHashCode() : 0);
                        hash = hash * 23 + (GroupId != null ? GroupId.GetHashCode() : 0);
                        break;
                       
                    case 4:
                        hash = hash * 23 + Payroll.GetHashCode();
                        hash = hash * 23 + (Name != null ? Name.GetHashCode() : 0);
                        hash = hash * 23 + (SuperiorId != null ? SuperiorId.GetHashCode() : 0);
                        hash = hash * 23 + (PlantId != null ? PlantId.GetHashCode() : 0);
                        hash = hash * 23 + (AreaId != null ? AreaId.GetHashCode() : 0);
                        hash = hash * 23 + (DistributionId != null ? DistributionId.GetHashCode() : 0);
                        hash = hash * 23 + (GroupId != null ? GroupId.GetHashCode() : 0);
                        break;
                    case 5:
                        hash = hash * 23 + (Name != null ? Name.GetHashCode() : 0);
                        hash = hash * 23 + (ObjectId != null ? ObjectId.GetHashCode() : 0);
                        hash = hash * 23 + (Email != null ? Email.GetHashCode() : 0);
                        hash = hash * 23 + (PlantId != null ? PlantId.GetHashCode() : 0);
                        break;
                }

                return hash;
            }
        }
    }
}
