using Microsoft.Xna.Framework;
using Nez;
using System;

namespace Fgj22.App.Components
{
    public class Damage : Component, IUpdatable
    {

        int OnHit;
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
                    DoCollision(Entity);
                }
            }
        }

        private void DoCollision(Entity other)
        {
            Console.WriteLine("Damage OnTriggerEnter");
            int myTeam = this.Entity.GetComponent<Team>().TeamNum;
            Team otherTeam = other.GetComponent<Team>();
            Health otherHealth = other.GetComponent<Health>();

            Console.WriteLine("myTeam: " + myTeam + " other: " + otherTeam + " otherHealth: " + otherHealth);

            if (otherTeam != null && otherHealth != null && myTeam != otherTeam.TeamNum)
            {
                otherHealth.Hit(OnHit);
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
