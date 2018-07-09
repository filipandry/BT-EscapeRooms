using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BT_EscapeRooms.Models
{
    public class Map
    {
        public string Username { get; set; }
        public Code.Difficulty Difficulty { get; set; }

        public int Rows { get; set; }
        public int Columns { get; set; }
        public int NumberOfMonsters { get; set; }
        public int NumberOfHealingPotions { get; set; }
        public int NumberOfToxicPotions { get; set; }

        public IGameObject[,] GameMap { get; set; }
        public Code.GameAction CurrentAction { get; set; }
        public Player CurrentPlayer { get; set; }
        public Character CurrentMonster { get; set; }

        public Map(string username, Code.Difficulty difficulty)
        {
            Username = username;
            Difficulty = difficulty;
            switch (Difficulty)
            {
                case Code.Difficulty.Easy:
                    SetEasyGame();
                    break;
                case Code.Difficulty.Normal:
                    SetNormalGame();
                    break;
                case Code.Difficulty.Hard:
                    SetHardGame();
                    break;
            }
            GenerateMap();
        }

        public void SetEasyGame()
        {
            Rows = 3;
            Columns = 3;
            NumberOfMonsters = 2;
            NumberOfHealingPotions = 1;
            NumberOfToxicPotions = 0;
        }

        private void SetNormalGame()
        {
            Rows = 4;
            Columns = 4;
            NumberOfMonsters = 5;
            NumberOfHealingPotions = 2;
            NumberOfToxicPotions = 1;
        }

        private void SetHardGame()
        {
            Rows = 5;
            Columns = 5;
            NumberOfMonsters = 8;
            NumberOfHealingPotions = 4;
            NumberOfToxicPotions = 2;
        }

        public void GenerateMap()
        {
            GameMap = new IGameObject[Rows, Columns];

            var objects = new List<IGameObject>();
            objects.AddRange(Enumerable.Range(0, NumberOfMonsters).Select(x => new Monster()));
            objects.AddRange(Enumerable.Range(0, NumberOfHealingPotions).Select(x => new HealingPotion()));
            objects.AddRange(Enumerable.Range(0, NumberOfToxicPotions).Select(x => new ToxicPotion()));

            var emptySpaces = (Rows * Columns) - 2 - NumberOfMonsters - NumberOfHealingPotions - NumberOfToxicPotions;//get total of empty spaces on map
            objects.AddRange(Enumerable.Range(0, emptySpaces).Select<int, IGameObject>(x => null));

            var rnd = new Random();

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if(row == Rows-1 && col == 0)
                    {//Player position
                        CurrentPlayer = new Player();
                        CurrentPlayer.Username = Username;
                        CurrentPlayer.Score = 1;
                        GameMap[row, col] = CurrentPlayer;

                    }
                    else if(row == Rows - 1 && col == Columns - 1)
                    {//The Boss position
                        GameMap[row, col] = new BossMonster();
                    }
                    else
                    {
                        var index = rnd.Next(0, objects.Count - 1);
                        var obj = objects[index];
                        objects.RemoveAt(index);
                        GameMap[row, col] = obj;
                    }
                }
            }
        }
        public void PlayerAttack()
        {
            if (CurrentMonster != null)
            {
                var damage = CurrentPlayer.Attack();
                CurrentMonster.Lives -= damage;
                if (CurrentMonster.Lives <= 0)
                {
                    CurrentPlayer.Score += ((Monster)CurrentMonster).ScoreValue;
                    CurrentMonster = null;
                    CurrentAction = Code.GameAction.None;
                }
            }
            CheckStatus();
        }
        public void MonsterAttack()
        {
            if(CurrentMonster != null)
            {
                var damage = CurrentMonster.Attack();
                CurrentPlayer.Lives -= damage;
                if(CurrentPlayer.Lives <= 0)
                {
                    CurrentPlayer.Lives = 0;
                    CurrentAction = Code.GameAction.GameOver;
                }
            }
            CheckStatus();
        }

        public void Move(string direction)
        {
            if(CurrentMonster != null)
            {
                if(CurrentMonster.Lives > 0)
                {// monster still alive. cannot move
                    return;
                }
            }

            switch (direction)
            {
                case "N":
                    MoveNorth();
                    break;
                case "W":
                    MoveWest();
                    break;
                case "E":
                    MoveEast();
                    break;
                case "S":
                    MoveSouth();
                    break;
            }
        }
        public void ManageMove(Player player, int newRow, int newCol, int row, int col)
        {
            CurrentAction = Code.GameAction.None;
            CurrentMonster = null;

            switch (GameMap[newRow, newCol])
            {
                case VisitedPlace visited:
                    CurrentAction = Code.GameAction.AlreadyVisited;
                    //do nothing
                    break;
                case BossMonster boss:
                    CurrentAction = Code.GameAction.FightBossMonster;
                    CurrentMonster = boss;
                    break;
                case Monster monster:
                    CurrentAction = Code.GameAction.FightMonster;
                    CurrentMonster = monster;
                    break;
                case HealingPotion healing:
                    CurrentAction = Code.GameAction.PickHealingPotion;
                    //player.Score++;
                    player.HealingPotions++;
                    break;
                case ToxicPotion toxic:
                    CurrentAction = Code.GameAction.PickToxicPotion;
                    //player.Score++;
                    player.ToxicPotions++;
                    break;
                case null:
                    player.Score++;
                    break;
            }
            GameMap[row, col] = new VisitedPlace();
            GameMap[newRow, newCol] = player;
        }
        public void MoveNorth()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (GameMap[row, col] is Player)
                    {
                        Player player = GameMap[row, col] as Player;
                        if (row == 0)// cannot move
                        {

                        }
                        else
                        {
                            var newRow = row - 1;
                            ManageMove(player, newRow,col, row, col);
                        }
                        return;
                    }
                }
            }
        }
        public void MoveSouth()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (GameMap[row, col] is Player)
                    {
                        Player player = GameMap[row, col] as Player;
                        if (row == Rows-1)// cannot move
                        {

                        }
                        else
                        {
                            var newRow = row + 1;
                            ManageMove(player, newRow,col, row, col);
                        }
                        return;
                    }
                }
            }
        }
        public void MoveWest()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (GameMap[row, col] is Player)
                    {
                        Player player = GameMap[row, col] as Player;
                        if (col == 0)// cannot move
                        {

                        }
                        else
                        {
                            var newCol = col - 1;
                            ManageMove(player, row, newCol, row, col);
                        }
                        return;
                    }
                }
            }
        }
        public void MoveEast()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (GameMap[row, col] is Player)
                    {
                        Player player = GameMap[row, col] as Player;
                        if (col == Columns -1)// cannot move
                        {

                        }
                        else
                        {
                            var newCol = col + 1;
                            ManageMove(player, row, newCol, row, col);
                        }
                        return;
                    }
                }
            }
        }

        public override string ToString()
        {
            Player playerResult = null;
            var result = new StringBuilder();
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (GameMap[row, col] == null)
                    {
                        result.Append(" ");
                    }
                    else
                    {
                        switch (GameMap[row, col])
                        {
                            case Player player:
                                playerResult = player;
                                result.Append("P");
                                break;
                            case BossMonster boss:
                                result.Append("B");
                                break;
                            case Monster monster:
                                result.Append("M");
                                break;
                            case HealingPotion healing:
                                result.Append("H");
                                break;
                            case ToxicPotion toxic:
                                result.Append("T");
                                break;
                            case VisitedPlace visited:
                                result.Append("V");
                                break;
                        }
                    }
                }
                if(row < Rows-1)
                result.AppendLine();
            }

            return result.ToString();//JsonConvert.SerializeObject(new { Map = result.ToString(), Player = playerResult, Action = CurrentAction });
        }

        private void CheckStatus()
        {
            //count monsters
            var totMonsters = 0;
            if(CurrentMonster != null)
            {
                totMonsters++;
            }
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if(GameMap[row,col] is Monster)
                    {
                        totMonsters++;
                    }
                }
            }
            if(totMonsters == 0)
            {
                CurrentAction = Code.GameAction.Victory;
            }
        }
    }
}
