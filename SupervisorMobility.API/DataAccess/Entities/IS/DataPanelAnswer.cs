using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.API.DataAccess.Entities.IS
{
    public class DataPanelAnswer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DataPanelAnswerId { get; set; }

        public bool? IsActive { get; set; }

        //Aun por definir cual es el contenido de la casilla
        public string Result { get; set; } = string.Empty;
        public int? LogbookId { get; set; }
        public LogbookAparence? Logbook { get; set; }
       
        public int? DataPanelSpecificationId { get; set; }
        public DataPanelSpecification? DataPanelSpecification { get; set; }


    }
}
