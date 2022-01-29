using Nez;
using Nez.Tiled;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Nez.AI.Pathfinding;
using Serilog;
using System.Linq;

namespace Fgj22.App.Systems {
    public class PathFinder : SceneComponent {
        TmxMap Map;
        TmxLayer CollisionLayer;
        AstarGridGraph AstarGraph;
			
        public PathFinder(TmxMap Map) {
            this.Map = Map;
            CollisionLayer = Map.GetLayer<TmxLayer>("main");
            AstarGraph = new AstarGridGraph(CollisionLayer);
        }

        // Get a route from `from` to `to`, in map coordinates (not world!)
        public List<Vector2> GetRoute(Vector2 from, Vector2 to) {
            var start = Map.WorldToTilePosition(from);
            var end = Map.WorldToTilePosition(to);

            Log.Information("Plotting a route from {start} to {end}", from, to);
            Log.Information("Map positions: {start} to {end}", start, end);

            var path = AstarGraph.Search(start, end);
            Log.Information("Final route: {path}", path);
            return path.Select(pos => Map.TileToWorldPosition(pos)).ToList();
        }

        public override void Update() { }
    }
}
