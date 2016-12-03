using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PokerPrototype.Models
{
    public class SendMessageModel
    {
        public string toError { get; set; }
        public string messageError { get; set; }
        public SendMessageModel(int id, string to, string message)
        {
            toError = messageError = "";
            if (to.Length == 0)
            {
                toError = "Enter a recipient";
            }
            if (message.Length == 0)
            {
                messageError = "Enter a message";
            }
        }
    }
}