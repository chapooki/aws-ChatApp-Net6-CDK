﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models
{
    public class NewChatMessage
    {
        public Guid RoomId { get; set; }
        public string Content { get; set; }
    }
}
