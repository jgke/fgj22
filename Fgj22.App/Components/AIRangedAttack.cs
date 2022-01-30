using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fgj22.App.Components
{
    public class AIRangedAttack : Component, IUpdatable
    {
        private int MinimumShootingDistance;
        private float ReloadTime;
        private readonly int Damage;
        private readonly float Lifetime;
        private readonly string Texture;
        private readonly int ProjectileSize;
        private float ReloadLeft;
        Player Player;

        public AIRangedAttack(Player player, int minimumShootingDistance, float reloadTime, int projectileSize, int damage, float lifetime, string texture)
        {
            Player = player;
            MinimumShootingDistance = minimumShootingDistance;
            ReloadTime = reloadTime;
            Damage = damage;
            Lifetime = lifetime;
            Texture = texture;
            ProjectileSize = projectileSize;
        }

        public void Update()
        {
            if(Player.Entity == null)
            {
                return;
            }

            if (Entity.GetComponent<Stunned>() != null)
            {
                return;
            }

            ReloadLeft -= Time.DeltaTime;

            if (Entity.Transform.Position.Pythagoras(Player.Transform.Position) > MinimumShootingDistance)
            {
                return;
            }

            if (ReloadLeft <= 0)
            {
                ReloadLeft = ReloadTime;
            }
            else
            {
                return;
            }

            var entity = Entity.Scene.CreateEntity("projectile", Entity.Transform.Position);

            var targetAngle = (Player.Transform.Position - Transform.Position).GetAngle();

            entity.AddComponent(new EnemyProjectile(targetAngle, ProjectileSize, Damage, Lifetime, Texture));
        }
    }
}
