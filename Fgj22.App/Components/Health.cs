using Microsoft.Xna.Framework;
using Nez;
using System;

namespace Fgj22.App.Components
{
    public class Health : Component
    {

        public int Maximum;
        public int Current;

        public Health(int Maximum)
        {
            this.Maximum = Maximum;
            this.Current = Maximum;
        }

        public int Hit(int damage)
        {
            this.Current -= damage;
            return this.Current;
        }
    }
}
