using Nez.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using Fgj22.App.Components;
using Nez.Tiled;
using Nez.Textures;
using Fgj22.App.Systems;
using Fgj22.App.Utility;
using Nez.UI;

namespace Fgj22.App
{
    class ShopComponent : Component
    {
        public override void OnAddedToEntity()
        {
            UICanvas canvas = new UICanvas();
            Entity.AddComponent(canvas);

            var table = canvas.Stage.AddElement(new Table());
            table.SetFillParent(true);

            var button1 = new TextButton("Continue game", TextButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green));
            table.Add(button1).SetMinWidth(100).SetMinHeight(30);
            table.Row();
            button1.OnClicked += _ =>
            {
                GameState.Instance.LevelNum += 1;
                Core.StartSceneTransition(new WindTransition(() => new StoryScene()));
            };
        }
    }

    public class ShopScene : ProgramScene
    {
        public override void Initialize()
        {
            base.Initialize();

            var menuEntity = CreateEntity("menu", new Vector2(0, 0));
            menuEntity.AddComponent(new ShopComponent());
        }
    }
}