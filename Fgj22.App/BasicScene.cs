using Nez.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using Fgj22.App.Components;
using Nez.Tiled;
using Nez.Textures;
using Fgj22.App.Systems;
using Fgj22.App.Utility;

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

            var map = Content.LoadTiledMap("Content/tiledMap.tmx");
			var topLeft = new Vector2(0, 0);
			var bottomRight = new Vector2(map.TileWidth * map.Width, map.TileWidth * map.Height);
            var tiledEntity = CreateEntity("tiled-map-entity");
            tiledEntity.AddComponent(new TiledMapRenderer(map, "main"));

            var cameraBounds = new CameraBounds(topLeft, bottomRight);

            var screenPosition = new ScreenPosition(cameraBounds);

            var editor = CreateEntity("editor");
            var editorComponent = new Editor(screenPosition);
            editor.AddComponent(editorComponent);

            var collisionLayer = map.GetLayer<TmxLayer>("main");
			var playerSpawn = map.GetObjectGroup("objects").Objects["spawn"];
			var playerSpawnPosition = new Vector2(playerSpawn.X, playerSpawn.Y);
            var playerEntity = CreateEntity("player", playerSpawnPosition);
            playerEntity.AddComponent(new Test(map, editorComponent));



			var enemySpawns = map.GetObjectGroup("enemies").Objects;
			foreach (TmxObject itemSpawnPoint in enemySpawns)
			{
				var enemy = CreateEntity("enemy", new Vector2(itemSpawnPoint.X, itemSpawnPoint.Y));
                enemy.AddComponent(new Enemy("Von Neumann Swarm"));
			}

            Camera.Entity.AddComponent(new FollowCamera(playerEntity));
			tiledEntity.AddComponent(cameraBounds);
            tiledEntity.AddComponent(cameraBounds);

            AddSceneComponent(new PathFinder(map));
        }
    }
}