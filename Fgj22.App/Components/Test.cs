using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Text;
using static Nez.Tiled.TiledMapMover;

namespace Fgj22.App.Components
{
    class Test : Component, IUpdatable, ILoggable
    {
        private SpriteAnimator Animator;
        private TiledMapMover Mover;
        [Loggable]
        private BoxCollider BoxCollider;
        private VirtualButton InputMoveUp;
        [Loggable]
        private CollisionState CollisionState = new CollisionState();

        public override void OnAddedToEntity()
        {
            var texture = Entity.Scene.Content.LoadTexture("Content/Sigrithr.png");
            var sprites = Sprite.SpritesFromAtlas(texture, 64, 64);
            Animator = Entity.AddComponent(new SpriteAnimator(sprites[0]));
            Mover = Entity.GetComponent<TiledMapMover>();
            BoxCollider = Entity.GetComponent<BoxCollider>();
            Entity.AddComponent(new Health(5));
            Entity.AddComponent(new Team(1));

            Animator.AddAnimation("Run", new[]
            {
                sprites[0],
                sprites[1],
                sprites[2],
                sprites[3],
                sprites[4],
                sprites[5],
                sprites[6],
                sprites[7],
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
            var velocity = new Vector2();

            if (InputMoveUp.IsDown)
            {
                velocity = new Vector2(0, 150);
            }

            Mover.Move(velocity * Time.DeltaTime, BoxCollider, CollisionState);

            var animation = "Run";

            if (animation != null && !Animator.IsAnimationActive(animation))
            {
                Animator.Play(animation);
            }
        }
    }
}
