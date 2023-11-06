using SupervisorMobility.API.Entities.CDMS.Downloads;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities.CDMS
{
    public class CDMS_DownloadFile
    {
        public bool success { get; set; }
        public Download_CDMS_Document operation { get; set; } = new Download_CDMS_Document();
        public string message { get; set; }
    }
}
