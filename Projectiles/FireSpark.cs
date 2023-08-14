using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace GemSwords.Projectiles
{
    public class FireSpark : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WandOfSparkingSpark);
            AIType = ProjectileID.WandOfSparkingSpark;
            Projectile.DamageType = DamageClass.Melee;
        }
    }
}
