using Fgj22.App.Systems;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fgj22.App;

namespace Fgj22.App.Components
{
    public class EnemyAI : Component, IUpdatable
    {

        private Player Target;
        private int FrequencyCounter = 0;
        private TmxMap Map;
        private readonly Enemy Parent;
        private TiledMapMover Mover;
        private Stack<Vector2> MovementPath;
        private readonly int Speed;
        private readonly int HoverDistance;

        public EnemyAI(Player target, TmxMap map, Enemy enemy, int speed = 1, int hoverDistance = 0)
        {
            Target = target;
            Map = map;
            Parent = enemy;
            Speed = speed;
            HoverDistance = hoverDistance;

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
                if (HoverDistance == 0)
                {
                    MovementPath = GetRoute(Target.Transform.Position);
                }
                else
                {
                    MovementPath = GetHoveringRoute(Target.Transform.Position);
                }
            }

            Vector2 velocity;
            Vector2 direction;
            if (MovementPath != null && MovementPath.Any())
            {
                direction = MovementPath.Peek() - Entity.Transform.Position;
                if (direction.Length() < 30)
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
            velocity = direction * Speed;

            Mover.Move(velocity, Parent.BoxCollider, Parent.CollisionState);

            Transform.Rotation = direction.GetAngle();

            FrequencyCounter += 1;
        }

        public Stack<Vector2> GetHoveringRoute(Vector2 targetPosition)
        {
            if(targetPosition.Pythagoras(Entity.Transform.Position) > HoverDistance * 3)
            {
                return GetRoute(targetPosition);
            }
            else
            {
                var target = GetRandomHoverPosition(targetPosition);
                for(int i = 0; i < 5; i ++)
                {
                    var route = GetRoute(target);

                    if(route != null)
                    {
                        return route;
                    }

                    target = GetRandomHoverPosition(targetPosition);
                }

                return null;
            }
        }

        Vector2 GetRandomHoverPosition(Vector2 targetPosition)
        {
            var positionVector = (Entity.Transform.Position - targetPosition);
            positionVector.Normalize();

            var xMod = ((Random.NextFloat() + 1) / 1.5f);
            var yMod = ((Random.NextFloat() + 1) / 1.5f);

            return new Vector2(xMod * positionVector.X, yMod * positionVector.Y) * HoverDistance;
        }

        public Stack<Vector2> GetRoute(Vector2 targetPosition)
        {
            var route = Entity.Scene.GetSceneComponent<PathFinder>().GetRoute(Entity.Transform.Position, targetPosition);

            if (route != null && route.Any())
            {
                route.Reverse();
                return new Stack<Vector2>(route);
            }

            return null;
        }


    }
}
