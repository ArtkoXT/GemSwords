using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace GemSwords.Items
{
    public class AmethystJewel : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 44;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 1);
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.LargeAmethyst, 1)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.SoulofLight, 5)
                .AddIngredient(ItemID.LivingFireBlock, 15)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
