using Fgj22.App.Utility;
using Microsoft.Xna.Framework;
using Nez;

namespace Fgj22.App.Components
{
    public class Team : Component, ILoggable
    {
        [Loggable]
        public Faction Faction;

        public Team(Faction team)
        {
            this.Faction = team;
        }
    }
}
