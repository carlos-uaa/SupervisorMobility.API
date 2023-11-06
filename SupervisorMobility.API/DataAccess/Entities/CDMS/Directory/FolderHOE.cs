using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities.CDMS.Directory
{
    public class FolderHOE
    {
        public int ID_Carpeta { get; set; }
        public string Nombre { get; set; }
        public int ID_Carpeta_Superior { get; set; }
        public string ruta { get; set; }
        public bool Directory { get; set; }
    }
}
