using System;
using Terraria.Audio;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Common.Registries;

namespace TheBindingOfRarria.Content.Projectiles;
public class PoisonAura : ModProjectile
{
    public override string Texture => Helper.GetVanillaExtraTexture(98);

    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 400;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Default;
        Projectile.damage = 10;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 2;
    }

    public override void AI()
    {
        Projectile.CenteredOnPlayer();
        Projectile.rotation += PiOver4 / 80;

        var targets = Array.FindAll(Main.npc, t => t.active && !t.friendly && t.Center.DistanceSQ(Projectile.Center) <= (Projectile.width / 2) * (Projectile.height / 2));
        foreach (var target in targets)
        {
            //target.AddBuff(BuffID.Poisoned, 60);
            target.lifeRegenCount -= 16;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = Textures._light[2].Value;

        Vector2 drawOrigin = new Vector2(texture.Width, texture.Height) * 0.5f;

        Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
        Color color = Color.LimeGreen.MultiplyRGB(Projectile.GetAlpha(lightColor)) * 0.33f;

        Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation * 0.66f, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

        texture = Textures._light[1].Value;

        Main.EntitySpriteDraw(texture, drawPos, null, color, -Projectile.rotation, drawOrigin, Projectile.scale * 0.75f, SpriteEffects.None, 0);

        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Poisoned, 60);
    }
}