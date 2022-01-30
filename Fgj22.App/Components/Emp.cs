using Fgj22.App.Utility;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Text;
using Nez.Particles;
using Serilog;

namespace Fgj22.App.Components
{
    public class Emp : Component
    {
        private Vector2 Velocity;

        public Emp(double angle)
        {
            var velocity = 100;
            this.Velocity = new Vector2((float)(Math.Cos(angle) * velocity), (float)(Math.Sin(angle) * velocity));
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            var texture = Entity.Scene.Content.LoadTexture("Content/fireball.png");
            var renderer = new SpriteRenderer(texture);
            Entity.AddComponent(renderer);
            Entity.AddComponent(new Damage(10, true));
            Entity.AddComponent(new Team(Faction.Friendly, false));
            Entity.AddComponent(new BoxCollider(30, 30));
            Entity.AddComponent(new Health(1000));
            Entity.AddComponent(new Lifetime(4));
            Entity.AddComponent(new Velocity(Velocity));

			var config = Entity.Scene.Content.LoadParticleEmitterConfig("Content/particles/Blue Galaxy.pex");
			var _particleEmitter = Entity.AddComponent(new ParticleEmitter(config));
            _particleEmitter.SetRenderLayer(-100);
			_particleEmitter.CollisionConfig.Enabled = false;
			_particleEmitter.SimulateInWorldSpace = true;
        }

        public override void OnRemovedFromEntity() {
           Particles.TimedParticleEntity(
               Entity.Scene, Entity.Transform.Position, "Content/particles/Giros Gratis.pex", 2);
            base.OnRemovedFromEntity();
        }
    }
}
