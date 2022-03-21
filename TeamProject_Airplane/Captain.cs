using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProject_Airplane
{
    [Serializable]
    class Captain
    {
        public bool CaptainOrCocaptain { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        //public bool WorkAvailable { get; set; } //?
        public Captain() { }
        public Captain(bool captainOrCocaptain, string name, string phoneNo, string email)
        {
            this.CaptainOrCocaptain = captainOrCocaptain;
            this.Name = name;
            this.PhoneNo = phoneNo;
            this.Email = email;
        }
    }
}
