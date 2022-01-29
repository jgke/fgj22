using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fgj22.App.Components
{
    public class Emp : Component
    {
        private Vector2 velocity2;

        public Emp(Vector2 velocity)
        {
            this.velocity2 = velocity;
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            var texture = Entity.Scene.Content.LoadTexture("Content/fireball.png");
            var renderer = new SpriteRenderer(texture);
            Entity.AddComponent(renderer);

            this.AddComponent(new Velocity(velocity2));
        }
    }
}
