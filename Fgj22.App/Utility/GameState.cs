using System.Collections.Generic;
using System;
using Nez;

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
        [Loggable]
        public HashSet<string> Upgrades = new HashSet<string>();
        [Loggable]
        public bool Transitioning; 

        public GameState(int levelNum)
        {
            LevelNum = levelNum;
        }

        public void DoTransition(Func<Scene> act) {
            this.Transitioning = true;
            Core.StartSceneTransition(new WindTransition(act)).OnTransitionCompleted += () => {
                this.Transitioning = false;
            };
        }
    }
}