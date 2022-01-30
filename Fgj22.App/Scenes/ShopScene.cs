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
using System.Collections.Generic;
using System.Linq;

namespace Fgj22.App
{
    class Upgrade
    {
        public string Id;
        public Action Action;
        public string Text;
        public string[] Dependencies;

        public Upgrade(string id, string text, Action action, string[] dependencies = null)
        {
            this.Id = id;
            this.Text = text;
            this.Action = action;
            this.Dependencies = dependencies;
        }
    }

    class ShopComponent : Component
    {
        static List<Upgrade> UpgradeList = new List<Upgrade>() {
            new Upgrade("doublespeed", "Double movement speed", () => { GameState.Instance.PlayerSpeed *= 2;} ),
            new Upgrade("", "Unlock 'HEAL'", () => {}, new string[]{"doublespeed"}),
            new Upgrade("", "Increase melee attack damage", () => {}, new string[]{"doublespeed"}),
        };

        public override void OnAddedToEntity()
        {
            UICanvas canvas = new UICanvas();
            Entity.AddComponent(canvas);

            var table = canvas.Stage.AddElement(new Table());
            table.SetFillParent(true);

            var availableUpgrades = UpgradeList
            .Where(upgrade => upgrade.Dependencies == null || upgrade.Dependencies.All(dependency => GameState.Instance.Upgrades.Contains(dependency)))
            .Where(upgrade => !GameState.Instance.Upgrades.Contains(upgrade.Id));

            foreach (var upgrade in availableUpgrades)
            {
                var upgradeButton = UiComponents.WrappingTextButton(upgrade.Text, () => {
                    upgrade.Action();
                    GameState.Instance.LevelNum += 1;
                    GameState.Instance.Upgrades.Add(upgrade.Id);
                    GameState.Instance.DoTransition(() => new StoryScene());
                });
                table.Add(upgradeButton).SetMinWidth(100).SetMinHeight(30).SetSpaceBottom(5);
                table.Row();
            }

            var contButton = UiComponents.WrappingTextButton("Skip upgrade, continue game", () => {
                GameState.Instance.LevelNum += 1;
                GameState.Instance.DoTransition(() => new StoryScene());
            });
            table.Add(contButton).SetMinWidth(100).SetMinHeight(30);
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