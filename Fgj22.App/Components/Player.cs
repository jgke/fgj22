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
using System.Linq;
using Fgj22.Spells.Spell;
using Fgj22.App.Utility;

namespace Fgj22.App.Components
{
    public class Player : Component, IUpdatable, ILoggable
    {
        private SpriteAnimator Animator;
        private TiledMapMover Mover;
        [Loggable]
        private BoxCollider BoxCollider;
        [Loggable]
        private CollisionState CollisionState = new CollisionState();

        private Editor Editor;
        private VirtualButton OpenEditor;

        private VirtualIntegerAxis XAxisInput;
        private VirtualIntegerAxis YAxisInput;

        private VirtualButton MeleeButton;
        public bool MeleeAttackActive = true;
        public double MeleeAttackDirection = 0;

        Vector2 velocity;

        private TmxMap Map;

        private List<Vector2> MovementPath;
        private int MovementPathPos = -1;

        public Player(TmxMap Map, Editor editor)
        {
            this.Map = Map;
            Editor = editor;
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
            Entity.AddComponent(new Team(Faction.Friendly, true));

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

            //Editor
            OpenEditor = new VirtualButton();
            OpenEditor.Nodes.Add(new VirtualButton.KeyboardKey(Keys.LeftControl));
        }

        public void UpdateAnimation()
        {
            if (Animator.IsAnimationActive("Melee") && Animator.IsRunning)
            {
                return;
            }

            if (MeleeAttackActive)
            {
                Animator.Play("Melee", SpriteAnimator.LoopMode.ClampForever);
                float duration = Animator.CurrentAnimation.Sprites.Length / (Animator.CurrentAnimation.FrameRate * Animator.Speed);
                Entity.TweenRotationDegreesTo(-45 + Entity.RotationDegrees, duration)
                    .SetLoops(LoopType.PingPong, 1)
                    .Start();
                return;
            }

            string animation = "Idle";

            if (velocity != Vector2.Zero)
            {

                float rotation = 180 - velocity.Angle2(new Vector2(0, 1));
                //Entity.Transform.Rotation = rotation * Mathf.Deg2Rad ;
                animation = "Run";
            }


            if (animation != null && !Animator.IsAnimationActive(animation))
            {
                Animator.Play(animation);
            }
        }

        public void Update()
        {
            if (OpenEditor.IsPressed)
            {
                Editor.SetVisibility(!Editor.Enabled);
            }

            if (!Editor.Enabled)
            {
                UpdateMovementPath();

                if (Animator.CurrentAnimationName != "Melee")
                {
                    UpdateFacingRotation();
                }

                MeleeAttackActive = MeleeButton.IsPressed || Input.LeftMouseButtonPressed;

                if (MeleeAttackActive)
                {
                    UpdateMeleeAttack();
                }
            }

            UpdateSpells();

            if (MovementPathPos != -1 && XAxisInput.Value == 0 && YAxisInput.Value == 0)
            {
                var direction = MovementPath[MovementPathPos] - Entity.Transform.Position;
                if (direction.Length() < 5)
                {
                    MovementPathPos += 1;
                    if (MovementPathPos < MovementPath.Count)
                    {
                        direction = MovementPath[MovementPathPos] - Entity.Transform.Position;
                    }
                    else
                    {
                        MovementPathPos = -1;
                    }
                }
                direction.Normalize();
                velocity = direction * GameState.Instance.PlayerSpeed;
            }
            else
            {
                MovementPath = new List<Vector2>();
                MovementPathPos = -1;
                velocity = new Vector2(XAxisInput.Value, YAxisInput.Value) * GameState.Instance.PlayerSpeed;
            }

            Mover.Move(velocity * Time.DeltaTime, BoxCollider, CollisionState);

            if (Animator.CurrentAnimationName != "Melee" && !Editor.Enabled)
            {
                Transform.Rotation = Transform.Rotation + (float)Math.PI / 2.0f;
            }
            UpdateAnimation();
        }

        private void UpdateMeleeAttack()
        {
            var meleeAttack = Entity.Scene.CreateEntity("meleeAttack", Entity.Transform.Position + new Vector2(30, 30));

            meleeAttack.AddComponent(new MeleeAttack(this, 10, 1, 30, -45));
            meleeAttack.AddComponent(new BoxCollider(30, 30));
            meleeAttack.AddComponent(new Team(Faction.Friendly, false));
        }

        private void UpdateMovementPath()
        {
            if (Input.RightMouseButtonPressed)
            {
                var start = Entity.Transform.Position;
                var end = Entity.Scene.Camera.MouseToWorldPoint();

                MovementPath = Entity.Scene.GetSceneComponent<PathFinder>().GetRoute(start, end);
                if (MovementPath != null)
                {
                    MovementPathPos = 0;
                }
                else
                {
                    MovementPathPos = -1;
                }
            }
        }

        private void UpdateFacingRotation()
        {
            var start = Entity.Transform.Position;
            var end = Entity.Scene.Camera.MouseToWorldPoint();

            Transform.Rotation = (end - start).GetAngle();
        }

        private void UpdateSpells()
        {
            if (Editor.Spells.Any())
            {
                var spell = Editor.Spells.Dequeue();

                if (spell is Fireball fb)
                {
                    var entity = Entity.Scene.CreateEntity("fireball", Entity.Transform.Position);
                    entity.AddComponent(new Emp(-fb.Angle * Math.PI / 180));
                }
                else if(spell is Zap)
                {
                    var entity = Entity.Scene.CreateEntity("stun", Entity.Transform.Position);
                    entity.AddComponent(new Stun());
                }
            }
        }

        public override void DebugRender(Batcher batcher)
        {
            base.DebugRender(batcher);
            var mouseLocation = Entity.Scene.Camera.MouseToWorldPoint();
            batcher.DrawPixel(mouseLocation.X, mouseLocation.Y, Debug.Colors.DebugText, 3);
        }
    }
}
