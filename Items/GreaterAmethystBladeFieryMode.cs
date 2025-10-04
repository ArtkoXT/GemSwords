using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace GemSwords.Items
{
    public class GreaterAmethystBladeFieryMode : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 27;
            Projectile.tileCollide = false;
            Projectile.scale = 1.7f;
        }
        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Main.myPlayer == Projectile.owner)
                Projectile.netUpdate = true;
            Timer += 1;
            // Kill the projectile if it reaches it's intented lifetime
            if (Timer >= 14 / player.GetAttackSpeed(DamageClass.Melee))
            {
                Projectile.Kill();
                return;
            }
            player.heldProj = Projectile.whoAmI;

            Projectile.spriteDirection = player.direction;
            Projectile.Center = Projectile.Center;

            // Sets the angle for the projectile to stop rotating at
            float maxRot = (int)Projectile.ai[2] >= 1 ? MathHelper.ToRadians(-220) * Projectile.spriteDirection : MathHelper.ToRadians(280) * Projectile.spriteDirection;
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
            //float rotationfactorMult = (int)Projectile.ai[2] >= 1 ? (float)Math.PI / -2f * Projectile.spriteDirection : 0;
            //float rotationFactor = Projectile.rotation + rotationfactorMult; // The rotation of the Jousting Lance.
            //float scaleFactor = -200f * Projectile.spriteDirection; // How far back the hit-line will be from the tip of the Jousting Lance. You will need to modify this if you have a longer or shorter Jousting Lance. Vanilla uses 95f
            //float widthMultiplier = 23f; // How thick the hit-line is. Increase or decrease this value if your Jousting Lance is thicker or thinner. Vanilla uses 23f
            //float collisionPoint = 0f; // collisionPoint is needed for CheckAABBvLineCollision(), but it isn't used for our collision here. Keep it at 0f.

            //// This Rectangle is the width and height of the Jousting Lance's hitbox which is used for the first step of collision.
            //// You will need to modify the last two numbers if you have a bigger or smaller Jousting Lance.
            //// Vanilla uses (0, 0, 300, 300) which that is quite large for the size of the Jousting Lance.
            //// The size doesn't matter too much because this rectangle is only a basic check for the collision (the hit-line is much more important).
            //Rectangle lanceHitboxBounds = new Rectangle(0, 0, 300, 300);

            //// Set the position of the large rectangle.
            //lanceHitboxBounds.X = (int)Projectile.position.X - lanceHitboxBounds.Width / 2;
            //lanceHitboxBounds.Y = (int)Projectile.position.Y - lanceHitboxBounds.Height / 2;

            //// This is the back of the hit-line with Projectile.Center being the tip of the Jousting Lance.
            //Vector2 hitLineEnd = Projectile.Center + rotationFactor.ToRotationVector2() * scaleFactor;

            //// The following is for debugging the size of the hit line. This will allow you to easily see where it starts and ends.
            //Dust.NewDustPerfect(Projectile.Center, DustID.Pixie, Velocity: Vector2.Zero, Scale: 0.5f);
            //Dust.NewDustPerfect(hitLineEnd, DustID.Pixie, Velocity: Vector2.Zero, Scale: 0.5f);

            //// First check that our large rectangle intersects with the target hitbox.
            //// Then we check to see if a line from the tip of the Jousting Lance to the "end" of the lance intersects with the target hitbox.
            //if (lanceHitboxBounds.Intersects(targetHitbox)
            //    && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, hitLineEnd, widthMultiplier * Projectile.scale, ref collisionPoint))
            //{
            //    return true;
            //}
            //return false;
            Vector2 start = Main.player[Projectile.owner].MountedCenter;
            float projectileLength = (Projectile.Size.Length() - 60) * Projectile.spriteDirection;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((projectileLength * -1) * Projectile.scale);
            Dust.NewDustPerfect(end, DustID.Pixie, Velocity: Vector2.Zero, Scale: 0.5f);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
        }
        public override void CutTiles()
        {
            Vector2 start = Main.player[Projectile.owner].MountedCenter;
            float projectileLength = (Projectile.Size.Length() - 60) * Projectile.spriteDirection;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((projectileLength * -1) * Projectile.scale);
            Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 3 * 60); // Sets the hit enemy on fire for 3 seconds
                                                    // Adds 1 to fuel everytime you hit an npc
            if (Main.LocalPlayer.HeldItem.ModItem is GreaterAmethystBlade greaterAmethystBlade && greaterAmethystBlade.Fuel < 100)
                greaterAmethystBlade.Fuel++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner]; // Defines player
            int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 50, default, 0.9f);
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
