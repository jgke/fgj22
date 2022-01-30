using Nez.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using Fgj22.App.Components;
using Nez.Tiled;
using Nez.Textures;
using Fgj22.App.Systems;
using Fgj22.App.Utility;
using System;
using Nez.Particles;
using Serilog;

namespace Fgj22.App
{
    public class GameplayScene : ProgramScene
    {
        float CompletedTimer = 2;

        public override void Initialize()
        {
            base.Initialize();

            var musicEntity = CreateEntity("musicEntity");
            musicEntity.AddComponent(new MusicPlayer("music/You_reAHackerLarry.ogg"));

            var map = Content.LoadTiledMap("Content/tiledMap.tmx");
            var topLeft = new Vector2(0, 0);
            var bottomRight = new Vector2(map.TileWidth * map.Width, map.TileWidth * map.Height);
            var tiledEntity = CreateEntity("tiled-map-entity");
            tiledEntity.AddComponent(new TiledMapRenderer(map, "main"))
                .SetRenderLayer(100);

            var cameraBounds = new CameraBounds(topLeft, bottomRight);

            var screenPosition = new ScreenPosition(cameraBounds);

            var editor = CreateEntity("editor");
            var editorComponent = new Editor(screenPosition);
            editor.AddComponent(editorComponent);

            var collisionLayer = map.GetLayer<TmxLayer>("main");
            var playerSpawn = map.GetObjectGroup("objects").Objects["spawn"];
            var playerSpawnPosition = new Vector2(playerSpawn.X, playerSpawn.Y);
            var playerEntity = CreateEntity("player", playerSpawnPosition);

            var player = new Player(map, editorComponent);
            playerEntity.AddComponent(player);

            var enemySpawns = map.GetObjectGroup("enemies").Objects;
            foreach (TmxObject itemSpawnPoint in enemySpawns)
            {
                var enemy = CreateEntity("enemy", new Vector2(itemSpawnPoint.X, itemSpawnPoint.Y));
                enemy.AddComponent(new Enemy("Macrotardigrade", player, map));
            }

            var shootingEnemy = CreateEntity("enemy", new Vector2(200, 200));
            shootingEnemy.AddComponent(new Enemy("asd", player, map));

            Camera.Entity.AddComponent(new FollowCamera(playerEntity));
            tiledEntity.AddComponent(cameraBounds);
            tiledEntity.AddComponent(cameraBounds);

            CreateEntity("progressBar").AddComponent(new ProgressBar());

            AddSceneComponent(new PathFinder(map));
        }

        public override void Update()
        {
            base.Update();

            var player = this.FindComponentOfType<Player>();

            if (player == null)
            {
                throw new Exception("You lost");
            }

            var enemy = this.FindComponentOfType<Enemy>();

            if (enemy == null)
            {
                CompletedTimer -= Time.DeltaTime;
            }
            if (CompletedTimer < 0)
            {
                CompletedTimer = 999999;
                Core.StartSceneTransition(new WindTransition(() => new ShopScene()));
            }
        }
    }
}