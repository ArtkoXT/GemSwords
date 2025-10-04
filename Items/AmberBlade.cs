using GemSwords.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace GemSwords.Items
{
    public class AmberBlade : ModItem
    {
        private int killCounter = 0;

        public int KillCounter
        {
            get { return killCounter; }
            set { killCounter = value; }
        }
        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.DamageType = DamageClass.Melee;
            Item.width = 60;
            Item.height = 60;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.value = Item.buyPrice(silver: 40);
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item1;
            Item.scale = 0.9f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Scorpion, 1);
            recipe.AddIngredient(ItemID.FossilOre, 10);
            recipe.AddIngredient(ItemID.DesertFossil, 15);
            recipe.AddIngredient(ItemID.Amber, 6);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Increment killCounter only if it's less than 5 and the target is killed
            if (killCounter < 5 && target.life <= 0)
            {
                killCounter++;
                SoundEngine.PlaySound(SoundID.Item17, player.position);
                if (killCounter == 5)
                    SoundEngine.PlaySound(SoundID.Item78, player.position);
            }


        }

        public override bool AltFunctionUse(Player player)
        {
            // Enable alternate function only if killCounter is greater than 0
            if (killCounter > 0)
                return true;

            return false;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.noMelee = true; // Prevents the sword from doing damage when using the alternate function
                SummonScorpion(player, killCounter); // Summons the scorpions, 'player' is for the position and 'killCounter' is for the amount
                return true;
            }
            Item.noMelee = false;
            return true;
        }

        public void SummonScorpion(Player player, int amountToSummon)
        {
            for (int i = 0; i < amountToSummon; i++)
            {
                Vector2 spawnPosition = player.Center + new Vector2(Main.rand.Next(-50, 50), Main.rand.Next(-50, -10));
                Projectile.NewProjectile(player.GetSource_FromThis(), spawnPosition, new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 5)), ModContent.ProjectileType<AmberScorpion>(), 30, 0, player.whoAmI);

            }
            SoundEngine.PlaySound(SoundID.Item46, player.position);
            killCounter = 0;
        }
    }
}