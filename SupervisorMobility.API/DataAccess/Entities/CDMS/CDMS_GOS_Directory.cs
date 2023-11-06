using SupervisorMobility.API.Entities.CDMS.Directory;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities.CDMS
{
    public class CDMS_GOS_Directory
    {
        public bool success { get; set; }
        public List<FolderGOS> operation { get; set; } = new List<FolderGOS>();
        public string message { get; set; }
    }
}
