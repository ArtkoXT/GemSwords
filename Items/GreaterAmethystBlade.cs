using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace GemSwords.Items
{
	public class GreaterAmethystBlade : ModItem
	{
		private int I;
		public int Fuel = 100;
		public int FuelTick;
		private bool FlameMode;
		public static int UseTime = 27;
		public override void SetDefaults()
		{
			Item.damage = 41;
			Item.DamageType = DamageClass.Melee;
			Item.width = 48;
			Item.height = 48;
			Item.useTime = 27;
			Item.useAnimation = 27;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.knockBack = 4;
			Item.value = Item.buyPrice(silver: 240);
			Item.rare = ItemRarityID.Purple;
			Item.UseSound = SoundID.Item1;
			Item.noUseGraphic = true;
			Item.shoot = Item.shoot = ModContent.ProjectileType<GreaterAmethystBladeProj>();
			Item.shootSpeed = 8f;
			Item.autoReuse = true;
		}

		public void setDamage(int dmg)
		{
			Item.damage = dmg;
        }

        public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<AmethystBlade>()
				.AddIngredient<AmethystJewel>()
				.AddTile(TileID.Anvils)
				.Register();
		}
		public override bool AltFunctionUse(Player player)
		{
			// Enables Right Click function if Fuel is 100 or more and disables it if not
			if (Fuel >= 100)
				return true;

			return false;
		}
        public override void HoldItem(Player player)
        {
			// Activates FlameMode by right clicking when Fuel is 100 or more
			if (player.altFunctionUse == 2 && Fuel >= 100)
			{
				FlameMode = true;
				SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, player.position);
			}
			// Deactivates FlameMode when Fuel reaches 0
			if (Fuel <= 0) 
				FlameMode = false;

			if (FlameMode == true)
			{
				setDamage(55); // Increases damage while FlameMode is active
                UseTime = 14; // Increases use speed while FlameMode is active
				if (FuelTick++ >= 5) // Decreases Fuel by 1 every 5 ticks while FlameMode is active
				{
					Fuel--;
					FuelTick = 0;
				}
			}
            else
            {
				setDamage(41); // Reverts Damage to default
                UseTime = 27; // Reverts Use Speed to default
				if (FuelTick++ >= 5) // Increases Fuel by 1 every 5 ticks if FlameMode is not active
				{
					Fuel++;
					FuelTick = 0;
				}
			}
			if (Fuel >= 100) // Caps Fuel to 100
				Fuel = 100;

			Item.useTime = UseTime;
			Item.useAnimation = UseTime;
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			int flameModeValue = FlameMode == true ? 1 : 0; // Converts FlameMode bool to int for projectile spawn
            Projectile.NewProjectile(source, position, new Vector2(0, 0), ModContent.ProjectileType<GreaterAmethystBladeProj>(), damage, knockback, player.whoAmI, 0f, flameModeValue, I); // Spawns Sword Projectile
			if (player.altFunctionUse == 0 && ++I >= 2) // Spawns a Ball of Fire every 2nd swing
            {
				Projectile.NewProjectile(source, position, velocity, ProjectileID.BallofFire, damage, knockback, player.whoAmI);
				SoundEngine.PlaySound(SoundID.Item20, player.position);
				I=0;
			}
			// Fiery swing sound when flame mode is on
			if (FlameMode == true)
				SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, player.position);
			return false;
        }
		public class GreaterAmethystBladeProj : ModProjectile
		{
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
				Projectile.width = 48;
				Projectile.height = 48;
				Projectile.aiStyle = -1;
				Projectile.timeLeft = 27;
				Projectile.tileCollide = false;
			}
			public int Timer
			{
				get => (int)Projectile.ai[0];
				set => Projectile.ai[0] = value;
			}
			public bool FieryMode
			{
				get => Projectile.ai[1] == 1;
				set => Projectile.ai[1] = value ? 1 : 0;
            }
			public override void AI()
			{
                Player player = Main.player[Projectile.owner];
				if (Main.myPlayer == Projectile.owner)
					Projectile.netUpdate = true;
				Timer += 1;
				// Kill the projectile if it reaches it's intented lifetime
				if (Timer >= UseTime / player.GetAttackSpeed(DamageClass.Melee))
				{
					Projectile.Kill();
					return;
				}
				player.heldProj = Projectile.whoAmI;
				// Changes Sword to a fiery version when flame mode is enabled and reverts it back to original when it's off
				Projectile.frame = (int)Projectile.ai[1];
                Projectile.scale = FieryMode == true ? 1.7f : 1.5f;

                Projectile.spriteDirection = player.direction;
				Projectile.Center = Projectile.Center;

				// Sets the angle for the projectile to stop rotating at
				float maxRot = (int)Projectile.ai[2] >= 1 ? MathHelper.ToRadians(-180) * Projectile.spriteDirection : MathHelper.ToRadians(220) * Projectile.spriteDirection;
				// The speed at which the projectile rotates at
				float speedRot = 0.15f * player.GetAttackSpeed(DamageClass.Melee);
				// Rotates the projectile based on the max rotation angle and projectile's current rotation
				Projectile.rotation = player.altFunctionUse == 0 ? MathHelper.Lerp(Projectile.rotation, maxRot, speedRot) : Projectile.rotation;

				// Makes player's arm follow the projectile rotation
				float _itemRotation = (int)Projectile.ai[2] >= 1 ? Projectile.rotation + (float)Math.PI / 3 * Projectile.spriteDirection : Projectile.rotation - (float)Math.PI * Projectile.spriteDirection;
				player.itemRotation = _itemRotation;
			}
			public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
			{
				float rotationfactorMult = (int)Projectile.ai[2] >= 1 ? (float)Math.PI / -2f * Projectile.spriteDirection : 0;
				float rotationFactor = Projectile.rotation + rotationfactorMult; // The rotation of the Jousting Lance.
				float scaleFactor = FieryMode == true ? -85f * Projectile.scale * Projectile.spriteDirection : -75f * Projectile.scale * Projectile.spriteDirection; // How far back the hit-line will be from the tip of the Jousting Lance. You will need to modify this if you have a longer or shorter Jousting Lance. Vanilla uses 95f
				float widthMultiplier = 23f; // How thick the hit-line is. Increase or decrease this value if your Jousting Lance is thicker or thinner. Vanilla uses 23f
				float collisionPoint = 0f; // collisionPoint is needed for CheckAABBvLineCollision(), but it isn't used for our collision here. Keep it at 0f.

				// This Rectangle is the width and height of the Jousting Lance's hitbox which is used for the first step of collision.
				// You will need to modify the last two numbers if you have a bigger or smaller Jousting Lance.
				// Vanilla uses (0, 0, 300, 300) which that is quite large for the size of the Jousting Lance.
				// The size doesn't matter too much because this rectangle is only a basic check for the collision (the hit-line is much more important).
				Rectangle lanceHitboxBounds = new Rectangle(0, 0, 300, 300);

				// Set the position of the large rectangle.
				lanceHitboxBounds.X = (int)Projectile.position.X - lanceHitboxBounds.Width / 2;
				lanceHitboxBounds.Y = (int)Projectile.position.Y - lanceHitboxBounds.Height / 2;

				// This is the back of the hit-line with Projectile.Center being the tip of the Jousting Lance.
				Vector2 hitLineEnd = Projectile.Center + rotationFactor.ToRotationVector2() * scaleFactor;

				// The following is for debugging the size of the hit line. This will allow you to easily see where it starts and ends.
				 //Dust.NewDustPerfect(Projectile.Center, DustID.Pixie, Velocity: Vector2.Zero, Scale: 0.5f);
				 Dust.NewDustPerfect(hitLineEnd, DustID.Pixie, Velocity: Vector2.Zero, Scale: 0.5f);

				// First check that our large rectangle intersects with the target hitbox.
				// Then we check to see if a line from the tip of the Jousting Lance to the "end" of the lance intersects with the target hitbox.
				if (lanceHitboxBounds.Intersects(targetHitbox)
					&& Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, hitLineEnd, widthMultiplier * Projectile.scale, ref collisionPoint))
				{
					return true;
				}
				return false;
			}
            public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
			{
				int Debuff = FieryMode == true ? BuffID.OnFire3 : BuffID.OnFire;
				target.AddBuff(Debuff, 3 * 60); // Sets the hit enemy on fire for 3 seconds
				// Adds 1 to fuel everytime you hit an npc
				if (Main.LocalPlayer.HeldItem.ModItem is GreaterAmethystBlade greaterAmethystBlade)
					greaterAmethystBlade.Fuel += 1;
			}
			public override bool PreDraw(ref Color lightColor)
			{
				Player player = Main.player[Projectile.owner]; // Defines player
				if (FieryMode == true)
				{
					int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 50, default, 0.9f);
				}
				SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None; // Draws the projectile flipped horizontally if facing the other direction
				Texture2D texture = TextureAssets.Projectile[Type].Value;

				int frameHeight = texture.Height / Main.projFrames[Projectile.type]; // Defines 1 frame height
				int startY = frameHeight * Projectile.frame; // Sets the texture drawing point to the correct frame height
				int offsetX = 4;
				int offsetY = 7;
				int rotateDirection = (int)Projectile.ai[2];
				float rotate = rotateDirection >= 1 ? MathHelper.ToRadians(-260) : MathHelper.ToRadians(260);
				Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight); // Rectangle to draw the texture in
				Vector2 origin = Projectile.spriteDirection == 1 ? new Vector2(-offsetX, frameHeight + offsetY) : new Vector2(texture.Width + offsetX, frameHeight + offsetY); // Sets the origin point to draw the projectile from
				float rotation = player.altFunctionUse == 0 ? Projectile.rotation + rotate * Projectile.spriteDirection : Projectile.rotation + MathHelper.ToRadians(-20) * Projectile.spriteDirection; // Sets Projectile rotation

				Vector2 position = player.Center + Projectile.velocity - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY); // Position where to draw the Projectile

				Color drawColor = Projectile.GetAlpha(lightColor);
				Main.EntitySpriteDraw(texture, position, sourceRectangle, drawColor, rotation, origin, Projectile.scale, spriteEffects, 0); // Draws the Projectile
				return false;
			}
		}
	}
}