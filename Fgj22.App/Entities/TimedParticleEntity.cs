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
    public static class Particles {
        public static Entity TimedParticleEntity(Scene Scene, Vector2 position, string particleConfig, float lifetime) {
            var Entity = Scene.CreateEntity("TimedParticleEntity", position);

            Entity.AddComponent(new Lifetime(lifetime));

			var config = Entity.Scene.Content.LoadParticleEmitterConfig(particleConfig);
			var _particleEmitter = Entity.AddComponent(new ParticleEmitter(config));
            _particleEmitter.SetRenderLayer(-100);
			_particleEmitter.CollisionConfig.Enabled = false;
			_particleEmitter.SimulateInWorldSpace = true;
            return Entity;
        }
    }
}
