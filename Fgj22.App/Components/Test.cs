using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Text;
using Fgj22.App.Systems;
using static Nez.Tiled.TiledMapMover;
using Nez.Tweens;
using Serilog;

namespace Fgj22.App.Components
{
    class Test : Component, IUpdatable, ILoggable
    {
        private SpriteAnimator Animator;
        private TiledMapMover Mover;
        [Loggable]
        private BoxCollider BoxCollider;
        [Loggable]
        private CollisionState CollisionState = new CollisionState();

        private VirtualIntegerAxis XAxisInput;
        private VirtualIntegerAxis YAxisInput;
        private VirtualButton MeleeButton;
        private int MeleeTime = 0;
        Vector2 velocity;
        private TmxMap Map;

        private List<Vector2> MovementPath;
        private int MovementPathPos = -1;

        public Test(TmxMap Map) {
            this.Map = Map;
        }

        public override void OnAddedToEntity()
        {
            var texture = Entity.Scene.Content.LoadTexture("Content/Sigrithr.png");
            var sprites = Sprite.SpritesFromAtlas(texture, 64, 64);
            Entity.AddComponent(new BoxCollider(-8, -16, 16, 32));
            var collisionLayer = Map.GetLayer<TmxLayer>("main");
            Mover = new TiledMapMover(collisionLayer);
            Entity.AddComponent(Mover);
            Animator = Entity.AddComponent(new SpriteAnimator(sprites[0]));
            BoxCollider = Entity.GetComponent<BoxCollider>();
            Entity.AddComponent(new Health(5));
            Entity.AddComponent(new Team(1));

            int r = 8;
            Animator.AddAnimation("Idle", new[]
            {
                sprites[0*r + 0]
            });
            Animator.AddAnimation("Run", new[]
            {
                sprites[0*r + 0],
                sprites[0*r + 1],
                sprites[0*r + 2],
                sprites[0*r + 3],
                sprites[0*r + 4],
                sprites[0*r + 5],
                sprites[0*r + 6],
                sprites[0*r + 7],
            });
            Animator.AddAnimation("Melee", new[]
            {
                sprites[1 * r + 0],
                sprites[1 * r + 1],
                sprites[1 * r + 2],
                sprites[1 * r + 3],
                sprites[1 * r + 4],
            });

            SetupInput();
        }

        public override void OnRemovedFromEntity()
        {
            XAxisInput.Deregister();
            YAxisInput.Deregister();
            MeleeButton.Deregister();
        }

        void SetupInput()
        {
            // horizontal input from dpad, left stick or keyboard left/right
            XAxisInput = new VirtualIntegerAxis();
            XAxisInput.Nodes.Add(new VirtualAxis.GamePadDpadLeftRight());
            XAxisInput.Nodes.Add(new VirtualAxis.GamePadLeftStickX());
            XAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Left, Keys.Right));

            // vertical input from dpad, left stick or keyboard up/down
            YAxisInput = new VirtualIntegerAxis();
            YAxisInput.Nodes.Add(new VirtualAxis.GamePadDpadUpDown());
            YAxisInput.Nodes.Add(new VirtualAxis.GamePadLeftStickY());
            YAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Up, Keys.Down));

            // Melee. A on the keyboard or A on the gamepad
            MeleeButton = new VirtualButton();
            MeleeButton.Nodes.Add(new VirtualButton.KeyboardKey(Keys.A));
            MeleeButton.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.A));
        }

        public void UpdateAnimation()
        {
            if (Animator.IsAnimationActive("Melee") && Animator.IsRunning)
            {
                return;
            }

            if (MeleeButton.IsDown && MeleeTime == 0)
            {
                Animator.Play("Melee", SpriteAnimator.LoopMode.ClampForever);
                float duration = Animator.CurrentAnimation.Sprites.Length / (Animator.CurrentAnimation.FrameRate * Animator.Speed);
                Entity.TweenRotationDegreesTo(-45, duration)
                    .SetLoops(LoopType.PingPong, 1)
                    .Start();
                return;
            }

            string animation = "Idle";

            if (velocity != Vector2.Zero)
            {
                animation = "Run";
            }

            
            if (animation != null && !Animator.IsAnimationActive(animation))
            {
                Animator.Play(animation);
            }
        }

        public void Update()
        {
			if (Input.LeftMouseButtonPressed) {
				var start = Entity.Transform.Position;
				var end = Entity.Scene.Camera.MouseToWorldPoint();

                MovementPath = Entity.Scene.GetSceneComponent<PathFinder>().GetRoute(start, end);
                if(MovementPath != null) {
                    MovementPathPos = 0;
                } else {
                    MovementPathPos = -1;
                }
            }

            if(MovementPathPos != -1) {
                var direction = MovementPath[MovementPathPos] - Entity.Transform.Position;
                if (direction.Length() < 5) {
                    MovementPathPos += 1;
                    if(MovementPathPos < MovementPath.Count) {
                        direction = MovementPath[MovementPathPos] - Entity.Transform.Position;
                    } else {
                        MovementPathPos = -1;
                    }
                }
                direction.Normalize();
                velocity = direction * 150;
            } else {
                velocity = new Vector2(XAxisInput.Value, YAxisInput.Value) * 150;
            }

            Mover.Move(velocity * Time.DeltaTime, BoxCollider, CollisionState);

            UpdateAnimation();
        }

        public override void DebugRender(Batcher batcher) {
            base.DebugRender(batcher);
            var mouseLocation = Entity.Scene.Camera.MouseToWorldPoint();
            batcher.DrawPixel(mouseLocation.X, mouseLocation.Y, Debug.Colors.DebugText, 3);
        }

    }
}
