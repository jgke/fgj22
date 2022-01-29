using Microsoft.Xna.Framework;
using Nez;
using System;

namespace Fgj22.App
{
    public static class Fgj22Vector2Ext
    {
        public static Vector2 Rotate(this Vector2 vec, double angle)
        {
            var radians = Math.PI * angle / 180;

            return new Vector2(
                (float)(vec.X * Math.Cos(radians) - vec.Y * Math.Sin(radians)),
                (float)(vec.X * Math.Sin(radians) + vec.Y * Math.Cos(radians))
            );
        }

        public static float Angle2(this Vector2 vec, Vector2 other)
        {
            return (float)(Math.Atan2(other.Y, other.X) - Math.Atan2(vec.Y, vec.X)) * Mathf.Rad2Deg;
        }

        public static Rectangle ToRect(this Vector2 pos, Vector2 size)
        {
            return new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
        }

        public static Vector2 Closest(this Vector2 target, params Vector2[] rest)
        {
            Vector2 best = target;
            float dist2 = float.MaxValue;

            foreach (var point in rest)
            {
                var dist = Vector2.DistanceSquared(target, point);
                if (dist < dist2)
                {
                    best = point;
                    dist2 = dist;
                }
            }

            return best;
        }

        public static float GetAngle(this Vector2 direction)
        {
            if (direction.X != 0)
            {
                var rotation = (float)Math.Atan(direction.Y / direction.X);
                if (direction.X < 0)
                {
                    return rotation + (float)Math.PI;
                }
                else
                {
                    return rotation;
                }
            }
            else
            {
                if (direction.Y < 0)
                {
                    return (float)Math.PI / 2.0f;
                }
                else
                {
                    return (float)Math.PI / 2.0f * 3.0f;
                }
            }
        }
    }
}
