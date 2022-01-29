using Fgj22.App.Systems;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fgj22.App;

namespace Fgj22.App.Components
{
    public class EnemyAI : Component, IUpdatable
    {
        private Test Target;
        private int FrequencyCounter = 0;
        private TmxMap Map;
        private readonly Enemy Parent;
        private TiledMapMover Mover;
        private Stack<Vector2> MovementPath;

        public EnemyAI(Test target, TmxMap map, Enemy enemy)
        {
            Target = target;
            Map = map;
            Parent = enemy;
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            var collisionLayer = Map.GetLayer<TmxLayer>("main");
            Mover = new TiledMapMover(collisionLayer);
            Entity.AddComponent(Mover);
        }

        public void Update()
        {
            FrequencyCounter %= 30;

            if (FrequencyCounter == 0)
            {
                var route = Entity.Scene.GetSceneComponent<PathFinder>().GetRoute(Entity.Transform.Position, Target.Transform.Position);

                if(route != null && route.Any())
                {
                    route.Reverse();
                    MovementPath = new Stack<Vector2>(route);
                }
            }

            Vector2 velocity;
            Vector2 direction;
            if (MovementPath.Any())
            {
                direction = MovementPath.Peek() - Entity.Transform.Position;
                if (direction.Length() < 10)
                {
                    MovementPath.Pop();

                    if (MovementPath.Any())
                    {
                        direction = MovementPath.Peek() - Entity.Transform.Position;
                    }
                }
            }
            else
            {
                direction = Target.Transform.Position - Entity.Transform.Position;
            }
            direction.Normalize();
            velocity = direction * 1; // TODO

            Mover.Move(velocity, Parent.BoxCollider, Parent.CollisionState);


            Transform.Rotation = direction.GetAngle();
        }

        
    }
}
