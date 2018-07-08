using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT_EscapeRooms.Models
{
    public class Score
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public Code.Difficulty Difficulty { get; set; }
        public DateTime Date { get; set; }
        public int Points { get; set; }
    }
}
