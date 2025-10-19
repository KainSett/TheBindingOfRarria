using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using TheBindingOfRarria.Common.Helpers;

namespace TheBindingOfRarria.Content.Projectiles;

public class HomingMissile : ModProjectile
{
    public override string Texture => ContentPath + "Dusts/PixellatedDustE98";

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.damage = 20;
        Projectile.timeLeft = 360;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Default;
        Projectile.OriginalArmorPenetration = 20;
        Projectile.ai[0] = -1;
    }

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation() + PiOver2;

        if (Projectile.timeLeft % 5 == 0 && (Projectile.ai[0] == -1 || !Main.npc[(int)Projectile.ai[0]].active))
        {
            var distanceSQ = 600 * 600f;
            foreach (var t in Main.npc) 
            {
                if (t.active && !t.friendly && t.lifeMax > 5 && t.CanBeChasedBy() && !t.immortal && !t.CountsAsACritter && t.Center.DistanceSQ(Projectile.Center) < distanceSQ)
                {
                    distanceSQ = t.DistanceSQ(Projectile.Center);
                    Projectile.ai[0] = t.whoAmI;
                }
            } 
        }

        if (Projectile.ai[0] == -1 || !Main.npc[(int)Projectile.ai[0]].active || Projectile.timeLeft > 345)
            return;

        Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.velocity.ToRotation().AngleLerp(Projectile.Center.DirectionTo(Main.npc[(int)Projectile.ai[0]].Center).ToRotation(), 0.12f) - Projectile.velocity.ToRotation());
    }

    public override bool? CanHitNPC(NPC target)
    {
        if (Projectile.timeLeft > 350)
            return false;

        return base.CanHitNPC(target);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        if (Projectile.ai[1] == 1)
            lightColor = Color.Purple;
        else if (Projectile.ai[1] == 2)
            lightColor = Color.Red;
        else if (Projectile.ai[1] == 3)
            lightColor = Color.Plum;


        Color color = lightColor;

        for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
            Main.spriteBatch.DrawPixellated(texture, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, null, Projectile.scale - 0.02f * i, Projectile.oldRot[i], texture.Size() / 2, color with { A = (byte)(150 - i * 5)}, Common.Systems.PixellationSystem.RenderType.Additive, Common.Systems.PixellationSystem.RenderLayer.Projectiles);

        Main.spriteBatch.DrawPixellated(texture, Projectile.Center - Main.screenPosition, null, Projectile.scale * 1.5f, Projectile.rotation, texture.Size() / 2, color with { A = 150 }, Common.Systems.PixellationSystem.RenderType.Additive, Common.Systems.PixellationSystem.RenderLayer.Projectiles);
        return false;
    }
}