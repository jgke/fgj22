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
    class MenuComponent : Component {
        public override void OnAddedToEntity() {
            UICanvas canvas = new UICanvas();
            Entity.AddComponent(canvas);

            var table = canvas.Stage.AddElement( new Table() );
            table.SetFillParent( true );

            var button1 = new TextButton("Start game", TextButtonStyle.Create( Color.Black, Color.DarkGray, Color.Green ) );
            table.Add( button1 ).SetMinWidth( 100 ).SetMinHeight( 30 );
            table.Row();
            button1.OnClicked += _ => {
                Core.StartSceneTransition( new WindTransition( () => new GameplayScene() ) );
            };
            var button2 = new TextButton("Quit game", TextButtonStyle.Create( Color.Black, Color.DarkGray, Color.Green ) );
            table.Add( button2 ).SetMinWidth( 100 ).SetMinHeight( 30 );
            table.Row();
            button2.OnClicked += _ => {
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