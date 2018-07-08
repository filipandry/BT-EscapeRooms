using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT_EscapeRooms.Models
{
    public class ToxicPotion : Potion
    {
        public ToxicPotion()
        {
            CanSave = true;
        }
    }
}
