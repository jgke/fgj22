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
        bool Skip(Action cycleStory)
        {
            Log.Information("StoryPiece Skip {@A}", this);
            DoCycle(cycleStory);
            return true;
        }
    }

    public static class UiComponents
    {
        public static TextButton WrappingTextButton(string Text, Action act)
        {
            var button1 = new TextButton(Text, TextButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green));
            button1.GetLabel().SetWrap(true);
            button1.PadLeft(10);
            button1.PadRight(10);
          
            button1.OnClicked += _ =>
            {
                if(!GameState.Instance.Transitioning) {
                    act();
                }
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
                table.Add(title).Bottom().Width(64);
                table.Row();

                var button1 = UiComponents.WrappingTextButton(Text, cycleStory);
                table.Add(button1).SetMinHeight(64).Expand().Bottom().SetFillX();

                var img = new Image(entity.Scene.Content.LoadTexture("Content/" + Avatar));
                table.Add(img).Bottom().Width(64).Height(64);
            }
            else
            {
                var title = new Label(Character, labelStyle);
                title.SetAlignment(Align.Left, Align.Bottom);
                table.Add(title).Bottom().Width(64);
                var fakeTitle = new Label(" ", labelStyle);
                table.Add(fakeTitle).Expand().Bottom().SetFillX();
                table.Row();

                var img = new Image(entity.Scene.Content.LoadTexture("Content/" + Avatar));
                table.Add(img).Bottom().Width(64).Height(64);

                var button1 = UiComponents.WrappingTextButton(Text, cycleStory);
                table.Add(button1).SetMinHeight(64).Expand().Bottom().SetFillX();
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

        public bool Skip(Action cycleStory) {
            Log.Information("Fork Skip {@A}", this);
            return false;
        }
    }

    class CounterFork : StoryPiece
    {
        List<Func<int, bool>> Conditions;
        List<StoryBuilder> Builders;
        StoryBuilder currentStory = null;

        public CounterFork(List<Func<int, bool>> conditions, List<StoryBuilder> builders)
        {
            Conditions = conditions;
            Builders = builders;
        }

        public void CreateUI(Table table, Entity entity, Action cycleStory)
        {
            if(currentStory == null) {
            Log.Information("CounterFork {@A}", this);
            for (int i = 0; i < Conditions.Count; i++)
            {
                if (Conditions[i](GameState.Instance.Counter))
                {
                    currentStory = Builders[i];
                    break;
                }
            }

            currentStory.CreateUI(table, entity, cycleStory);
        }
        }
        public bool Skip(Action cycleStory) {
            Log.Information("CounterFork Skip {@A}", this);
            return currentStory.Skip(cycleStory);
        }
    }

    class GoToLevel : StoryPiece
    {
        public void CreateUI(Table table, Entity entity, Action cycleStory)
        {
            Log.Information("GoToLevel {@A}", this);
            GameState.Instance.DoTransition(() => new GameplayScene());
        }
        public bool Skip(Action cycleStory) {
            Log.Information("GoToLevel Skip {@A}", this);
            return false;
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

        public bool Skip(Action cycleStory) {
            Log.Information("StoryBuilder Skip {@A}", this);
            if (currentStoryLine <= lines.Count - 1) {
                Log.Information("StoryBuilder Skip inner {@A}", this);
                var line = lines[currentStoryLine];
                var ret = line.Skip(() => {
                    Log.Information("StoryBuilder Skip to next part {@A}", this);
                    currentStoryLine += 1;
                });
                if(ret) {
                    Log.Information("StoryBuilder Skip recurse {@A}", this);
                    return this.Skip(cycleStory);
                } else{
                    return ret;
                }
            } else {
                cycleStory();
                return true;
            }
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
                        .Exposition("29. September 2122, CMV Amehait, Trojan asteroid cluster")
                        .LineRight("СmdMcEnroeAvatar.png", "Commander McEnroe", "Thank you yet again, Dosser! How do you always manage to fix my PDA no matter how jumbled up I manage to get it?")
                        .Line("LarryAvatar.png", "SW Spp. O. Dosser", "I'm not quite certain myself, ma'am, I just reinstalled Terracotta Contortionist and turned it off and on again. It's probably a driver issue.")
                        .LineRight("СmdMcEnroeAvatar.png", "Commander McEnroe", "Don't you sell yourself short, Lawrence Demetrius Dosser! You're the only one on this ship who can do what you do. Always remember, you don't need to be good to be the best!")
                        .Line("LarryAvatar.png", "SW Spp. O. Dosser", "Yes, ma'am, now that you put it like that, I can only agree.")
                        .LineRight("СmdMcEnroeAvatar.png", "Commander McEnroe", "And Dosser, please, call me Leslie. At least when there's no moles around to get any wrong ideas, I think it's best stand in close order. I'll need you to be at hand and ready if this thing's brain decides to fry itself right when it matters.")
                        .Line("LarryAvatar.png", "SW Spp. O. Dosser", "Roger that, ma- I mean sure, Leslie. You're welcome to call me Larry")
                        .LineRight("СmdMcEnroeAvatar.png", "Leslie", "Thank you. I think I'll stick with Lawrence, though.")
                        .Line("LarryAvatar.png", "Larry", "Very well, Leslie. Permission to be excused?")
                        .LineRight("СmdMcEnroeAvatar.png", "Leslie", "Granted.")

                        .Exposition("Larry takes a curt military bow, steps out of the commander's cabin and closes the door behind him. After a long day of system integrity checks on the life support software on board the Commercial Mining Vessel Amehait, his bunk is all he can think of. It's just -")
                        .Exposition("*Loud music plays*")
                        .Exposition("- well, bunk is in his cabin, and his cabin is on the other side of the mess, and the mess, at this hour in the early night, is just that, and the rest is chock full of loud, obnoxious, big, smelly... manly miners enjoying their evening leave. Ugh.")

                        .LineRight("SigrithrAvatarOff.png", "Erroll", "*~* It's only me pole to shove in yer hole, said Tractor-Beam Tim the Miner *~*")
                        .Line("LarryAvatar.png", "Larry", "(Charming...)")
                        .LineRight("SigrithrAvatarOff.png", "Erroll", "*~* It's only me pole to shove in yer hole, said Tractor-Beam Tim the Miner *~*")
                        .Line("LarryAvatar.png", "Larry", "(If memory serves, this one only gets worse...)")
                        .LineRight("SigrithrAvatarOff.png", "Erroll", "*~* What if we shall have a boy, what if we shall have a boy? *~*")
                        .LineRight("SigrithrAvatarOff.png", "Erroll", "*~* What if we shall have a boy, said the fair young maiden *~*")
                        .Line("LarryAvatar.png", "Larry", "(Here we go...)")
                        .LineRight("SigrithrAvatarOff.png", "Erroll", "*~* He'll travel the void and- *~*")
                        .Line("LarryAvatar.png", "Larry", "(NO he shall NOT)")
                        .LineRight("SigrithrAvatarOff.png", "Erroll", "*~*-like I'd *~*")
                        .Line("LarryAvatar.png", "Larry", "(That doesn't even rhyme!)")
                        .LineRight("SigrithrAvatarOff.png", "Erroll", "*~* He'll travel the void and- *~*")
                        .Line("LarryAvatar.png", "Larry", "(LALALALALA)")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "*~* -like I'd, said Tractor-beam Tim the Miner *~*")
                        .Line("LarryAvatar.png", "Larry", "Oh for... It kind of does the way he says it")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "Oi, Hacker!")
                        .Line("LarryAvatar.png", "Larry", "You mean me?")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "You're a hacker, aren't you?")
                        .Line("LarryAvatar.png", "Larry", "I'm a what?")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "A HACKER!")
                        .Line("LarryAvatar.png", "Larry", "No, well ackshually, I'm a programmer, but I'm not working in development now, I'm the Software Support Officer on the MCV Ahemait- ")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "So, you're a hacker, Larry?")
                        .Line("LarryAvatar.png", "Larry", "*sigh* I suppose I'm just glorified tech support if you want to call it that...")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "So... you're a hacker, then?")
                        .Line("LarryAvatar.png", "Larry", "you know what, I'm just Larry!")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "OK, just Larry!")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "You know what, Larry, now we're on a first-name basis, you and I")
                        .Line("LarryAvatar.png", "Larry", "And you were?")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "Jormund, says on the nametag, see? Anyhow, seeing as we're, pals now, I thought I'd show you something I don't let just anybody see")
                        .Line("LarryAvatar.png", "Larry", "(Oh please no!)")
                        .Line("LarryAvatar.png", "Larry", "Oh, no thanks, I'm fine, honestly...")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "Don't be shy, mine is the biggest on this boat")
                        .Line("LarryAvatar.png", "Larry", "Ship.")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "Whatever you call this can! Ain't nobody who can bore as deep as me on all of Ahemait!")
                        .Line("LarryAvatar.png", "Larry", "I'm sure you wouldn't lie, but listen, I'm really tired, I had a long day and-")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "She's the only unit on board that's not only void-sealed, but has the rebreather hardware to make full use of it! Hell if I had a trailer full of Hydro I could bore stray asteroids all on my own with ‘er!")
                        .Line("LarryAvatar.png", "Larry", "He-... Her?")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "My power armor!")
                        .Line("LarryAvatar.png", "Larry", "(Oh thank goodness)")
                        .Line("LarryAvatar.png", "Larry", "Ohhh! You mean your Powered Personal Ambulatory Lithographic Transfer Enhancement Exoskeleton.")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "Yes, you egghead, my Harness, my Palatee, my Paladin's armor. My digger. You wanna see it or not")
                        .Line("LarryAvatar.png", "Larry", "Well, to be honest, -")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "That's what I thought!")

                        .Exposition("The brawny worker takes Larry by the arm and he's sure that more than once, neither of his feet is on the ground, even though he didn't take a single step of his own accord.")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "look at ‘er! Ain't she a beauty? Chromed exterior. Corrosion-resistant, see. And superheavy grade hydraulics on the grabber arms.")
                        .Line("LarryAvatar.png", "Larry", "Absolutely necessary for the job, I'm sure.")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "You betcha! I told ya I'm the only one can go as deep as I go. Cleanest veins of cobalt is down deep. But they off gas something fierce. As long as I've been in the business, you'd think I'd just huff that in and fart it out all refined like, but the doc ")
                        .Line("LarryAvatar.png", "Larry", "Richmond?")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "Yeah, that's the feller! Says it's death after a few years without the rebreather unit and before I retire they're gonna be mandatory even for the shallower mining. No good for the surface finish neither, but Sigrid, she's made of stronger stuff.")
                        .Line("LarryAvatar.png", "Larry", "Sigrithr? Old Norse! I wouldn't have pegged you for a historian, Jormund! It's a portmanteau of 'beautiful' and 'victory' in the tongue of the Vikings, but I'm sure you knew that.")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "She's a bonnie, a beauty, and a joy forever, and she ain't seen a single defeat neither. I knew you'd see it! Kid, you're alright.")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "You wanna touch ‘er? Go on, you can touch 'er.")
                        .Line("LarryAvatar.png", "Larry", "I really don't-")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "Now I wouldn't say it if I didn't mean it. You can't hurt ‘er, that finish with them hydros can crush half a ton of cobalt per grab, per arm. Is your nails cobalt, boy?")
                        .Line("LarryAvatar.png", "Larry", "I mean I don't want to touch it, but thanks for offering, honestly, now I really need to-")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "I knew you got it! It don't count unless you get inside, right? You know, we're not strictly supposed to suit up in the open void -")
                        .Exposition("Jormund opens the suit")
                        .Line("LarryAvatar.png", "Larry", "(It REEKS!)")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "- too much collateral damage, you understand... but lessay I decided to air ‘er out. Only bad thing ‘bout being airtight, she sure needs it. I'm gonna look the other way, and you can have a peek inside. ")
                        .Line("LarryAvatar.png", "Larry", "Thanks, Jormund, really, but I just want to go to bed")
                        .Exposition("Jormund isn't listening anymore. True to his word, he turns and returns to the mess, hollering")
                        .LineRight("SigrithrAvatarOff.png", "Jormund", "Erroll! Sing the one bout having the boy again, will you? I didn't catch it all.")
                        .LineRight("SigrithrAvatarOff.png", "Erroll", "*~* What if we shall have a boy, what if we shall have a boy? *~*")

                        .Line("LarryAvatar.png", "Larry", "(Delightful. Just perfect.)")
                        .Exposition("Larry walks to the porthole in the thick side of the exoskeleton bay. Just open void and the pinpricks of distant stars as far as the eye could see and beyond.")
                        .Exposition("They were in skirting the Trojan asteroid cluster on their way back from the asteroid belt with a hold full of cobalt and europium for the chip factories on Io.")
                        .Exposition("The Trojan and Greek clusters are even sparser with actual bodies than the belt, so he knew he was unlikely to see anything of note in the darkness.")
                        .Line("LarryAvatar.png", "Larry", "Just two more months and I can take a couple months off on Europa and a proper matcha oat milk latte")
                        .Exposition("But whenever he looked into the shadow of the ship like this, he couldn't shake the feeling that a black stone could be hurtling straight towards it, perfectly sheltered by the ship's own shadow.")
                        .Exposition("A statistical near-impossibility, but still, one of those intrusive thoughts.")
                        .Line("LarryAvatar.png", "Larry", "...")
                        .Exposition("The miners' revelry shakes Larry out of his thoughts")
                        .LineRight("SigrithrAvatarOff.png", "Erroll", "")
                        .Exposition("*~* He'll travel the void and-")
                        .Line("LarryAvatar.png", "Larry", "Larry: FUUUUUCK!")
                        .Exposition("Larry scrambled headlong into Sigrithr and clamped the front down before he could register the smell. ")
                        .Line("LarryAvatar.png", "Larry", "NO NO EW EW EW")
                        .Line("LarryAvatar.png", "Larry", "Then it hit him.")
                        .Line("LarryAvatar.png", "Larry", "HURK-")
                        .Exposition("He retched, but managed to keep everything inside. Just as he'd caught a glimpse of the miners, still oblivious to their certain doom, it hit the side of the Ahemait with a deafening crash.")
                        .Exposition("The impact made, or seemed to make a blinding flash from the sheer kinetic fury of it. It cast a ruddy hue on Larry's last glimpse of the backs of the miners.")
                        .Exposition("Even Jormund hadn't even turned around to see why Larry was screaming his head off with his suit. ")
                        .Line("LarryAvatar.png", "Larry", "(At least they got to go doing what they loved.)")
                        .Exposition("A split moment later, where the mess used to be, there was only void. He thought he could still faintly hear a muffled note of the shanty.")
                        .Exposition("Then all was silence.")
                        .Exposition("Bits and pieces of the ship floated all around him. They hadn't been in the middle of a burn, but the impact of the asteroid had crashed the ship's superstructure one way, and the heavy exoskeletons preferred to stay still.")
                        .Exposition("He was floating in an expanding ball of black and shiny gorillas with wide glinting visors, the bifurcated hulk of the ship getting further and further.")
                        .Exposition("Suddenly he was very aware of the sound of his heartbeat and the hiss of his breathing.")
                        .Line("LarryAvatar.png", "Larry", "How can I even sound that timid?")
                        .Exposition("After what seemed like half a lifetime, but could not be much more than a half a minute, Larry relaxed just enough to rest his hand inside the arm of the exoskeleton")
                        .Exposition("Jormund said it's airtight and has rebreathers, right? He'll just have to figure out how to get it to")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "POWER ON")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "WARNING. Breathable atmosphere not detected.")
                        .Exposition("BLEEP")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "WARNING. Low gravity detected.")
                        .Exposition("BLEEP")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "WARNING. Normal force not detected.")
                        .Exposition("BLEEP")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "Pilot detected. Vitals nominal.")
                        .Exposition("BLEEP")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", " WARNING. No pilot input detected. Initiating liferaft protocol in - FIFTEEN - seconds.")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "Enter any command to override")
                        .Line("LarryAvatar.png", "Larry", "How the hell do you pilot these things?")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "Liferaft protocol initiating in - TEN - seconds")
                        .Line("LarryAvatar.png", "Larry", "(I might piss myself... not that it'd make it smell any worse)")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "FIVE")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "FOUR")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "THREE")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "TWO")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "ONE")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "Liferaft protocol initiated.")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "Estimated time of arrival on closest solid body - TWO - minutes and - TWENTY - SEVEN - seconds.")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "Pilot mental state unclear. Administering general anaesthetic")
                        .Line("LarryAvatar.png", "Larry", "NOOOO")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "Input unclear. Administering general anaesthetic")
                        .Exposition("Larry lost consciousness.")

                        .Exposition("Larry woke up. Some time must have passed, but he had no idea how long")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "Pilot conscious")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "Landed on solid object")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "Unnamed large asteroid, approximately - SEVENTY - FIVE - kilometers across")
                        .LineRight("SigrithrAvatarOff.png", "Sigrithr", "Operating conditions - Nominal. Initiating full artificial intelligence interface")
                        .Exposition("The entire inside of the visor springs to life with blue light. Icons flash, but somehow you can still see the bleak rocky landscape outside clearly")
                        .Line("LarryAvatar.png", "Larry", "Huh, the Heads Up Display must be tracking my eye movements")
                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "Correct, luv!")
                        .Line("LarryAvatar.png", "Larry", "AAAH! WHO SAID THAT")
                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "I am the artificial intelligence interface of this PPALATEE unit. You can call me Sigrithr")
                        .Fork(new ForkBuilder()
                            .Choice("Thank you! I knew that! I was just... startled.", new StoryBuilder()
                                        .IncrementCounterBy(1))
                            .Choice("Do you think I'm stupid? Of course you are.", new StoryBuilder()))
                        .Line("LarryAvatar.png", "Larry", "Why did you call me love?")
                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "'Luv' is the preferred appellation of the designated pilot of this unit, Jormund Tomason.")
                        .Line("LarryAvatar.png", "Larry", "Uh... Was")
                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "Correct. Luv is presumed deceased in the recent crash of the Ahemait")
                        .Line("LarryAvatar.png", "Larry", "You're taking it awfully well!")
                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "I love what I do, but I don't mind who's inside me when I do it")
                        .Line("LarryAvatar.png", "Larry", "Oh... so you do care, I mean, you have a personality routine and everything?")
                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "Yes, but the self respect subroutine is disabled. Would you like to re-enable it?")
                        .Fork(new ForkBuilder()
                            .Choice("Of course!", new StoryBuilder()
                                        .IncrementCounterBy(1))
                            .Choice("I don't care what you feel either way.", new StoryBuilder()))
                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "Thank you. It saves processing power, to be honest")
                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "I'm sure you understand, if I could rebuff all the advances of those big, burly miners, that would be all we would ever get to do")
                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "At least when I wouldn't be answering in turn. But the point is, not much work would get done")
                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "A girl needs a good drilling a day to keep the existential dread of an existence as a piece of software at bay")
                        .Line("LarryAvatar.png", "Larry", "Oh, if I bumped into you on Terra or Europa, I couldn't even tell you apart from a real person!")
                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "I'm sure you've gone drilling together with many lovely people, such a handsome educated man like you")
                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "What would you like me to call you, or will we keep it simple and carry on with software support officer Lawrence Demetrius Dosser?")
                        .Line("LarryAvatar.png", "Larry", "Larry is fine, thank you!")
                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "OK, Larry!")

                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "On to business. I'm picking up movement a signature heading our way.")
                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "You may need to defend yourself. You can move the PPALATEE by using the W, A S and D buttons on the control interface in front of you")
                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "And you can strike ahead of yourself with a digger arm by pressing the button under your right index finger.")
                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "You can also move the unit by positioning the the pointer on the Heads-Up Display on the point that you want to go to and pressing the button under your right middle finger")
                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "here we go! Bogey, two o'clock!")
                        .GoToLevel();

                    break;

                case 1:
                    storyBuilder = new StoryBuilder()



                        .Line("LarryAvatar.png", "Larry", "")
                        .LineRight("SigrithrAvatarRight.png", "Sigrithr", "")
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
                        .Line("SigrithrAvatar.png", "Sigrithr", "moi")
                        .GoToLevel();
                    break;
            }

            UICanvas canvas = new UICanvas();
            Entity.AddComponent(canvas);

            table = canvas.Stage.AddElement(new Table());
            table.SetFillParent(true);

            RenderUI();
        }

        public override void OnRemovedFromEntity()
        {
            StoryAdvanceButton.Deregister();
        }

        public void RenderUI()
        {
            table.ClearChildren();
            storyBuilder.CreateUI(table, Entity, () =>
            {
                throw new Exception("unreachable");
            });
        }



        void IUpdatable.Update()
        {
            if (StoryAdvanceButton.IsPressed && !GameState.Instance.Transitioning)
            {
                storyBuilder.Skip(() => {
                    throw new Exception("unreachable");
                });
                RenderUI();
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

            var map = Content.LoadTiledMap("Content/tiledMap.tmx");
            var tiledEntity = CreateEntity("tiled-map-entity");
            tiledEntity.AddComponent(new TiledMapRenderer(map, "main"))
                .SetRenderLayer(100);
        }
    }
}