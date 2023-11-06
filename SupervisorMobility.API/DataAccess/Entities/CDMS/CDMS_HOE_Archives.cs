using SupervisorMobility.API.Entities.CDMS.Archives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities.CDMS
{
    public class CDMS_HOE_Archives
    {
        public bool success { get; set; }
        public List<HOEDocument> operation { get; set; } = new List<HOEDocument>();
        public string message { get; set; }
    }
}
