using Nez.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using Fgj22.App.Components;
using Nez.Tiled;
using Nez.Textures;
using Fgj22.App.Systems;
using Fgj22.App.Utility;
using Nez.UI;
using System.Collections.Generic;
using Serilog;
using System;
using Microsoft.Xna.Framework.Input;

namespace Fgj22.App
{
    class StoryPiece
    {
        public string Character;
        public string Text;

        public StoryPiece(string character, string text)
        {
            this.Character = character;
            this.Text = text;
        }
    }

    class StoryBuilder
    {
        public List<StoryPiece> lines;
        public StoryBuilder()
        {
            lines = new List<StoryPiece>();
        }

        public StoryBuilder Line(string by, string what)
        {
            lines.Add(new StoryPiece(by, what));
            return this;
        }

        public Story Build()
        {
            return new Story(lines);
        }
    }

    class Story
    {
        public List<StoryPiece> Content;

        public Story(List<StoryPiece> content)
        {
            this.Content = content;
        }
    }

    class StoryComponent : Component, IUpdatable
    {
        int storyLine = 0;
        Story story;
        Table table;
        private VirtualButton StoryAdvanceButton;

        public override void OnAddedToEntity()
        {
            StoryAdvanceButton = new VirtualButton();
            StoryAdvanceButton.Nodes.Add(new VirtualButton.KeyboardKey(Keys.A));
            StoryAdvanceButton.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.A));

            Log.Information("Loading story {A}", GameState.Instance);
            switch (GameState.Instance.LevelNum)
            {
                case 0:
                    story = new StoryBuilder()
                        .Line("Content/SigrithrAvatar.png", "moi")
                        .Line("Content/VonNeumannAvatar.png", "no moi")
                        .Build();
                    break;

                default:
                    story = new StoryBuilder()
                        .Line("Content/SigrithrAvatar.png", "moi")
                        .Build();
                    break;
            }

            UICanvas canvas = new UICanvas();
            Entity.AddComponent(canvas);

            table = canvas.Stage.AddElement(new Table());
            table.SetFillParent(true);

            CycleStory();
        }

        public override void OnRemovedFromEntity()
        {
            StoryAdvanceButton.Deregister();
        }

        public void CycleStory()
        {
            if (storyLine >= story.Content.Count)
            {
                Core.StartSceneTransition(new WindTransition(() => new GameplayScene()));
            }
            else
            {
                var line = story.Content[storyLine];
                table.ClearChildren();
                table.Bottom();
                var img = new Image(Entity.Scene.Content.LoadTexture(line.Character));
                table.Add(img).Bottom().Width(100).Height(100);
                var button1 = new TextButton(line.Text, TextButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green));
                table.Add(button1).SetMinHeight(100).Expand().Bottom().SetFillX();
                table.Row();
                button1.OnClicked += _ =>
                {
                    CycleStory();
                };
                storyLine += 1;
            }
        }

        void IUpdatable.Update()
        {
            if (StoryAdvanceButton.IsPressed)
            {
                CycleStory();
            }
        }
    }

    public class StoryScene : ProgramScene
    {
        public override void Initialize()
        {
            base.Initialize();

            var menuEntity = CreateEntity("menu", new Vector2(0, 0));
            menuEntity.AddComponent(new StoryComponent());
        }
    }
}