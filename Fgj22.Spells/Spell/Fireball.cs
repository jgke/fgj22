using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fgj22.Spells.Spell
{
    public class Fireball : SpellBase
    {
        public int Angle { get; set; }

        public static Regex Pattern { get; } = new Regex(@"FB\([1-9]([0-9])*\)");

        public static Fireball Create(string input)
        {
            var value = input.Substring(3).TrimEnd(')');

            return new Fireball
            {
                Angle = Int32.Parse(value)
            };
        }

    }
}
