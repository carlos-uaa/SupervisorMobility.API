using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.Models.ADUser
{
    public class LoginResponse
    {
        public AD_User response { get; set; }
    }
}
