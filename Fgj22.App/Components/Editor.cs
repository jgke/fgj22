using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fgj22.App.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using static Nez.VirtualButton;

namespace Fgj22.App.Components
{
    public class Editor : Component, IUpdatable
    {
        private SpriteRenderer EditorRenderer;
        private TextComponent Text;

        private readonly List<(KeyboardKey key, string text)> TextInput = new List<(KeyboardKey key, string text)>();
        private KeyboardKey Backspace;

        private string Content = "";
        private ScreenPosition Position;

        public Editor(ScreenPosition position)
        {
            Position = position;
            SetupInput();

        }

        void SetupInput()
        {
            for(int i = (int)Keys.A; i <= (int)Keys.Z; i ++)
            {
                var key = (Keys)i;
                TextInput.Add((new VirtualButton.KeyboardKey(key), key.ToString().ToLower()));
            }

            Backspace = new KeyboardKey(Keys.Back);
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            var texture = Entity.Scene.Content.LoadTexture("Content/editor.png");

            EditorRenderer = new SpriteRenderer(texture);
            Text = new TextComponent(Graphics.Instance.BitmapFont, Content, new Vector2(-80, -40), Color.White);

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
            this.Entity.SetPosition(Position.GetPositionOnScreen(new Vector2(-100, -100)));

            var pressedButton = TextInput.FirstOrDefault(b => b.key.IsPressed);
            if (pressedButton != default)
            {
                Content += pressedButton.text;
            }
            else if(Backspace.IsPressed)
            {
                Content = Content.Substring(0, Content.Length - 1);
            }

            Text.Text = Content;
        }
    }
}
