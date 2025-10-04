using GemSwords.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using static System.Net.Mime.MediaTypeNames;

namespace GemSwords.UI
{
    internal class AmberUI : UIState
    {
        private UIElement area;
        private readonly UICycleImage[] imageArr = new UICycleImage[5];

        public override void OnInitialize()
        {
            area = new UIElement();
            area.Left.Set(-area.Width.Pixels - 600, 1f);
            area.Top.Set(30, 0f);
            area.Width.Set(182, 0f);
            area.Height.Set(60, 0f);

            for (int i = 0; i < 5; i++)
            {
                imageArr[i] = new UICycleImage(ModContent.Request<Texture2D>("GemSwords/UI/AmberUI"), 2, 26, 26, 0, 0);
                imageArr[i].Left.Set(22 + i * 26, 0f);
                imageArr[i].Top.Set(0, 0f);

                area.Append(imageArr[i]);
            }

            Append(area);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.HeldItem.ModItem is not AmberBlade)
                return;

            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.HeldItem.ModItem is not AmberBlade amberBlade)
                return;
            
            for (int j = 0; j < amberBlade.KillCounter; j++)
                imageArr[j].CurrentState = 1;

            if (amberBlade.KillCounter == 0)
            {
                foreach (UICycleImage img in imageArr)
                    img.CurrentState = 0;
            }
                

            base.Update(gameTime);
        }
    }
    [Autoload(Side = ModSide.Client)]
    internal class AmberUISystem : ModSystem
    {
        private UserInterface amberUserInterface;

        internal AmberUI amberUI;

        public override void Load()
        {
            amberUI = new();
            amberUserInterface = new();
            amberUserInterface.SetState(amberUI);

        }

        public override void UpdateUI(GameTime gameTime)
        {
            amberUserInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "GemSwords: Amber UI",
                    delegate
                    {
                        amberUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
