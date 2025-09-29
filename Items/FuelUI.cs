using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace GemSwords.Items
{
	internal class FuelUI : UIState
	{
		private UIText text;
		private UIElement area;
		public override void OnInitialize()
		{
			area = new UIElement();
            area.Width.Set(182, 0f);
            area.Height.Set(60, 0f);


            text = new UIText("0/0", 0.8f); // text to show stat


			area.Append(text);
			Append(area);
		}
		public override void Draw(SpriteBatch spriteBatch)
		{
			// This prevents drawing unless we are using a Greater Amethyst Blade
			if (Main.LocalPlayer.HeldItem.ModItem is not GreaterAmethystBlade)
				return;

			base.Draw(spriteBatch);
		}
		public override void Update(GameTime gameTime)
		{
			if (Main.LocalPlayer.HeldItem.ModItem is not GreaterAmethystBlade)
				return;
			// Setting the text per tick to update and show our resource values.
			if (Main.LocalPlayer.HeldItem.ModItem is GreaterAmethystBlade greaterAmethystBlade)
			{
				text.SetText($"Fuel: {greaterAmethystBlade.Fuel} / 100");
				//if (greaterAmethystBlade.Fuel >= 100)
				//	text.SetText("Full!");
			}

            float uiWidth = area.Width.Pixels;
            float uiHeight = area.Height.Pixels; ;  // Use the fixed height you set above

            area.Left.Set((Main.screenWidth - uiWidth / 2f) / 2f, 0f);
            area.Top.Set((Main.screenHeight - uiHeight) / 2f - 30f, 0f); // -30f is the vertical offset


            area.Recalculate();

            base.Update(gameTime);
        }
	}
	class FuelUISystem : ModSystem
	{
		private UserInterface FuelUserInterface;

		internal FuelUI FuelUI;

		public override void Load()
		{
			// All code below runs only if we're not loading on a server
			if (!Main.dedServ)
			{
				FuelUI = new();
				FuelUserInterface = new();
				FuelUserInterface.SetState(FuelUI);
			}
		}

		public override void UpdateUI(GameTime gameTime)
		{
			FuelUserInterface?.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
			if (resourceBarIndex != -1)
			{
				layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
					"GemSwords: Amethyst Fuel",
					delegate {
						FuelUserInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}
