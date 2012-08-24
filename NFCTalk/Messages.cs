using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFCTalk
{
    public class Message
    {
        public enum DirectionValue
        {
            In = 0,
            Out = 1
        }

        public DirectionValue Direction { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public bool Archived { get; set; }
    }
}
