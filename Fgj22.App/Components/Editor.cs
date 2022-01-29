using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.Textures;

namespace Fgj22.App.Components
{
    public class Editor : Component, IUpdatable
    {
        private SpriteRenderer EditorRenderer;
        private TextComponent Text;

        private VirtualButton InputQ;

        private string Content = "Asdasd";


        public Editor()
        {
            SetupInput();
        }

        void SetupInput()
        {
            InputQ = new VirtualButton();
            InputQ.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Q));
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            var texture = Entity.Scene.Content.LoadTexture("Content/editor.png");

            EditorRenderer = new SpriteRenderer(texture);
            Text = new TextComponent(Graphics.Instance.BitmapFont, Content, new Vector2(2, 2), Color.White);

            Entity.AddComponent(EditorRenderer);
            Entity.AddComponent(Text);

            Enabled = false;
            Text.Enabled = false;
            EditorRenderer.Enabled = false;
        }

        public void SetVisibility(bool isVisible)
        {
            EditorRenderer.Enabled = isVisible;
            Text.Enabled = isVisible;
            Enabled = isVisible;
        }

        public void Update()
        {
            if (InputQ.IsPressed)
            {
                Content += "Q";
            }

            Text.Text = Content;
        }
    }
}
