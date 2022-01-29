using Microsoft.Xna.Framework;
using Nez;

namespace Fgj22.App.Components
{
    public class Team : Component, ILoggable
    {
        [Loggable]
        public int TeamNum;

        public Team(int teamNum)
        {
            this.TeamNum = teamNum;
        }
    }
}
