using Fgj22.App.Utility;
using Microsoft.Xna.Framework;
using Nez;

namespace Fgj22.App.Components
{
    public class Team : Component, ILoggable
    {
        [Loggable]
        public Faction Faction;

        public bool CanBeCollidedWith;

        public Team(Faction team, bool canBeCollidedWith)
        {
            this.Faction = team;
            this.CanBeCollidedWith = canBeCollidedWith;
        }
    }
}
