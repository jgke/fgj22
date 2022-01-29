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

namespace Fgj22.App
{
    class StoryPiece {
         public string Character;
         public string Text;

         public StoryPiece(string character, string text) {
             this.Character = character;
             this.Text = text;
         }
    }

    class StoryBuilder {
        public List<StoryPiece> lines;
        public StoryBuilder() {
            lines = new List<StoryPiece>();
        }
        
        public StoryBuilder Line(string by, string what) {
            lines.Add(new StoryPiece(by, what));
            return this;
        }

        public Story Build() {
            return new Story(lines);
        }
    }

    class Story {
        public List<StoryPiece> Content;

        public Story(List<StoryPiece> content) {
            this.Content = content;
        }
    }

    class StoryComponent : Component {
        int SceneNumber;
        int storyLine = 0;
        Story story;
        Table table;

        public StoryComponent(int sceneNumber) {
            this.SceneNumber = sceneNumber;
        }

        public override void OnAddedToEntity() {
            switch(SceneNumber) {
                case 0:
                    story = new StoryBuilder()
                        .Line("Content/SigrithrAvatar.png", "moi") 
                        .Line("Content/VonNeumannAvatar.png", "no moi") 
                        .Build();
                    break;

                default:
                    story = new StoryBuilder().Build();
                    break;
            }

            UICanvas canvas = new UICanvas();
            Entity.AddComponent(canvas);

            table = canvas.Stage.AddElement( new Table() );
            table.SetFillParent( true );

            CycleStory();
        }

        public void CycleStory() {
            Log.Information("Cycle {A} {B}", storyLine, story.Content);
            if(storyLine >= story.Content.Count) {
                try {
                    Core.StartSceneTransition( new WindTransition( () => new GameplayScene() ) );
                } catch (Exception e) {
                    Log.Information("{e}", e);
                }
            } else {
                var line = story.Content[storyLine];
                table.ClearChildren();
                table.Bottom();
                var img = new Image(Entity.Scene.Content.LoadTexture(line.Character));
                table.Add( img ).Bottom().Width( 100 ).Height( 100 );
                var button1 = new TextButton(line.Text, TextButtonStyle.Create( Color.Black, Color.DarkGray, Color.Green ) );
                table.Add( button1 ).SetMinHeight( 100 ).Expand().Bottom().SetFillX();
                table.Row();
                button1.OnClicked += _ => {
                    storyLine += 1;
                    CycleStory();
                };
            }
        }
    }

    public class StoryScene : ProgramScene
    {
        int SceneNumber;
        public StoryScene(int sceneNumber) {
            SceneNumber = sceneNumber;
        }
        public override void Initialize()
        {
            base.Initialize();

            var menuEntity = CreateEntity("menu", new Vector2(0, 0));
            menuEntity.AddComponent(new StoryComponent(SceneNumber));
        }
    }
}