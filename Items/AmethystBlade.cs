using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GemSwords.Items
{
	public class AmethystBlade : ModItem
	{
		private int swingCount;
		public override void SetDefaults()
		{
			Item.damage = 15;
			Item.DamageType = DamageClass.Melee;
			Item.width = 44;
			Item.height = 44;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.autoReuse = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 4;
			Item.value = Item.buyPrice(silver: 40);
			Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<Projectiles.FireSpark>();
			Item.shootSpeed = 8f;
			Item.scale = 1.1f;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.CopperBar, 15);
			recipe.AddIngredient(ItemID.Amethyst, 4);
			recipe.AddIngredient(ItemID.SiltBlock, 15);
			recipe.AddIngredient(ItemID.Torch, 5);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(player, target, hit, damageDone);
			target.AddBuff(BuffID.OnFire, 3 * 60);
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			swingCount++;
            if (swingCount >= 2)
            {
				Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
				SoundEngine.PlaySound(SoundID.Item20, player.position);
				swingCount=0;
			}
			return false;
        }
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
			if (Main.rand.NextBool(3))
			{
				Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Torch);
			}
        }
    }
}