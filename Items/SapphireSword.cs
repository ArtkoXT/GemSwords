using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace GemSwords.Items
{
	public class SapphireSword : ModItem
	{
		private int hitCount;
		public override void SetDefaults()
		{
			Item.damage = 24;
			Item.DamageType = DamageClass.Melee;
			Item.width = 48;
			Item.height = 48;
			Item.useTime = 23;
			Item.useAnimation = 23;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(silver: 80);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.SilverBar, 15);
			recipe.AddIngredient(ItemID.Sapphire, 4);
			recipe.AddIngredient(ItemID.SnowBlock, 15);
			recipe.AddIngredient(ItemID.IceBlock, 10);
			recipe.AddIngredient(ItemID.Shiverthorn, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			hitCount++;
			if (hitCount >= 3)
            {
				Projectile.NewProjectile(player.GetSource_FromThis(), target.Center.X, target.Center.Y-200, target.velocity.X/2, 10f, ModContent.ProjectileType<Projectiles.SapphireProj>(), 40, 1f, player.whoAmI);
				SoundEngine.PlaySound(SoundID.Item101, new Vector2 (target.Center.X, target.Center.Y-200));
				hitCount = 0;
			}
			target.AddBuff(BuffID.Frostburn, 60);
		}
	}
}