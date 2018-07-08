using BT_EscapeRooms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT_EscapeRooms.Code
{
    public class ApplicationState
    {
        public object GameLock = new object();
        public Dictionary<Guid, Map> Games { get; set; }

        public void AddGame(Guid user,Map game)
        {
            lock (GameLock)
            {
                if(Games == null)
                {
                    Games = new Dictionary<Guid, Map>();
                }
                Games.Add(user, game);
            }
        }
        public void RemoveGame(Guid user)
        {
            lock (GameLock)
            {
                if(Games != null && Games.ContainsKey(user))
                {
                    Games.Remove(user);
                }
            }
        }
        public void UpdateGame(Guid user, Map game)
        {
            lock (GameLock)
            {
                if(Games != null && Games.ContainsKey(user))
                {
                    Games[user] = game;
                }
                else
                {
                    AddGame(user, game);
                }
            }
        }
        public Map GetGame(Guid user)
        {
            lock (GameLock)
            {
                if(Games != null && Games.ContainsKey(user))
                {
                    return Games[user];
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
