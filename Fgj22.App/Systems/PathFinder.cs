using Nez;
using Nez.Tiled;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Nez.AI.Pathfinding;
using Serilog;
using System.Linq;
using System;

namespace Fgj22.App.Systems
{
    public class PathFinder : SceneComponent
    {
        TmxMap Map;
        TmxLayer CollisionLayer;
        AstarGridGraph AstarGraph;

        private static bool HasCollisionInRange(TmxLayer Layer, int x, int y, int dx, int dy)
        {
            for (int y_ = Math.Max(0, y - dy); y_ <= Math.Min(y + dy, Layer.Height - 1); y_++)
            {
                for (int x_ = Math.Max(0, x - dx); x_ <= Math.Min(x + dx, Layer.Width - 1); x_++)
                {
                    if (Layer.GetTile(x_, y_) != null)
                        return true;
                }
            }
            return false;
        }

        public PathFinder(TmxMap Map)
        {
            this.Map = Map;
            CollisionLayer = Map.GetLayer<TmxLayer>("main");
            AstarGraph = new AstarGridGraph(CollisionLayer.Width, CollisionLayer.Height);

            for (var y = 0; y < CollisionLayer.Map.Height; y++)
            {
                for (var x = 0; x < CollisionLayer.Map.Width; x++)
                {
                    if (PathFinder.HasCollisionInRange(CollisionLayer, x, y, 1, 1))
                    {
                        AstarGraph.Walls.Add(new Point(x, y));
                    }
                }
            }
            /*
            for (var y = 0; y < CollisionLayer.Map.Height; y++)
            {
                for (var x = 0; x < CollisionLayer.Map.Width; x++)
                {
                    if(CollisionLayer.GetTile(x, y) != null) {
                        Console.Write("#");
                    } else {
                        Console.Write(".");
                    }
                }
                Console.WriteLine("");
            }
            Console.WriteLine("--");

            for (var y = 0; y < CollisionLayer.Map.Height; y++)
            {
                for (var x = 0; x < CollisionLayer.Map.Width; x++)
                {
                    if(AstarGraph.Walls.Contains(new Point(x, y))) {
                        Console.Write("#");
                    } else {
                        Console.Write(".");
                    }
                }
                Console.WriteLine("");
            }
            Console.WriteLine("--");
            */
        }

        // Get a route from `from` to `to`, in map coordinates (not world!)
        public List<Vector2> GetRoute(Vector2 from, Vector2 to)
        {
            var start = Map.WorldToTilePosition(from);
            var end = Map.WorldToTilePosition(to);

            Log.Information("Plotting a route from {start} to {end}", from, to);
            Log.Information("Map positions: {start} to {end}", start, end);

            var path = AstarGraph.Search(start, end);
            Log.Information("Final route: {path}", path);
            if (path != null)
            {
                var convertedPath =
                    path.Select(pos => Map.TileToWorldPosition(pos) + new Vector2(Map.TileWidth, Map.TileHeight) / 2).ToList();
                Log.Information("Final route in world coordinates: {path}", convertedPath);
                return convertedPath;
            }
            else
            {
                return null;
            }
        }

        public override void Update() { }
    }
}
