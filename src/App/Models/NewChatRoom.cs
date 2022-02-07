using System;
using System.Collections.Generic;

namespace App.Models
{
    public class NewChatRoom
    {
        public string Name { get; set; }
        public List<Guid> UserIds { get; set; }
    }
}
