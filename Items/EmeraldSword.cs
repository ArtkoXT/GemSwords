using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GemSwords.Items
{
	public class EmeraldSword : ModItem
	{
		private static int Counter; // Using Counter to control
		public override void SetDefaults()
		{
			Item.damage = 22;
			Item.DamageType = DamageClass.Melee;
			Item.width = 48;
			Item.height = 48;
			Item.useTime = 19;
			Item.useAnimation = 19;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = false;
			Item.knockBack = 4;
			Item.value = Item.buyPrice(gold: 1);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.TungstenBar, 15);
			recipe.AddIngredient(ItemID.Emerald, 4);
			recipe.AddIngredient(ItemID.JungleSpores, 4);
			recipe.AddIngredient(ItemID.Vine, 1);
			recipe.AddIngredient(ItemID.Moonglow, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
		public override bool AltFunctionUse(Player player)
		{
			if (Counter > 0) // Makes it so you can only right click use if the counter is above 0
				return true;
			else return false;
		}
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Increases Counter by 1 for each hit on npcs and when the Counter is = 1 spawns a Tracker on the target
			Counter++;
			if (Counter == 1)
				 Projectile.NewProjectile(player.GetSource_FromThis(), target.Center, new Vector2(0f, 0f), ModContent.ProjectileType<Tracker>(), 0, 0, player.whoAmI);
			if (target.life <= 0) // Sets the Counter to 0 when the enemy dies
				Counter = 0;
		}
		public override bool CanUseItem(Player player)
		{
			// Makes it so right click does not do melee damage and sets Counter to 0 on right click
			if (player.altFunctionUse == 2)
			{
				Item.noMelee = true;
				Counter = 0;
			}
			else Item.noMelee = false;
			return true;
		}
		public override void HoldItem(Player player)
		{
			player.AddBuff(BuffID.DryadsWard, 1);
		}
		public class Tracker : ModProjectile
		{
			private int AliveTime;
			public override void SetDefaults()
			{
				Projectile.hostile = false;
				Projectile.friendly = true;
				Projectile.penetrate = -1;
				Projectile.DamageType = DamageClass.Melee;
				Projectile.width = 20;
				Projectile.height = 20;
				Projectile.aiStyle = -1;
				Projectile.timeLeft = 200;
				Projectile.tileCollide = false;
				Projectile.scale = 0.75f;
			}
			public override void AI()
			{

				Projectile.ai[0] += 1f;

				FadeInAndOut();

				float maxDetectRadius = 50f; // The maximum radius at which a projectile can detect a target
				NPC closestNPC = FindClosestNPC(maxDetectRadius);
				if (closestNPC == null) // If there's no closestNPC kill the Tracker
				{
					return;
				}
				Projectile.Center = closestNPC.Center; // Makes the Tracker stay on the enemy
				
			}
			public override void PostAI()
			{
				Player player = Main.player[Projectile.owner];
				if (player.altFunctionUse == 2 || ++AliveTime >=200 ) // On right click spawn thorn burst
				{
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(15f, 0f), ProjectileID.VilethornBase, 20, 2f, player.whoAmI);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0f, -15f), ProjectileID.VilethornBase, 20, 2f, player.whoAmI);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(12f, -10f), ProjectileID.VilethornBase, 20, 2f, player.whoAmI);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(-12f, -10f), ProjectileID.VilethornBase, 20, 2f, player.whoAmI);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(-15f, 0f), ProjectileID.VilethornBase, 20, 2f, player.whoAmI);
					Counter = 0;
				}
				if (Counter == 0) // If Counter is 0 then remove the tracker
					Projectile.Kill();
			}
			public void FadeInAndOut()
			{
				// If lasts less than 50 ticks — fade in, if more — fade out
				if (Projectile.ai[0] <= 50f)
				{
					// Fade in
					Projectile.alpha -= 15;
					// Cap alpha before timer reaches 50 ticks
					if (Projectile.alpha < 0)
						Projectile.alpha = 0;
					return;
				}

				// Fade out
				Projectile.alpha += 15;
				// Cap alpha to the maximum 255(completely transparent)
				if (Projectile.alpha > 255)
					Projectile.alpha = 0;
			}
			public NPC FindClosestNPC(float maxDetectDistance)
			{
				NPC closestNPC = null;

				// Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
				float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

				// Loop through all NPCs(max always 200)
				for (int k = 0; k < Main.maxNPCs; k++)
				{
					NPC target = Main.npc[k];
					// Check if NPC able to be targeted. It means that NPC is
					// 1. active (alive)
					// 2. chaseable (e.g. not a cultist archer)
					// 3. max life bigger than 5 (e.g. not a critter)
					// 4. can take damage (e.g. moonlord core after all it's parts are downed)
					// 5. hostile (!friendly)
					// 6. not immortal (e.g. not a target dummy)
					if (target.active)
					{
						// The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
						float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

						// Check if it is within the radius
						if (sqrDistanceToTarget < sqrMaxDetectDistance)
						{
							sqrMaxDetectDistance = sqrDistanceToTarget;
							closestNPC = target;
						}
					}
				}

				return closestNPC;
			}
		}
	}
}