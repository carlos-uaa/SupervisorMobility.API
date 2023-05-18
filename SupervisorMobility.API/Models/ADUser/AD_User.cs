using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SupervisorMobility.API.Entities;

namespace SupervisorMobility.API.Models.ADUser
{
    public class AD_User
    {
        public string dn { get; set; }
        public string distinguishedName { get; set; }
        public string userPrincipalName { get; set; }
        public string sAMAccountName { get; set; }
        public string whenCreated { get; set; }
        public string pwdLastSet { get; set; }
        public string userAccountControl { get; set; }
        public string sn { get; set; }
        public string givenName { get; set; }
        public string cn { get; set; }
        public string displayName { get; set; }
    }
}
