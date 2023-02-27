using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static ClosedXML.Excel.XLPredefinedFormat;
using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public int Nomina { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public bool? IsActive { get; set; }

        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }
        public int? AreaId { get; set; }
        public Area? Area { get; set; }  
        public int? GroupId { get; set; }
        public Group? Group { get; set; }

       
    }
}
