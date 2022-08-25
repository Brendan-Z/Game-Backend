using A2.Models;

namespace A2.Data
{
    public interface IA2Repo
    {
        public User RegisterUser(User user);
        public bool LoginCheck(string username, string password);

        public bool UserExists(User user);
        public GameRecord PairingUser(string username);
        public IEnumerable<GameRecord> GetGameRecords();
        public bool GetUserActiveGame(String username);
        public GameRecord AddToGameRecord(GameRecord gameRecord);
        public GameRecord GetGameById(string id);
        public void UpdateGameRecord(GameRecord gameRecord);
        public void RemoveGameRecord(GameRecord gameRecord);
    }
}