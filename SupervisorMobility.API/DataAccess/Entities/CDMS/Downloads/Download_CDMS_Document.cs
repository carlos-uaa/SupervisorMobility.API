using SupervisorMobility.API.DataAccess.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.API.Entities.CDMS.Downloads
{
    public class Download_CDMS_Document
    {
        
        public string NameDocKey { get; set; }
        public string URL { get; set; }
    }
}
