using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fgj22.App.Utility;
using Fgj22.Spells;
using Fgj22.Spells.Spell;
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
        private KeyboardKey Execute;

        private string Content = "";
        private ScreenPosition Position;

        public Queue<SpellBase> Spells = new Queue<SpellBase>();

        public Editor(ScreenPosition position)
        {
            Position = position;
            SetupInput();

        }

        void SetupInput()
        {
            for (int i = (int)Keys.A; i <= (int)Keys.Z; i++)
            {
                var key = (Keys)i;
                TextInput.Add((new KeyboardKey(key), key.ToString().ToUpper()));
            }

            var dPrefix = (int)Keys.D0;
            for (int i = 0; i <= (int)Keys.D9 - dPrefix; i++)
            {
                var key = (Keys)(i + dPrefix);
                TextInput.Add((new KeyboardKey(key), i.ToString()));
            }

            var numPadPrefix = (int)Keys.NumPad0;
            for (int i = 0; i <= (int)Keys.NumPad9 - numPadPrefix; i++)
            {
                var key = (Keys)(i + numPadPrefix);
                TextInput.Add((new KeyboardKey(key), i.ToString()));
            }

            TextInput.Add((new KeyboardKey(Keys.Space), " "));

            Backspace = new KeyboardKey(Keys.Back);
            Execute = new KeyboardKey(Keys.Enter);
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            var texture = Entity.Scene.Content.LoadTexture("Content/editor.png");

            EditorRenderer = new SpriteRenderer(texture);
            Text = new TextComponent(Graphics.Instance.BitmapFont, Content, new Vector2(-80, -40), Color.White);

            Entity.AddComponent(EditorRenderer).SetRenderLayer(-100);
            Entity.AddComponent(Text).SetRenderLayer(-101);

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
            else if (Backspace.IsPressed && Content.Length > 0)
            {
                Content = Content[0..^1];
            }
            else if (Execute.IsPressed)
            {
                var spell = SpellParser.ParseInput(Content);


                if (spell != null)
                {
                    Spells.Enqueue(spell);
                    Content = "";

                    SetVisibility(false);
                }
            }

            Text.Text = Content;
        }
    }
}
