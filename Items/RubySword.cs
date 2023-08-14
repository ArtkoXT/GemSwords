using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace GemSwords.Items
{
	public class RubySword : ModItem
	{
		public int I;
		public override void SetDefaults()
		{
			Item.damage = 30;
			Item.DamageType = DamageClass.Melee;
			Item.scale = 1.25f;
			Item.width = 46;
			Item.height = 50;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<RubySwordProj>();
			Item.shootSpeed = 7f;
			Item.noMelee = false;

			Item.value = Item.buyPrice(gold: 4);
			Item.rare = ItemRarityID.Yellow;

		}
        public override bool AltFunctionUse(Player player)
        {
			if( I <= 0) { return false; }
			else { return true; }
        }
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.GoldBar, 15);
			recipe.AddIngredient(ItemID.Ruby, 4);
			recipe.AddIngredient(ItemID.Sandstone, 15);
			recipe.AddIngredient(ItemID.FossilOre, 4);
			recipe.AddTile(TileID.Hellforge);
			recipe.Register();
		}
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			I++;
			if( I == 1 || I == 2)
            {
				SoundEngine.PlaySound(SoundID.Item79, player.position);
			}
			if ( I >= 2) { I = 2; }
		}
        public override bool CanUseItem(Player player)
        {
			if(player.altFunctionUse == 2)
            {
				Item.noMelee = true;
				Item.UseSound = SoundID.Item71;
				if (I >= 1)
                {
					player.AddBuff(BuffID.RapidHealing, 60);
                }
            }
			else
            {
				Item.noMelee = false;
				Item.UseSound = SoundID.Item1;
			}
            return true;
        }
        public override void HoldItem(Player player)
        {
            Item.damage = 30 + I * 5;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			int dmg = (int)(damage * 1.6f);
			Projectile.NewProjectile(source, position, new Vector2(player.direction * 7f, 0), ModContent.ProjectileType<RubySwordProj>(), 0, knockback, player.whoAmI);
			if (player.altFunctionUse == 2)
			{
				if (I >= 1)
				{
					Projectile.NewProjectile(source, position, new Vector2(player.direction * 7f, 0), ModContent.ProjectileType<RubySwordProj>(), 0, knockback, player.whoAmI);
					Projectile.NewProjectile(source, position, velocity*2, ModContent.ProjectileType<Projectiles.RubyWave>(), dmg, knockback, player.whoAmI);
					I--;
				}
            }
			return false;
        }
		public class RubySwordProj : ModProjectile
        {
			public int Timer;
			public override void SetStaticDefaults()
            {
				Main.projFrames[Projectile.type] = 3;
			}
            public override void SetDefaults()
			{
				Projectile.hostile = false;
				Projectile.friendly = true;
				Projectile.penetrate = -1;
				Projectile.usesLocalNPCImmunity = true;
				Projectile.localNPCHitCooldown = -1;
				Projectile.DamageType = DamageClass.Melee;
				Projectile.width = 46;
				Projectile.height = 50;
				Projectile.aiStyle = -1;
				Projectile.timeLeft = 25;
				Projectile.tileCollide = false;
				Projectile.scale = 1.25f;
			}
			public float RotateValue { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }
			public override void AI()
			{
				Player player = Main.player[Projectile.owner];
				player.heldProj = Projectile.whoAmI;
				if (Main.myPlayer == Projectile.owner)
					Projectile.netUpdate = true;
				if (++Timer >= 25 / player.GetAttackSpeed(DamageClass.Melee))
					Projectile.Kill();
				if (Main.LocalPlayer.HeldItem.ModItem is RubySword rubySword)
					Projectile.frame = rubySword.I;
				float rotSpeed = 0.2f * player.GetAttackSpeed(DamageClass.Melee);
				int rotMax = 35;

				Projectile.spriteDirection = player.direction;
				RotateValue = Projectile.rotation + MathHelper.ToRadians(rotMax) * Projectile.spriteDirection;

				Projectile.rotation = MathHelper.Lerp(Projectile.rotation, RotateValue, rotSpeed);
				
				var cache = Projectile.Center;
				Projectile.width = (int)(46 * Projectile.scale);
				Projectile.height = (int)(50 * Projectile.scale);
				Projectile.Center = cache;

				Projectile.Center = player.Center + Projectile.velocity * 15f;
				Projectile.velocity = Projectile.rotation.ToRotationVector2() * 0.1f;
				player.itemRotation = Projectile.rotation * Projectile.spriteDirection;
			}
            public override bool PreDraw(ref Color lightColor)
            {
				Player player = Main.player[Projectile.owner];

				Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

				int frameHeight = texture.Height / Main.projFrames[Projectile.type];
				int startY = frameHeight * Projectile.frame;

				Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

				SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

				float rotation = Projectile.velocity.ToRotation() + (MathHelper.ToRadians(270) * Projectile.spriteDirection);

				Vector2 position = player.Center + Projectile.velocity - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

				Vector2 origin = Projectile.spriteDirection == 1 ? new Vector2(-5, texture.Height-95) : new Vector2(texture.Width+5, texture.Height-95);

				Color color = Projectile.GetAlpha(lightColor);
				Main.EntitySpriteDraw(texture, position, sourceRectangle, color, rotation, origin, Projectile.scale, spriteEffects, 0);
				return false;
            }
        }

	}
}