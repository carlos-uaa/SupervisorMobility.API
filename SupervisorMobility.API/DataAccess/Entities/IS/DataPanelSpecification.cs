using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.IS
{
    public class DataPanelSpecification
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DataPanelSpecificationId { get; set; }

        public bool? IsActive { get; set; }

        //Formato tiene datos con tendencia a ser establecidos
        public int ItemOrder { get; set; }
        public string DataSpecification { get; set; } = string.Empty;

        public int? DataPanelId {  get; set; }
        public DataPanel? DataPanel {  get; set; }

    }
}
