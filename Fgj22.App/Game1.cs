using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Serilog;

namespace Fgj22.App
{
    public class Game1 : Core
    {
        protected override void Initialize()
        {
            base.Initialize();

            Window.AllowUserResizing = true;
            Scene = new MenuScene();
            DebugRenderEnabled = false;
        }
    }
}
