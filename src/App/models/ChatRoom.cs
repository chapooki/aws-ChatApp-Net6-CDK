using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.models
{
    public class ChatRoom
    {
        public Guid Id { get; set; }    
        public string Name { get; set; }
        public DateTime LatestMessageDateTime { get; set; }
        public List<Guid> UserIds { get; set; }
    }
}
