using Fgj22.App.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fgj22.App.Utility
{
    public class ScreenPosition
    {
        private CameraBounds CameraBounds;

        public ScreenPosition(CameraBounds cameraBounds)
        {
            this.CameraBounds = cameraBounds;
        }

        public Vector2 GetPositionOnScreen(Vector2 relativePosition)
        {
            var position = CameraBounds.GetTunedCameraPosition();

            return CameraBounds.Entity.Scene.Camera.Position + position + relativePosition;
        }
    }
}
