using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerRepo.Models
{
    [AttributeUsage(AttributeTargets.All)]
    public class MyAttribute : Attribute
    {
        private string myName;
        public MyAttribute(string name)
        {
            myName = name;
        }
        public string Name
        {
            get
            {
                return myName;
            }
        }
    }
}
