using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models
{
    public class ChatMessage
    {
       public Guid Id { get; set; }
       public Guid SenderUserId { get; set; }
       public Guid RoomId { get; set; }
       public DateTime SendDateTime { get; set; }
       public string Content { get; set; }
    }
}
