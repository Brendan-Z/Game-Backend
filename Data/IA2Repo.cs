using A2.Models;

namespace A2.Data
{
    public interface IA2Repo
    {
        public User RegisterUser(User user);
        public bool UserExists(User user);
        public bool ValidLogin(string username, string password);
        public GameRecord PairingUser(string username);
        public IEnumerable<GameRecord> GetGameRecords();
        public GameRecord GetGameById(string id);
        public GameRecord AddToGameRecord(GameRecord gameRecord);
        public void UpdateGameRecord(GameRecord gameRecord);
        public void RemoveGameRecord(GameRecord gameRecord);
    }
}