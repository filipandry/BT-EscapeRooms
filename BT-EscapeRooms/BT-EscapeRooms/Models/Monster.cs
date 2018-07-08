using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT_EscapeRooms.Models
{
    public class Monster : Character
    {
        public int ScoreValue { get; set; }
        public Monster()
        {
            ScoreValue = 5;
            Lives = 5;
            MinDamage = 1;
            MaxDamage = 3;
        }
    }
}
