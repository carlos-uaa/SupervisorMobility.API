using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities.CDMS.Directory
{
    public class FolderGOS
    {
        public string Nombre { get; set; }
        public string ruta { get; set; }
        public bool Directory { get; set; }
    }
}
