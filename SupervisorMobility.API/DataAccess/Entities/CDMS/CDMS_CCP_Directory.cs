using SupervisorMobility.API.Entities.CDMS.Directory;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities.CDMS { 
    public class CDMS_CCP_Directory
    {
        public bool success { get; set; }
        public List<FolderCCP> operation { get; set; } = new List<FolderCCP>();
        public string message { get; set; }
    }
}
