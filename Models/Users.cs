using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerRepo.Models
{
    public class Users
    {
        public int id { get; set; }
        public string name { get; set; }

        public string email { get; set; }   

        public Users(int id, string name, string email)
        {
            this.id = id;
            this.name = name;
            this.email = email;
        }
    }
}
