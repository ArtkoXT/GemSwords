using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using GemSwords.Items;

namespace GemSwords.GlobalProjectiles
{
    public class MeleeDamageConversion : GlobalProjectile
    {
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_Parent { Entity: Projectile tracker } && tracker.type == ModContent.ProjectileType<EmeraldSword.Tracker>() || Main.LocalPlayer.HeldItem.ModItem is EmeraldSword && projectile.type == ProjectileID.VilethornBase)
                projectile.DamageType = DamageClass.Melee;
            if ( Main.LocalPlayer.HeldItem.DamageType is MeleeDamageClass)
                projectile.DamageType = DamageClass.Melee;
        }
    }
}
