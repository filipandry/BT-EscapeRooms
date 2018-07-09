using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT_EscapeRooms.Models
{
    public class BossMonster : Monster
    {
        public BossMonster()
        {
            ScoreValue = 10;
            Lives = 5;
            MinDamage = 1;
            MaxDamage = 5;
        }
    }
}
