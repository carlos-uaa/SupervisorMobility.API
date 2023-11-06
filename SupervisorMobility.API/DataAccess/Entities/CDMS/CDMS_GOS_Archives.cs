using SupervisorMobility.API.Entities.CDMS.Archives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities.CDMS
{ 
    public class CDMS_GOS_Archives
    {
        public bool success { get; set; }
        public List<GOSDocument> operation { get; set; } = new List<GOSDocument>();
        public string message { get; set; }
    }
}
