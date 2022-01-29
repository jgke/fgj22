using Nez.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using Fgj22.App.Components;
using Nez.Tiled;
using Nez.Textures;
using Fgj22.App.Systems;

namespace Fgj22.App
{
    public class BasicScene : SampleScene
    {
        public override void Initialize()
        {
            base.Initialize();

            // Set the playing area size
            SetDesignResolution(480, 320, SceneResolutionPolicy.ShowAllPixelPerfect);
            // Set the rendering resolution
            Screen.SetSize(1440, 1280);

            var editor = CreateEntity("editor", new Vector2(300, 100));
            editor.AddComponent(new Editor());

            var map = Content.LoadTiledMap("Content/tiledMap.tmx");
            var collisionLayer = map.GetLayer<TmxLayer>("main");
			var playerSpawn = map.GetObjectGroup("objects").Objects["spawn"];
			var playerSpawnPosition = new Vector2(playerSpawn.X, playerSpawn.Y);
            var playerEntity = CreateEntity("player", playerSpawnPosition);
            playerEntity.AddComponent(new Test(map));

            var tiledEntity = CreateEntity("tiled-map-entity");
            tiledEntity.AddComponent(new TiledMapRenderer(map, "main"));

			var enemySpawns = map.GetObjectGroup("enemies").Objects;
			foreach (TmxObject itemSpawnPoint in enemySpawns)
			{
				var enemy = CreateEntity("enemy", new Vector2(itemSpawnPoint.X, itemSpawnPoint.Y));
                enemy.AddComponent(new Enemy("standard"));
			}

            Camera.Entity.AddComponent(new FollowCamera(playerEntity));
			var topLeft = new Vector2(0, 0);
			var bottomRight = new Vector2(map.TileWidth * map.Width, map.TileWidth * map.Height);
			tiledEntity.AddComponent(new CameraBounds(topLeft, bottomRight));
            tiledEntity.AddComponent(new CameraBounds(topLeft, bottomRight));

            AddSceneComponent(new PathFinder(map));
        }
    }
}