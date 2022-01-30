namespace Fgj22.App
{
    public class GameState : ILoggable
    {
        static GameState _instance;
        public static GameState Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameState(0);
                }
                return _instance;
            }
        }

        [Loggable]
        public int LevelNum;
        [Loggable]
        public int PlayerSpeed = 150;
        [Loggable]
        public int Counter = 0;

        public GameState(int levelNum)
        {
            LevelNum = levelNum;
        }
    }
}