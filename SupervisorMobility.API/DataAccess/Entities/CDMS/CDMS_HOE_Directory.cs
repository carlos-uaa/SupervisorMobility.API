using SupervisorMobility.API.Entities.CDMS.Directory;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities.CDMS
{
    public class CDMS_HOE_Directory
    {
        public bool success { get; set; }
        public List<FolderHOE> operation { get; set; } = new List<FolderHOE>();
        public string message { get; set; }
    }
}
