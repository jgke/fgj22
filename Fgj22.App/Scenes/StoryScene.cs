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

    class Line : StoryPiece
    {
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

        public void CreateUI(Table table, Entity entity, Action cycleStory)
        {
            table.Bottom();
            var img = new Image(entity.Scene.Content.LoadTexture("Content/" + Avatar));
            table.Add(img).Bottom().Width(100).Height(100);
            var button1 = new TextButton(Text, TextButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green));
            table.Add(button1).SetMinHeight(100).Expand().Bottom().SetFillX();
            table.Row();
            button1.OnClicked += _ => { cycleStory(); };
        }
    }

    class IncrementCounterBy : StoryPiece
    {
        public int Amount;

        public IncrementCounterBy(int amount)
        {
            this.Amount = amount;
        }

        public void CreateUI(Table table, Entity entity, Action cycleStory)
        {

            GameState.Instance.Counter += Amount;
            cycleStory();
        }
    }

    class Exposition : StoryPiece
    {
        public string Text;

        public Exposition(string text)
        {
            this.Text = text;
        }

        public void CreateUI(Table table, Entity entity, Action cycleStory)
        {
            table.Bottom();
            var button1 = new TextButton(Text, TextButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green));
            table.Add(button1).SetMinHeight(100).Expand().Bottom().SetFillX();
            button1.OnClicked += _ => { cycleStory(); };
        }
    }

    class Fork : StoryPiece
    {
        public int? choice = null;
        List<string> Choices;
        List<StoryBuilder> ChoiceBuilders;

        public Fork(List<string> choices, List<StoryBuilder> choiceBuilders)
        {
            Choices = choices;
            ChoiceBuilders = choiceBuilders;
        }

        public void CreateUI(Table table, Entity entity, Action cycleStory)
        {
            if (choice == null)
            {
                table.Bottom();
                for (int i = 0; i < Choices.Count; i++)
                {
                    int num = i;
                    var button1 = new TextButton(Choices[i], TextButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green));
                    table.Add(button1).SetMinHeight(100).Expand().Bottom().SetFillX();
                    button1.OnClicked += _ =>
                    {
                        table.Clear();
                        choice = num;
                        this.CreateUI(table, entity, cycleStory);
                    };
                }
            }
            else
            {
                Log.Information("{@A}", choice);
                Log.Information("{@A}", ChoiceBuilders);

                ChoiceBuilders[choice.Value].CreateUI(table, entity, cycleStory);
            }
        }
    }

    class GoToLevel : StoryPiece
    {
        public void CreateUI(Table table, Entity entity, Action cycleStory)
        {
            Core.StartSceneTransition(new WindTransition(() => new GameplayScene()));
        }
    }

    class ForkBuilder
    {
        public List<string> Choices;
        public List<StoryBuilder> ChoiceBuilders;

        public ForkBuilder()
        {
            Choices = new List<string>();
            ChoiceBuilders = new List<StoryBuilder>();
        }
        public ForkBuilder Choice(string text, StoryBuilder builder)
        {
            Choices.Add(text);
            ChoiceBuilders.Add(builder);
            return this;
        }
    }

    class CounterForkBuilder
    {
        public CounterForkBuilder() { }
        public CounterForkBuilder IfMoreThan(int amount, StoryBuilder innerContent)
        {
            throw new Exception("not implemented");
        }
        public CounterForkBuilder Otherwise(StoryBuilder innerContent)
        {
            throw new Exception("not implemented");
        }
    }

    class StoryBuilder : StoryPiece
    {
        public List<StoryPiece> lines;
        int currentStoryLine = 0;

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
            lines.Add(new IncrementCounterBy(amount));
            return this;
        }

        public StoryBuilder Exposition(string text)
        {
            lines.Add(new Exposition(text));
            return this;
        }

        public StoryBuilder Fork(ForkBuilder builder)
        {
            lines.Add(new Fork(builder.Choices, builder.ChoiceBuilders));
            return this;
        }

        public StoryBuilder CounterFork(CounterForkBuilder builder)
        {
            throw new Exception("not implemented");
        }

        public StoryBuilder GoToLevel()
        {
            lines.Add(new GoToLevel());
            return this;
        }

        public void CreateUI(Table table, Entity entity, Action cycleStory)
        {
            var line = lines[currentStoryLine];
            line.CreateUI(table, entity, () =>
            {
                if (currentStoryLine < lines.Count - 1)
                {
                    currentStoryLine += 1;
                    table.ClearChildren();
                    this.CreateUI(table, entity, cycleStory);
                }
                else
                {
                    cycleStory();
                }
            });
        }
    }

    class StoryComponent : Component, IUpdatable
    {
        StoryBuilder storyBuilder;
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
                    storyBuilder = new StoryBuilder()
                        .Line("SigrithrAvatar.png", "Sigrithr", "moi")
                        .LineRight("SigrithrAvatar.png", "Sigrithr", "moi toinen")
                        .Exposition("pelkkaa tarinatekstia")
                        .Fork(new ForkBuilder()
                            .Choice("Eka vaihtoehto", new StoryBuilder()
                                        .Line("SigrithrAvatar.png", "Sigrithr", "Valitsit ekan vaihtoedon")
                                        .IncrementCounterBy(1))
                            .Choice("Toka vaihtoehto", new StoryBuilder()
                                        .Line("SigrithrAvatar.png", "Sigrithr", "Valitsit tokan vaihtoedon")))
                        //.CounterFork(new CounterForkBuilder()
                        //    .IfMoreThan(0, new StoryBuilder()
                        //                .Line("Content/SigrithrAvatar.png", "Sigrithr", "Valitsit joskus ekan vaihtoehdon"))
                        //    .Otherwise( new StoryBuilder()
                        //                .Line("Content/SigrithrAvatar.png", "Sigrithr", "Et valinnut ekaa vaihtoehtoa")))
                        .Line("SigrithrAvatar.png", "Sigrithr", "tama on keskustelun loppu")
                        .GoToLevel();
                    break;

                default:
                    storyBuilder = new StoryBuilder()
                        .Line("Content/SigrithrAvatar.png", "Sigrithr", "moi");
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
            table.ClearChildren();
            storyBuilder.CreateUI(table, Entity, () =>
            {
                throw new Exception("unreachable");
            });
        }

        void IUpdatable.Update()
        {
            //if (StoryAdvanceButton.IsPressed)
            //{
            //    storyLine += 1;
            //    CycleStory();
            //}
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