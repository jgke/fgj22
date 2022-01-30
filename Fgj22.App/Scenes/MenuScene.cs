using Nez.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Nez;
using Fgj22.App.Components;
using Nez.Tiled;
using Nez.Textures;
using Fgj22.App.Systems;
using Fgj22.App.Utility;
using Nez.UI;
using System;
using System.IO;
using Triton.Audio.Decoders;

namespace Fgj22.App
{
    class MenuComponent : Component
    {
        public override void OnAddedToEntity()
        {
            Entity.AddComponent(new MusicPlayer("music/Menu_You_reAHackerLarry.ogg"));
            UICanvas canvas = new UICanvas();
            Entity.AddComponent(canvas);

            var table = canvas.Stage.AddElement(new Table());
            table.SetFillParent(true);

            var titleLabel = new Label("Heroes of Materiel and Logic", new LabelStyle()
            {
                FontColor = Color.Black,
                FontScaleX = 3,
                FontScaleY = 3,
            });
            table.Add(titleLabel);
            table.Row();

            var button1 = new TextButton("Start game", TextButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green));
            table.Add(button1).SetMinWidth(100).SetMinHeight(30);
            table.Row();
            button1.OnClicked += _ =>
            {
                GameState.Instance.DoTransition(() => new StoryScene());
            };
            var button2 = new TextButton("Quit game", TextButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green));
            table.Add(button2).SetMinWidth(100).SetMinHeight(30);
            table.Row();
            button2.OnClicked += _ =>
            {
                Core.Exit();
            };
        }
    }

    public class MenuScene : ProgramScene
    {
        public override void Initialize()
        {
            base.Initialize();

            var menuEntity = CreateEntity("menu", new Vector2(0, 0));
            menuEntity.AddComponent(new MenuComponent());
        }
    }
}