using Microsoft.Xna.Framework;
using Nez;
using System;
using Nez.Textures;
using Nez.Sprites;
using Fgj22.App.Utility;
using Nez.Tiled;

namespace Fgj22.App.Components
{
    public class Enemy : Component, ILoggable
    {
        private SpriteAnimator Animator;
        [Loggable]
        string ty;
        private readonly Test Player;
        private readonly TmxMap Map;

        public Enemy(string ty, Test player, TmxMap map)
        {
            this.ty = ty;
            Player = player;
            Map = map;
        }

        public BoxCollider BoxCollider { get; internal set; }
        public TiledMapMover.CollisionState CollisionState { get; internal set; } = new TiledMapMover.CollisionState();


        public override void OnAddedToEntity()
        {
            int health;
            int collisionDamage;
            string sprite;
            Microsoft.Xna.Framework.Graphics.Texture2D texture;
            System.Collections.Generic.List<Sprite> sprites;
            Sprite[] stayAnimationFrames;

            switch (ty)
            {
                case "Lone Von Neumann":
                    health = 2;
                    collisionDamage = 1;
                    sprite = "Content/Lone Von Neumann.png";
                    texture = Entity.Scene.Content.LoadTexture(sprite);
                    sprites = Sprite.SpritesFromAtlas(texture, 64, 64);
                    stayAnimationFrames = new[]
                    {
                        sprites[0 + 0],
                        sprites[0 + 1],
                        sprites[0 + 2],
                        sprites[0 + 3],
                        sprites[0 + 4]
                    };
                    break;

                case "Von Neumann Swarm":
                    health = 7;
                    collisionDamage = 2;
                    sprite = "Content/Von Neumann Swarm.png";
                    texture = Entity.Scene.Content.LoadTexture(sprite);
                    sprites = Sprite.SpritesFromAtlas(texture, 64, 64);
                    stayAnimationFrames = new[]
                    {
                        sprites[0 + 0],
                        sprites[0 + 1],
                        sprites[0 + 2],
                        sprites[0 + 3],
                        sprites[0 + 4]
                    };
                    break;

                default:
                    Console.WriteLine("Created unknown enemy type (replacing with standardEnemy): " + ty);
                    health = 5;
                    collisionDamage = 1;
                    sprite = "Content/caveman.png";
                    texture = Entity.Scene.Content.LoadTexture(sprite);
                    sprites = Sprite.SpritesFromAtlas(texture, 64, 64);
                    stayAnimationFrames = new[]
                    {
                        sprites[8 + 0],
                    };
                    break;
            }

            BoxCollider = new BoxCollider(-8, -16, 16, 32);

            Entity.AddComponent(new Health(health));
            Entity.AddComponent(new Damage(collisionDamage, false));
            Entity.AddComponent(BoxCollider);
            Entity.AddComponent(new Team(Faction.Enemy, true));
            Entity.AddComponent(new EnemyAI(Player, Map, this));

            // todo: better way to handle animations
            Animator = Entity.AddComponent(new SpriteAnimator(sprites[0]));
            Animator.AddAnimation("Stay", stayAnimationFrames);
            Animator.Play("Stay");
        }
    }
}
