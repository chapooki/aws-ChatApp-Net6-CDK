using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.models
{
    public class MyRequest
    {
        public string RequestType { set; get; }
        public ChatMessage ChatMessage { set; get; }
    }
}
