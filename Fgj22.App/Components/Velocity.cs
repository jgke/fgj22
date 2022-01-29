using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fgj22.App.Components
{
    public class Velocity : Component, IUpdatable
    {
        private Vector2 Speed;

        public Velocity(Vector2 velocity)
        {
            Speed = velocity;
        }

        public void Update()
        {
            this.Entity.Transform.Position += Speed * Time.DeltaTime;
        }
    }
}
