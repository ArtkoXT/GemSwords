using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace GemSwords.Projectiles
{
    public class RubyWave : ModProjectile
    {
        public int Timer = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
        }
        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.alpha = 0;
            Projectile.scale = 1.2f;
        }
        public override void AI()
        {

            Projectile.frame = 7;
            Projectile.alpha += 4;
            if (Projectile.alpha >= 255)
                Projectile.Kill();
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            if (Projectile.velocity.X < 0.0) 
                Projectile.spriteDirection = -1;
            if (++Timer >= 10)
            {
                Projectile.velocity = Projectile.velocity * 0.90f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 vel = oldVelocity*0.01f;
            Projectile.velocity = vel;
            return false;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = true;
            width = 10;
            height = 10;
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y-30), 50, 100, DustID.GemRuby, 0f, 0f, 50, default, 0.9f);
            Main.dust[dust].noGravity = true;
            return true;
        }
    }
}