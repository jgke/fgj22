using Nez.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using Fgj22.App.Components;
using Nez.Tiled;
using Nez.Textures;

namespace Fgj22.App
{
    [SampleScene("Basic Scene", 9999, "Scene with a single Entity. The minimum to have something to show")]
    public class BasicScene : SampleScene
    {
        public override void Initialize()
        {
            base.Initialize();

            // default to 1280x720 with no SceneResolutionPolicy
            SetDesignResolution(1280, 720, SceneResolutionPolicy.None);
            Screen.SetSize(1280, 720);

            var editor = CreateEntity("editor", new Vector2(300, 100));
            editor.AddComponent(new Editor());

            var playerEntity = CreateEntity("player", new Vector2(Screen.Width / 2 - 200, Screen.Height / 2 - 200));
            playerEntity.AddComponent(new Test());
            playerEntity.AddComponent(new BoxCollider(-8, -16, 16, 32));
            var map = Content.LoadTiledMap("Content/tiledMap.tmx");
            playerEntity.AddComponent(new TiledMapMover(map.GetLayer<TmxLayer>("main")));

            var tiledEntity = CreateEntity("tiled-map-entity");
            tiledEntity.AddComponent(new TiledMapRenderer(map, "main"));

            var enemy = CreateEntity("enemy", new Vector2(Screen.Width / 2 - 200, Screen.Height / 2 - 100));
            enemy.AddComponent(new Enemy("standard"));
        }
    }
}