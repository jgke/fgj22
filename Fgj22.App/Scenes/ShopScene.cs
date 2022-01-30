using Nez.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using Fgj22.App.Components;
using Nez.Tiled;
using Nez.Textures;
using Fgj22.App.Systems;
using Fgj22.App.Utility;
using Nez.UI;
using System;

namespace Fgj22.App
{
    class Upgrade
    {
        string Id;
        Func<GameState, GameState> Action;
        string Text;
        string[] Dependencies;

        public Upgrade(string id, string text, Func<GameState, GameState> action, string[] dependencies = null)
        {
            this.Id = id;
            this.Text = text;
            this.Action = action;
            this.Dependencies = dependencies;
        }
    }

    class ShopComponent : Component
    {
        public override void OnAddedToEntity()
        {
            UICanvas canvas = new UICanvas();
            Entity.AddComponent(canvas);

            var table = canvas.Stage.AddElement(new Table());
            table.SetFillParent(true);

            var speedButton = new TextButton("Double speed", TextButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green));
            table.Add(speedButton).SetMinWidth(100).SetMinHeight(30);
            table.Row();
            speedButton.OnClicked += _ =>
            {
                GameState.Instance.PlayerSpeed *= 2;
            };

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