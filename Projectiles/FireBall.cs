using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace GemSwords.Projectiles
{
    public class FireBall : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BallofFire);
            AIType = ProjectileID.BallofFire;
            Projectile.DamageType = DamageClass.Melee;
        }
    }
}
