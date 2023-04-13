using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.DataAccess.Entities
{
    public class ADuser
    {
        public string oid {  get; set; }
        public string name { get; set; }
        public string email { get; set; }
    }
}
