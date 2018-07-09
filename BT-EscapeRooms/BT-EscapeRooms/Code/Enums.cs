using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT_EscapeRooms.Code
{
    public enum Difficulty
    {
        Easy=1,
        Normal=2,
        Hard=3,
    }

    public enum MapObject
    {
        Empty = 0,
        Player = 1,
        Monster = 2,
        BostMonster = 3,
        HealingPotion = 4,
        ToxicPotion = 5
    }

    public enum GameAction
    {
        None = 0,
        FightMonster = 1,
        FightBossMonster = 2,
        GameOver = 3,
        Victory = 4,
        AlreadyVisited = 5,
        PickHealingPotion = 6,
        PickToxicPotion = 7
    }
}
