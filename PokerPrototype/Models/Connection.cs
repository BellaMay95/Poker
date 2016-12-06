using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PokerPrototype.Models
{
    public class Connection
    {
        public static string Str
        {
            get
            {
                return "server=sql9.freemysqlhosting.net;database=sql9140372;user=sql9140372;password=WSx2C8iRZx;";
                //alternate database below. Have not set up tables yet
                //return "server=sql9.freemysqlhosting.net;database=sql3148156;user=sql3148156;password= 5jSMq4ea5u;";
            }
            set { }
        }
    }
}