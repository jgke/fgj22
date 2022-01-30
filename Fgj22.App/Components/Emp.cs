using Fgj22.App.Utility;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Text;
using Nez.Particles;
using Serilog;
using System.Linq;

namespace Fgj22.App.Components
{
    public class Emp : Component, IUpdatable
    {
        private Vector2 Velocity;
        private int Speed = 100;
        private Velocity VelocityComponent;

        public Emp(double angle)
        {
            this.Velocity = new Vector2((float)(Math.Cos(angle) * Speed), (float)(Math.Sin(angle) * Speed));
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
            Entity.AddComponent(new Health(1000, false));
            Entity.AddComponent(new Lifetime(4));

            VelocityComponent = new Velocity(Velocity);
            Entity.AddComponent(VelocityComponent);

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

        public void Update()
        {
            var enemies = Entity.Scene.FindComponentsOfType<Enemy>();

            var closest = enemies.OrderBy(e => e.Transform.Position.Pythagoras(Transform.Position)).FirstOrDefault();

            if(closest == null)
            {
                return;
            }

            var directionToEnemy = (closest.Transform.Position - Transform.Position);
            directionToEnemy.Normalize();

            var newDirection = VelocityComponent.Speed + directionToEnemy;
            newDirection.Normalize();

            VelocityComponent.Speed = Speed * newDirection;
        }
    }
}
