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
        private readonly string Texture;
        private Vector2 Velocity;
        private readonly int Damage;
        private readonly float Lifetime;
        private int Width;

        public EnemyProjectile(double angle, int width, int damage, float lifetime, string texture)
        {
            var velocity = 100;
            Width = width;
            this.Velocity = new Vector2((float)(Math.Cos(angle) * velocity), (float)(Math.Sin(angle) * velocity));
            Damage = damage;
            Lifetime = lifetime;
            Texture = texture;
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            var texture = Entity.Scene.Content.LoadTexture($"Content/{Texture}");
            var renderer = new SpriteRenderer(texture);
            Entity.AddComponent(renderer);
            Entity.AddComponent(new Damage(Damage, true));
            Entity.AddComponent(new Team(Faction.Enemy, false));
            Entity.AddComponent(new BoxCollider(Width, Width));
            Entity.AddComponent(new Health(1000));
            Entity.AddComponent(new Lifetime(Lifetime));

            this.AddComponent(new Velocity(Velocity));
        }
    }
}
