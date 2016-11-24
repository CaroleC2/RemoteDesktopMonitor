using System;
using System.Collections.Generic;
using System.Dynamic;
using System.EnterpriseServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDMService
{
    internal class Value
    {
        
            internal Value(string password)
            {
                Password = password;
            }

            internal string Password { get; private set; }

            internal object Data { get; set; }
      }

}
