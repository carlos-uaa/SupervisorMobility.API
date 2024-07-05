using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.IS
{
    public class DataPanel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DataPanelId { get; set; }

        public bool? IsActive { get; set; }

        //Formato tiene datos con tendencia a ser establecidos
        public int ItemOrder { get; set; }
        public string DataTitle { get; set; } = string.Empty;

        public ICollection<DataPanelSpecification>? Specifications { get; set; }
        = new List<DataPanelSpecification>();
    }
}
