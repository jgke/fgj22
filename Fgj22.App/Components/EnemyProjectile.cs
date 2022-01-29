using Fgj22.App.Utility;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fgj22.App.Components
{
    public class EnemyProjectile : Component
    {
        private Vector2 Velocity;

        public EnemyProjectile(double angle)
        {
            var velocity = 100;
            this.Velocity = new Vector2((float)(Math.Cos(angle) * velocity), (float)(Math.Sin(angle) * velocity));
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            var texture = Entity.Scene.Content.LoadTexture("Content/projectile.png");
            var renderer = new SpriteRenderer(texture);
            Entity.AddComponent(renderer);
            Entity.AddComponent(new Damage(3, true));
            Entity.AddComponent(new Team(Faction.Enemy, false));
            Entity.AddComponent(new BoxCollider(20, 20));
            Entity.AddComponent(new Health(1000));
            Entity.AddComponent(new Lifetime(4));

            this.AddComponent(new Velocity(Velocity));
        }
    }
}
