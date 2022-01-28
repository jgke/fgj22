using Fgj22.Spells;
using Fgj22.Spells.Spell;
using System;
using Xunit;

namespace Fgj22.Tests
{
    public class ParserTests
    {
        [Fact]
        public void TestParser_GeneratesFireball()
        {
            var spell = SpellParser.ParseInput("FB(1)");
            Assert.NotNull(spell);
            Assert.True(spell is Fireball fb && fb.Angle == 1);
        }

        [Fact]
        public void TestTestParser_ReturnsNull()
        {
            var spell = SpellParser.ParseInput("FB(A)");
            Assert.Null(spell);
        }
    }
}
