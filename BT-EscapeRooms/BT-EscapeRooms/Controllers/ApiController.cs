using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BT_EscapeRooms.Code;
using BT_EscapeRooms.Data;
using BT_EscapeRooms.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BT_EscapeRooms.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class ApiController : Controller
    {
        ApplicationState state;
        EscapeRoomContext context;

        public ApiController(EscapeRoomContext _context, ApplicationState _state)
        {
            state = _state;
            context = _context;
        }
        [HttpGet("Scores")]
        public ActionResult GetScores(string username)
        {
            var scoresList = context.Scores.Where(w => string.IsNullOrEmpty(username) ? true : w.Username == username);

            return Json(scoresList);
        }

        [HttpPut("Username")]
        public ActionResult UpdateUsername(string username, string newUsername)
        {
            if (string.IsNullOrEmpty(username))
            {
                return NotFound();
            }
            var scores = context.Scores.Where(w => w.Username == username).ToList();
            foreach(var score in scores)
            {
                score.Username = newUsername;
            }
            var result = context.SaveChanges();
            return Json(result);
        }

        [HttpDelete("Scores")]
        public ActionResult DeleteScores(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return NotFound();
            }
            var scores = context.Scores.Where(w => w.Username == username).ToList();
            foreach (var score in scores)
            {
                context.Remove(score);
            }
            var result = context.SaveChanges();
            return Json(result);
        }

        [HttpGet("NewGame")]
        public ActionResult NewGame(string username, Code.Difficulty difficulty)
        {
            var userID = Guid.NewGuid();
            var map = new Map(username, difficulty);
            state.AddGame(userID, map);
            return Json(new { User = userID, Map = map.ToString(), FullMap = map });
        }
        [HttpPost("Move")]
        public ActionResult Move(Guid user, string direction)
        {
            var map = state.GetGame(user);
            if (map == null)
            {
                return NotFound();
            }
            map.Move(direction);
            return Json(new { User = user, Map = map.ToString(), FullMap = map });
        }
        [HttpPost("Attack")]
        public ActionResult Attack(Guid user)
        {
            var map = state.GetGame(user);
            if (map == null)
            {
                return NotFound();
            }
            map.PlayerAttack();
            map.MonsterAttack();
            if (map.CurrentAction == GameAction.Victory || map.CurrentAction == GameAction.GameOver)
            {//save score
                context.Add(new Score
                {
                    Date = DateTime.Now,
                    Difficulty = map.Difficulty,
                    Points = map.CurrentPlayer.Score,
                    Username = map.CurrentPlayer.Username
                });
                context.SaveChanges();
            }

            return Json(new { User = user, Map = map.ToString(), FullMap = map });
        }
        [HttpGet("Name")]
        public ActionResult Name()
        {
            return Json(HttpContext.User.Identity.IsAuthenticated);
        }
    }
}