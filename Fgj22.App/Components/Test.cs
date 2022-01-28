using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fgj22.App.Components
{
    class Test : Component, IUpdatable
    {
        private SpriteAnimator Animator;
        private VirtualButton InputMoveUp;

        public override void OnAddedToEntity()
        {
            var texture = Entity.Scene.Content.LoadTexture("Content/caveman.png");
            var sprites = Sprite.SpritesFromAtlas(texture, 32, 32);
            Animator = Entity.AddComponent(new SpriteAnimator(sprites[0]));

            Animator.AddAnimation("Run", new[]
            {
                sprites[8 + 0],
                sprites[8 + 1],
                sprites[8 + 2],
                sprites[8 + 3],
                sprites[8 + 4],
                sprites[8 + 5],
                sprites[8 + 6]
            });

            SetupInput();
        }

        void SetupInput()
        {
            InputMoveUp = new VirtualButton();
            InputMoveUp.Nodes.Add(new VirtualButton.KeyboardKey(Keys.W));
        }

        public void Update()
        {
            if(InputMoveUp.IsDown)
            {
                this.Entity.Position += new Microsoft.Xna.Framework.Vector2(0, -Time.DeltaTime * 150);
            }

            var animation = "Run";

            if (animation != null && !Animator.IsAnimationActive(animation))
            {
                Animator.Play(animation);
            }
        }
    }
}
