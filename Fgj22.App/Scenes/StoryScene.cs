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
    interface StoryPiece
    {
        void CreateUI(Table table, Entity entity, Action cycleStory);
    }

    class Line : StoryPiece {
        public string Avatar;
        public string Character;
        public string Text;
        public bool CharacterIsRight;

        public Line(string avatar, string character, string text, bool characteIsRight)
        {
            this.Avatar = avatar;
            this.Character = character;
            this.Text = text;
            this.CharacterIsRight = characteIsRight;
        }

        public void CreateUI(Table table, Entity entity, Action cycleStory) {
            table.Bottom();
            var img = new Image(entity.Scene.Content.LoadTexture(Character));
            table.Add(img).Bottom().Width(100).Height(100);
            var button1 = new TextButton(Text, TextButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green));
            table.Add(button1).SetMinHeight(100).Expand().Bottom().SetFillX();
            table.Row();
            button1.OnClicked += _ => { cycleStory(); };
        }
    }

    class ForkBuilder
    {
        public ForkBuilder() {}
        public ForkBuilder Choice(string text, StoryBuilder innerContent) {
            throw new Exception("not implemented");
        }
    }

    class CounterForkBuilder
    {
        public CounterForkBuilder() {}
        public CounterForkBuilder IfMoreThan(int amount, StoryBuilder innerContent) {
            throw new Exception("not implemented");
        }
        public CounterForkBuilder Otherwise(StoryBuilder innerContent) {
            throw new Exception("not implemented");
        }
    }

    class StoryBuilder
    {
        public List<StoryPiece> lines;
        public StoryBuilder()
        {
            lines = new List<StoryPiece>();
        }

        public StoryBuilder Line(string avatar, string by, string what)
        {
            lines.Add(new Line(avatar, by, what, false));
            return this;
        }

        public StoryBuilder LineRight(string avatar, string by, string what)
        {
            lines.Add(new Line(avatar, by, what, true));
            return this;
        }

        public StoryBuilder IncrementCounterBy(int amount)
        {
            GameState.Instance.Counter += amount;
            return this;
        }

        public StoryBuilder Exposition(string text)
        {
            throw new Exception("not implemented");
        }

        public StoryBuilder Fork(ForkBuilder builder)
        {
            throw new Exception("not implemented");
        }

        public StoryBuilder CounterFork(CounterForkBuilder builder)
        {
            throw new Exception("not implemented");
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
            StoryBuilder storyBuilder;

            Log.Information("Loading story {A}", GameState.Instance);
            switch (GameState.Instance.LevelNum)
            {
                case 0:
                    storyBuilder = new StoryBuilder()
                        .Line("SigrithrAvatar.png", "Sigrithr", "moi")
                        .LineRight("SigrithrAvatar.png", "Sigrithr", "moi")
                        .Exposition("pelkkää tarinatekstiä")
                        .Fork(new ForkBuilder()
                            .Choice("Eka vaihtoehto", new StoryBuilder()
                                        .Line("Content/SigrithrAvatar.png", "Sigrithr", "Valitsit ekan vaihtoedon")
                                        .IncrementCounterBy(1))
                            .Choice("Toka vaihtoehto",  new StoryBuilder()
                                        .Line("Content/SigrithrAvatar.png", "Sigrithr", "Valitsit tokan vaihtoedon")))
                        .CounterFork(new CounterForkBuilder()
                            .IfMoreThan(0, new StoryBuilder()
                                        .Line("Content/SigrithrAvatar.png", "Sigrithr", "Valitsit joskus ekan vaihtoehdon"))
                            .Otherwise( new StoryBuilder()
                                        .Line("Content/SigrithrAvatar.png", "Sigrithr", "Et valinnut ekaa vaihtoehtoa")))
                        .Line("Content/SigrithrAvatar.png", "Sigrithr", "tämä on keskustelun loppu");
                    break;

                default:
                    storyBuilder = new StoryBuilder()
                        .Line("Content/SigrithrAvatar.png", "Sigrithr", "moi");
                    break;
            }

            story = storyBuilder.Build();

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
                line.CreateUI(table, Entity, CycleStory);
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