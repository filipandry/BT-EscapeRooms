using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT_EscapeRooms.Models
{
    public class Player : Character
    {
        public string Username { get; set; }
        public int Score { get; set; }

        public int HealingPotions { get; set; }
        public int ToxicPotions { get; set; }

        public Player()
        {
            Lives = 10;
            MinDamage = 1;
            MaxDamage = 5;
        }
    }
}
