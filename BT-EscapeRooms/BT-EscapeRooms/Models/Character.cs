using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT_EscapeRooms.Models
{
    public abstract class Character : IGameObject
    {
        public int Lives { get; set; }
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }

        public virtual int Attack()
        {
            var rnd = new Random();
            return rnd.Next(MinDamage, MaxDamage + 1);
        }
    }
}
