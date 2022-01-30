using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Fgj22.Spells.Spell
{
    public class Zap : SpellBase
    {
        public static Regex Pattern { get; } = new Regex(@"STUN");

        public static Zap Create(string input)
        {
            return new Zap
            {
                
            };
        }
    }
}
