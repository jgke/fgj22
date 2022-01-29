using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fgj22.App.Components
{
    public class MeleeAttack : Damage
    {
        Component Parent;

        public float LifetimeSeconds { get; }
        public int DistanceFromParent { get; }
        public float RotationPrefix { get; }

        public MeleeAttack(Component parent, int damage, float lifetimeSeconds, int distanceFromParent, float rotationPrefix = 0) : base(damage, true)
        {
            Parent = parent;
            LifetimeSeconds = lifetimeSeconds;
            DistanceFromParent = distanceFromParent;
            RotationPrefix = rotationPrefix;
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            Entity.AddComponent(new Lifetime(LifetimeSeconds));
        }

        public override void Update()
        {
            base.Update();

            Transform.Position = Parent.Transform.Position + new Vector2((float)(Math.Cos(Parent.Transform.Rotation + RotationPrefix) * DistanceFromParent), (float)(Math.Sin(Parent.Transform.Rotation + RotationPrefix) * DistanceFromParent));
        }
    }
}
