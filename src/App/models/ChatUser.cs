using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.models
{
    public class ChatUser
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public List<Guid> RoomIds { get; set; }
    }
}
