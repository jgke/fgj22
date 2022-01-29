using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fgj22.App.Components
{
    public class Lifetime : Component, IUpdatable
    {
        private float Seconds;
        
        public Lifetime(float seconds)
        {
            Seconds = seconds;
        }

        public void Update()
        {
            Seconds -= Time.DeltaTime;

            if(Seconds <= 0)
            {
                Entity.Destroy();
            }
        }
    }
}
