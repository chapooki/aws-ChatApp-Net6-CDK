using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models
{
    public class ChatMessage
    {
       public string Sender { get; set; }
       public string Receiver { get; set; }
       public DateTime SendDateTime { get; set; }
       public DateTime? ReceivedDateTime { get; set; }
       public string Content { get; set; }
    }
}
