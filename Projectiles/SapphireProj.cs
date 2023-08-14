using Terraria;
using Terraria.ModLoader;
using Terraria.ID;


namespace GemSwords.Projectiles
{
    public class SapphireProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Blizzard);
            AIType = ProjectileID.Blizzard;
            Projectile.DamageType = DamageClass.Melee;
        }
    }
}
