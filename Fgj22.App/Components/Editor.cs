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

        private VirtualButton InputOpen;

        private VirtualButton InputQ;

        private bool Visible = false;
        private string Content = "Asdasd";


        public Editor()
        {
            SetupInput();
        }

        void SetupInput()
        {
            InputOpen = new VirtualButton();
            InputOpen.Nodes.Add(new VirtualButton.KeyboardKey(Keys.LeftControl));
            
            InputQ = new VirtualButton();
            InputQ.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Q));
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            var texture = Entity.Scene.Content.LoadTexture("Content/editor.png");

            EditorRenderer = new SpriteRenderer(texture);
            EditorRenderer.Enabled = Visible;

            Text = new TextComponent(Graphics.Instance.BitmapFont, Content, new Vector2(2, 2), Color.White);
            Text.Enabled = Visible;

            Entity.AddComponent(EditorRenderer);
            Entity.AddComponent(Text);
        }

        public void Update()
        {
            if(InputOpen.IsPressed)
            {
                Visible = !Visible;

                EditorRenderer.Enabled = Visible;
                Text.Enabled = Visible;
            }
            else
            {
                if(InputQ.IsPressed)
                {
                    Content += "Q";
                }
            }

            if(Visible)
            {
                Text.Text = Content;
            }
        }
    }
}
