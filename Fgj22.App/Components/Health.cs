using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System;

namespace Fgj22.App.Components
{
    public class Health : Component, IUpdatable, ILoggable
    {
        [Loggable]
        public int Maximum;
        [Loggable]
        public int Current;

        public int Previous;

        private SpriteRenderer Renderer;

        private bool ShowHealthBar;

        private Entity HealthBar;

        public Health(int Maximum, bool showHealthBar)
        {
            this.Maximum = Maximum;
            this.Current = Maximum;
            this.Previous = Maximum;
            ShowHealthBar = showHealthBar;
        }

        public int Hit(int damage)
        {
            this.Current -= damage;
            return this.Current;
        }

        public void Update()
        {
            if (Current <= 0)
            {
                Entity.Destroy();
            }

            if (ShowHealthBar)
            {
                if(Current <= 0)
                {
                    HealthBar.Destroy();
                    ShowHealthBar = false;
                    return;
                }

                if (Previous != Current)
                {
                    Renderer.Sprite = new Sprite(CreateHealthBar());
                }

                Previous = Current;


                Renderer.Transform.Rotation = 0;
                Renderer.Transform.Position = new Vector2(0, 30) + Entity.Transform.Position;

            }
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            if (ShowHealthBar)
            {
                var texture = CreateHealthBar();

                HealthBar = Entity.Scene.CreateEntity("healthBar");
                Renderer = new SpriteRenderer(texture);
                HealthBar.AddComponent(Renderer);
                Renderer.Transform.Rotation = 0;
            }
        }

        Texture2D CreateHealthBar()
        {
            var length = (int)Math.Round(((double)Current / (double)Maximum) * 5.0, 0) * 10;

            if(length == 0)
            {
                length = 1;
            }

            var k = Entity.Scene.Content.LoadTexture("Content/fireball.png");
            var texture = new Texture2D(k.GraphicsDevice, length, 5, false, SurfaceFormat.Color);

            Color[] data = new Color[length * 5];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Red;
            texture.SetData(data);

            return texture;
        }

    }
}
