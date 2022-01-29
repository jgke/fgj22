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
    /// <summary>
    /// this entire class is one big sweet hack job to make adding samples easier. An exceptional hack is made so that we can render small
    /// pixel art scenes pixel perfect and still display our UI at a reasonable size.
    /// </summary>
    public abstract class SampleScene : Scene
    {
        public const int ScreenSpaceRenderLayer = 999;
        public UICanvas Canvas;

        public SampleScene()
        {
            AddRenderer(new ScreenSpaceRenderer(100, ScreenSpaceRenderLayer));

            // create our canvas and put it on the screen space render layer
            Canvas = CreateEntity("ui").AddComponent(new UICanvas());
            Canvas.IsFullScreen = true;
            Canvas.RenderLayer = ScreenSpaceRenderLayer;
        }
    }
}