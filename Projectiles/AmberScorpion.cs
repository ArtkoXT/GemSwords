using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace GemSwords.Projectiles
{
    public class AmberScorpion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabySpider);
            AIType = ProjectileID.BabySpider;
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 3 * 60);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Color drawColor = Projectile.GetAlpha(lightColor);
            float rotation = Projectile.rotation * Projectile.spriteDirection;
            Vector2 origin = Projectile.spriteDirection == 1 ? new Vector2(15, frameHeight - 10) : new Vector2(texture.Width - 15, frameHeight - 10);
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, position, sourceRectangle, drawColor, rotation, origin, Projectile.scale, spriteEffects);
            return false;
        }
    }
}
