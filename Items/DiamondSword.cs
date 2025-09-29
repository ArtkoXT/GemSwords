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
	public class DiamondSword : ModItem
	{
		private int hitCount;
		public override void SetDefaults()
		{
			Item.damage = 36;
			Item.DamageType = DamageClass.Melee;
			Item.width = 46;
			Item.height = 46;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(gold: 6);
			Item.rare = ItemRarityID.Cyan;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.noMelee = false;
			Item.shoot = ModContent.ProjectileType<Projectiles.DiaShards>();
			Item.shootSpeed = 8f;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.PlatinumBar, 15);
			recipe.AddIngredient(ItemID.Diamond, 4);
			recipe.AddIngredient(ItemID.ShadowScale, 5);
			recipe.AddTile(TileID.Hellforge);
			recipe.Register();
			recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.PlatinumBar, 15);
			recipe.AddIngredient(ItemID.Diamond, 4);
			recipe.AddIngredient(ItemID.TissueSample, 5);
			recipe.AddTile(TileID.Hellforge);
			recipe.Register();
		}
		public override bool AltFunctionUse(Player player)
		{
			if (hitCount <= 0) 
				return false;

			return true;
		}
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			hitCount++;
			if (hitCount <= 2)
				SoundEngine.PlaySound(SoundID.MaxMana, player.position);

			if (hitCount == 3)
                SoundEngine.PlaySound(SoundID.Item79, player.position);

			if (hitCount >= 3)
				hitCount = 3;

		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position, new Vector2(0, 0), ModContent.ProjectileType<DiamondSwordProj>(), 0, knockback, player.whoAmI, 0f, hitCount);
            if (player.altFunctionUse == 2 && hitCount >= 1)
			{
				Item.noMelee = true;
                Projectile.NewProjectile(source, position, new Vector2(player.direction * 7f, 0), ModContent.ProjectileType<DiamondSwordProj>(), 0, knockback, player.whoAmI, 0f, hitCount);
                SoundEngine.PlaySound(SoundID.Item28, player.position);
                for (; hitCount > 0; hitCount--)
				{
                    Projectile.NewProjectile(source, position, new Vector2(player.direction * 4f+hitCount, -7f+hitCount), type, damage, knockback, player.whoAmI);
                }
            }
			else Item.noMelee = false;
			return false;
        }
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
			if (Main.rand.NextBool(3))
			{
				int MeleeDust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.GemDiamond);
				Main.dust[MeleeDust].noGravity = true;
			}
		}
        public override void HoldItem(Player player)
		{
			player.AddBuff(BuffID.Shine, 2);
			Item.damage = 36 + hitCount * 2;
		}
		public class DiamondSwordProj : ModProjectile
		{
			private int Timer;

            public override void SetStaticDefaults()
			{
				Main.projFrames[Projectile.type] = 4;
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
				Projectile.height = 46;
				Projectile.aiStyle = -1;
				Projectile.timeLeft = 25;
				Projectile.tileCollide = false;
			}
			public float RotateValue { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }
			public override void AI()
			{
				int hitCount = (int)Projectile.ai[1];
                Player player = Main.player[Projectile.owner];
				player.heldProj = Projectile.whoAmI;
				if (++Timer >= 25 / player.GetAttackSpeed(DamageClass.Melee))
					Projectile.Kill();
				Projectile.frame = hitCount;
				float rotSpeed = 0.18f * player.GetAttackSpeed(DamageClass.Melee);
				int rotMax = 40;

				Projectile.spriteDirection = player.direction;
				RotateValue = Projectile.rotation + MathHelper.ToRadians(rotMax) * Projectile.spriteDirection;

				Projectile.rotation = MathHelper.Lerp(Projectile.rotation, RotateValue, rotSpeed);

				var cache = Projectile.Center;
				Projectile.width = (int)(46 * Projectile.scale);
				Projectile.height = (int)(46 * Projectile.scale);
				Projectile.Center = cache;

				Projectile.Center = player.Center + Projectile.velocity * 15f;
				Projectile.velocity = Projectile.rotation.ToRotationVector2() * 0.1f;
				player.itemRotation = Projectile.rotation;
			}
			public override bool PreDraw(ref Color lightColor)
			{
				Player player = Main.player[Projectile.owner];

				Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

				int frameHeight = texture.Height / Main.projFrames[Projectile.type];
				int startY = frameHeight * Projectile.frame;

				Rectangle sourceRectangle = new (0, startY, texture.Width, frameHeight);

				SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

				float rotation = Projectile.velocity.ToRotation() + (MathHelper.ToRadians(270) * Projectile.spriteDirection);

				Vector2 position = player.Center + Projectile.velocity - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

				Vector2 origin = Projectile.spriteDirection == 1 ? new Vector2(-4, (texture.Height / Main.projFrames[Projectile.type])+7) : new Vector2(texture.Width+4, (texture.Height / Main.projFrames[Projectile.type])+7);

				Color color = Projectile.GetAlpha(lightColor);
				Main.EntitySpriteDraw(texture, position, sourceRectangle, color, rotation, origin, Projectile.scale, spriteEffects, 0);
				return false;
			}
		}

	}
}