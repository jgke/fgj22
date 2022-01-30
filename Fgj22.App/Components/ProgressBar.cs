using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fgj22.App.Components
{
    class ProgressBar : Component, IUpdatable
    {
        Table Table;
        Label Title;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            UICanvas canvas = new UICanvas();
            Entity.AddComponent(canvas);

            LabelStyle labelStyle = new LabelStyle()
            {
                FontColor = Color.Black,
                Background = new PrimitiveDrawable(Color.DarkGray)
            };

            Table = canvas.Stage.AddElement(new Table());
            Table.SetHeight(20);
            Table.SetWidth(90);
            Table.SetPosition(400, 0);

            Title = new Label("ASD", labelStyle);
            Title.SetAlignment(Align.Left);
            Table.Add(Title).Bottom().Width(150).SetAlign(Align.Right);
        }

        public void Update()
        {
            var enemies = Entity.Scene.FindComponentsOfType<Enemy>();

            Title.SetText($" Enemies remaining: {enemies.Count}  ");
        }
    }
}
