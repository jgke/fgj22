using Microsoft.Xna.Framework;
using Nez;
using System;
using Serilog;

namespace Fgj22.App.Components
{
    public class Damage : Component, IUpdatable, ILoggable
    {
        [Loggable]
        int OnHit;
        [Loggable]
        bool destroyOnHit;

        public Damage(int damageOnHit, bool destroyOnHit)
        {
            this.OnHit = damageOnHit;
            this.destroyOnHit = true;
        }

        public override void OnAddedToEntity()
        {
            var collider = Entity.GetComponent<BoxCollider>();
        }

        void IUpdatable.Update()
        {
            var neighborColliders = Physics.BoxcastBroadphaseExcludingSelf(Entity.GetComponent<Collider>());

            // loop through and check each Collider for an overlap
            foreach (var collider in neighborColliders)
            {
                if (Entity.GetComponent<Collider>().Overlaps(collider))
                {
                    DoCollision(collider.Entity);
                }
            }
        }

        private void DoCollision(Entity other)
        {
            Console.WriteLine("Damage OnTriggerEnter");
            var myTeam = this.Entity.GetComponent<Team>().Faction;
            Team otherTeam = other.GetComponent<Team>();
            Health otherHealth = other.GetComponent<Health>();

            Log.Information("myTeam: {A}, other: {@B}, otherHealth: {@C}", myTeam, otherTeam, otherHealth);

            if (otherTeam != null && otherHealth != null && myTeam != otherTeam.Faction)
            {
                otherHealth.Hit(OnHit);
            }
            else
            {
                return;
            }

            if (this.destroyOnHit)
            {
                Entity.Destroy();
            }
            else
            {
                // callback here or sth
            }
        }
    }
}
