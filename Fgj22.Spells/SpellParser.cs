using Fgj22.Spells.Spell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fgj22.Spells
{
    public static class SpellParser
    {
        public static SpellBase ParseInput(string command)
        {
            if(Fireball.Pattern.Matches(command).Count == 1)
            {
                return Fireball.Create(command);
            }
            else if (Zap.Pattern.Matches(command).Count == 1)
            {
                return Zap.Create(command);
            }
            else
            {
                return null;
            }
        }
    }
}
