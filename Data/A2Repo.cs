using Microsoft.EntityFrameworkCore.ChangeTracking;
using A2.Models;

namespace A2.Data
{
    public class A2Repo : IA2Repo
    {
        private readonly A2DBContext _dbContext;

        public A2Repo(A2DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User RegisterUser(User user)
        {
            EntityEntry<User> e = _dbContext.Users.Add(user);
            User us = e.Entity;
            _dbContext.SaveChanges();
            return us;
        }

        public bool ValidLogin(string username, string password)
        {
            User user = _dbContext.Users.FirstOrDefault(e => e.UserName == username && e.Password == password);

            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool UserExists(User user)
        {
            return _dbContext.Users.Any(e => e.UserName == user.UserName);
        }

        public GameRecord PairingUser(string username)
        {
            GameRecord game = _dbContext.GameRecords.FirstOrDefault(e => e.State == "wait");

            if (game == null)
            {
                GameRecord newGame = new() { 
                    GameId = Guid.NewGuid().ToString(), 
                    State = "wait", 
                    Player1 = username, 
                    Player2 = null, LastMovePlayer1 = null, 
                    LastMovePlayer2 = null };

                EntityEntry<GameRecord> e = _dbContext.GameRecords.Add(newGame);
                GameRecord newGameAdded = e.Entity;
                _dbContext.SaveChanges();
                return newGameAdded;
            }
            else if (game.Player1 == username)
            {
                return game;
            }
            else
            {
                game.State = "progress";
                game.Player2 = username;
                _dbContext.SaveChanges();
                return game;
            }
        }

        public IEnumerable<GameRecord> GetGameRecords()
        {
            IEnumerable<GameRecord> allGameRecords = _dbContext.GameRecords.ToList();
            return allGameRecords;
        }

        public GameRecord GetGameById(string id)
        {
            GameRecord game = _dbContext.GameRecords.FirstOrDefault(e => e.GameId == id);
            return game;
        }

        public GameRecord AddToGameRecord(GameRecord gameRecord)
        {
            EntityEntry<GameRecord> e = _dbContext.GameRecords.Add(gameRecord);
            GameRecord g = e.Entity;
            _dbContext.SaveChanges();
            return g;
        }

        public void UpdateGameRecord(GameRecord gameRecord)
        {
            _dbContext.Update(gameRecord);
            _dbContext.SaveChanges();
        }

        public void RemoveGameRecord(GameRecord gameRecord)
        {
            _dbContext.Remove(gameRecord);
            _dbContext.SaveChanges();
        }
    }
}