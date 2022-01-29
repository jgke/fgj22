using Microsoft.Xna.Framework;
using Nez;
using System;
using Nez.Textures;
using Nez.Sprites;

namespace Fgj22.App.Components
{
    public class Enemy : Component, ILoggable
    {
        private SpriteAnimator Animator;
        [Loggable]
        string ty;
        public Enemy(string ty)
        {
            this.ty = ty;
        }

        public override void OnAddedToEntity()
        {
            int health;
            int collisionDamage;
            string sprite;

            switch (ty)
            {
                case "standard":
                    health = 5;
                    collisionDamage = 1;
                    sprite = "Content/caveman.png";
                    break;

                default:
                    Console.WriteLine("Created unknown enemy type (replacing with standardEnemy): " + ty);
                    health = 5;
                    collisionDamage = 1;
                    sprite = "Content/caveman.png";
                    break;
            }

            Entity.AddComponent(new Health(health));
            Entity.AddComponent(new Damage(collisionDamage, true));
            Entity.AddComponent(new BoxCollider(-8, -16, 16, 32));
            Entity.AddComponent(new Team(2));

            var texture = Entity.Scene.Content.LoadTexture(sprite);
            var sprites = Sprite.SpritesFromAtlas(texture, 32, 32);
            // todo: better way to handle animations
            Animator = Entity.AddComponent(new SpriteAnimator(sprites[0]));
            Animator.AddAnimation("Stay", new[]
            {
                sprites[8 + 0],
            });
            Animator.Play("Stay");
        }
    }
}
