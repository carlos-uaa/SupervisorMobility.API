using SupervisorMobility.API.Entities.CDMS.Archives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities.CDMS
{
    public class CDMS_CCP_Archives
    {
        public bool success { get; set; }
        public List<CCPDocument> operation { get; set; } = new List<CCPDocument>();
        public string message { get; set; }
    }
}
