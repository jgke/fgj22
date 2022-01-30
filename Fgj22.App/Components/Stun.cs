using Fgj22.App.Utility;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fgj22.App.Components
{
    public class Stun : Component
    {
        public int Radius = 100;
        public float Duration = 2;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            
            var texture = Entity.Scene.Content.LoadTexture("Content/stun.png");
            var renderer = new SpriteRenderer(texture);
            Entity.AddComponent(renderer);
            Entity.AddComponent(new Team(Faction.Friendly, false));
            Entity.AddComponent(new Lifetime(0.5f));

            StunEnemiesOnRange();
        }

        private void StunEnemiesOnRange()
        {
            var enemies = Entity.Scene.FindComponentsOfType<Enemy>();

            var enemiesWithinRange = enemies.Where(e => e.Transform.Position.Pythagoras(Entity.Transform.Position) < Radius);

            foreach(var enemy in enemiesWithinRange)
            {
                enemy.AddComponent(new Stunned(2));
            }
        }
    }
}
