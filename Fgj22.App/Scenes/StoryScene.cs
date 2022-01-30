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
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Fgj22.App
{
    interface StoryPiece
    {
        void CreateUI(Table table, Entity entity, Action cycleStory);
        void DoCycle(Action cycleStory)
        {
            cycleStory();
        }
    }

    static class UiComponents
    {
        public static TextButton WrappingTextButton(string Text, Action act)
        {
            var button1 = new TextButton(Text, TextButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green));
            button1.GetLabel().SetWrap(true);
            button1.OnClicked += _ =>
            {
                act();
            };
            return button1;
        }

        public static string SafeString(string inputString)
        {
            return Encoding.ASCII.GetString(
                Encoding.Convert(
                    Encoding.UTF8,
                    Encoding.GetEncoding(
                        Encoding.ASCII.EncodingName,
                        new EncoderReplacementFallback("_"),
                        new DecoderExceptionFallback()
                        ),
                    Encoding.UTF8.GetBytes(inputString)
                ));
        }
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
            this.Character = UiComponents.SafeString(character);
            this.Text = UiComponents.SafeString(text);
            this.CharacterIsRight = characteIsRight;
        }

        public void CreateUI(Table mainTable, Entity entity, Action cycleStory)
        {
            Log.Information("Line {@A}", this);
            mainTable.Bottom();
            var table = new Table();
            mainTable.Add(table).Expand().Bottom().SetFillX();

            LabelStyle labelStyle = new LabelStyle()
            {
                FontColor = Color.Black,
                Background = new PrimitiveDrawable(Color.DarkGray)
            };

            if (this.CharacterIsRight)
            {
                var fakeTitle = new Label(" ", labelStyle);
                table.Add(fakeTitle).Expand().Bottom().SetFillX();

                var title = new Label(Character, labelStyle);
                title.SetAlignment(Align.Right, Align.Bottom);
                table.Add(title).Bottom().Width(100);
                table.Row();

                var button1 = UiComponents.WrappingTextButton(Text, cycleStory);
                table.Add(button1).SetMinHeight(100).Expand().Bottom().SetFillX();

                var img = new Image(entity.Scene.Content.LoadTexture("Content/" + Avatar));
                table.Add(img).Bottom().Width(100).Height(100);
            }
            else
            {
                var title = new Label(Character, labelStyle);
                title.SetAlignment(Align.Left, Align.Bottom);
                table.Add(title).Bottom().Width(100);
                var fakeTitle = new Label(" ", labelStyle);
                table.Add(fakeTitle).Expand().Bottom().SetFillX();
                table.Row();

                var img = new Image(entity.Scene.Content.LoadTexture("Content/" + Avatar));
                table.Add(img).Bottom().Width(100).Height(100);

                var button1 = UiComponents.WrappingTextButton(Text, cycleStory);
                table.Add(button1).SetMinHeight(100).Expand().Bottom().SetFillX();
            }
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
            Log.Information("IncrementCounterBy {@A}", this);
            DoCycle(cycleStory);
        }

        public void DoCycle(Action cycleStory)
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
            this.Text = UiComponents.SafeString(text);
        }

        public void CreateUI(Table table, Entity entity, Action cycleStory)
        {
            Log.Information("Exposition {@A}", this);
            table.Bottom();
            var button1 = UiComponents.WrappingTextButton(Text, cycleStory);
            table.Add(button1).SetMinHeight(100).Expand().Bottom().SetFillX();
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
            Log.Information("Fork {@A}", this);
            if (choice == null)
            {
                Log.Information("Fork choice null");
                table.Bottom();
                for (int i = 0; i < Choices.Count; i++)
                {
                    int num = i;
                    var button1 = UiComponents.WrappingTextButton(UiComponents.SafeString(Choices[i]), () =>
                    {
                        table.Clear();
                        choice = num;
                        this.CreateUI(table, entity, cycleStory);
                    });
                    table.Add(button1).SetMinHeight(100).Expand().Bottom().SetFillX();
                }
            }
            else
            {
                Log.Information("Fork choice {@A}", choice);

                ChoiceBuilders[choice.Value].CreateUI(table, entity, cycleStory);
            }
        }
    }

    class CounterFork : StoryPiece
    {
        List<Func<int, bool>> Conditions;
        List<StoryBuilder> Builders;

        public CounterFork(List<Func<int, bool>> conditions, List<StoryBuilder> builders)
        {
            Conditions = conditions;
            Builders = builders;
        }

        public void CreateUI(Table table, Entity entity, Action cycleStory)
        {
            Log.Information("CounterFork {@A}", this);
            for (int i = 0; i < Conditions.Count; i++)
            {
                if (Conditions[i](GameState.Instance.Counter))
                {
                    Builders[i].CreateUI(table, entity, cycleStory);
                    return;
                }
            }
            throw new Exception("Unreachable");
        }
    }

    class GoToLevel : StoryPiece
    {
        public void CreateUI(Table table, Entity entity, Action cycleStory)
        {
            Log.Information("GoToLevel {@A}", this);
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
        public List<Func<int, bool>> Conditions;
        public List<StoryBuilder> Builders;
        public CounterForkBuilder()
        {
            Conditions = new List<Func<int, bool>>();
            Builders = new List<StoryBuilder>();
        }
        public CounterForkBuilder IfMoreThan(int amount, StoryBuilder innerContent)
        {
            Conditions.Add(n => n > amount);
            Builders.Add(innerContent);
            return this;
        }
        public CounterForkBuilder Otherwise(StoryBuilder innerContent)
        {
            Conditions.Add(n => true);
            Builders.Add(innerContent);
            return this;
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
            lines.Add(new CounterFork(builder.Conditions, builder.Builders));
            return this;
        }

        public StoryBuilder GoToLevel()
        {
            lines.Add(new GoToLevel());
            return this;
        }

        public void CreateUI(Table table, Entity entity, Action cycleStory)
        {
            Log.Information("StoryBuilder choice={@A} {@B}", currentStoryLine, this);
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
                        .Exposition("29. September 2122, CMV Amehait, äTrojan asteroid cluster")
                        .LineRight("СmdMcEnroeAvatar.png", "Commander McEnroe", "Thank you yet again, Dosser! How do you always manage to fix my PDA no matter how jumbled up I manage to get it?")
                        .Line("SigrithrAvatar.png", "Sigrithr", "I'm not quite certain myself, ma'am, I just reinstalled Terracotta Contortionist and turned it off and on again. It's probably a driver issue.")

                        .Line("LarryAvatar.png", "SW Spp. O. Dosser", "")
                        .LineRight("СmdMcEnroeAvatar.png", "Commander McEnroe", "")
                        .Exposition("")
                        .Fork(new ForkBuilder()
                            .Choice("Eka vaihtoehto", new StoryBuilder()
                                        .Line("SigrithrAvatar.png", "Sigrithr", "Valitsit ekan vaihtoedon")
                                        .IncrementCounterBy(1))
                            .Choice("Toka vaihtoehto", new StoryBuilder()
                                        .Line("SigrithrAvatar.png", "Sigrithr", "Valitsit tokan vaihtoedon")))
                        .CounterFork(new CounterForkBuilder()
                            .IfMoreThan(0, new StoryBuilder()
                                        .Line("SigrithrAvatar.png", "Sigrithr", "Valitsit joskus ekan vaihtoehdon"))
                            .Otherwise(new StoryBuilder()
                                        .Line("SigrithrAvatar.png", "Sigrithr", "Et valinnut ekaa vaihtoehtoa")))
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