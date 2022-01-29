using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Nez.UI;
using Microsoft.Xna.Framework.Graphics;
using Nez.Tweens;
using System.Linq;
using Nez.Console;
using Nez;

namespace Fgj22.App
{
    public abstract class ProgramScene : Scene
    {
        public const int ScreenSpaceRenderLayer = 999;
        public UICanvas Canvas;

        public ProgramScene()
        {
            AddRenderer(new ScreenSpaceRenderer(100, ScreenSpaceRenderLayer));

            // create our canvas and put it on the screen space render layer
            Canvas = CreateEntity("ui").AddComponent(new UICanvas());
            Canvas.IsFullScreen = true;
            Canvas.RenderLayer = ScreenSpaceRenderLayer;
        }

        public override void Initialize()
        {
            base.Initialize();
            ClearColor = Color.Aquamarine;

            // Set the playing area size
            SetDesignResolution(480, 320, SceneResolutionPolicy.ShowAllPixelPerfect);
            // Set the rendering resolution
            Screen.SetSize(1440, 1280);
        }
    }
}