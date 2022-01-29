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

        VirtualIntegerAxis _xAxisInput;
        VirtualIntegerAxis _yAxisInput;
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

        public override void OnRemovedFromEntity() {
            _xAxisInput.Deregister();
            _yAxisInput.Deregister();
        }

        void SetupInput()
        {
            // horizontal input from dpad, left stick or keyboard left/right
            _xAxisInput = new VirtualIntegerAxis();
            _xAxisInput.Nodes.Add(new VirtualAxis.GamePadDpadLeftRight());
            _xAxisInput.Nodes.Add(new VirtualAxis.GamePadLeftStickX());
            _xAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Left, Keys.Right));

            // vertical input from dpad, left stick or keyboard up/down
            _yAxisInput = new VirtualIntegerAxis();
            _yAxisInput.Nodes.Add(new VirtualAxis.GamePadDpadUpDown());
            _yAxisInput.Nodes.Add(new VirtualAxis.GamePadLeftStickY());
            _yAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Up, Keys.Down));

        }

        public void Update()
        {
            var velocity = new Vector2(_xAxisInput.Value, _yAxisInput.Value) * 150;

            Mover.Move(velocity * Time.DeltaTime, BoxCollider, CollisionState);

            var animation = "Run";

            if (animation != null && !Animator.IsAnimationActive(animation))
            {
                Animator.Play(animation);
            }
        }
    }
}
