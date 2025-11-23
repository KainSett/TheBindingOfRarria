using System;
using System.Diagnostics.Metrics;
using Terraria.Audio;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Common.Registries;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Buffs.Debuffs;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.Projectiles;
/*public class BlackHoleAura : ModProjectile
{
    public override string Texture => Helper.GetVanillaExtraTexture(98);

    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 300;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Default;
        Projectile.damage = 10;
        Projectile.penetrate = -1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 30;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 3;
    }

    public override void AI()
    {
        Projectile.CenteredOnPlayer();
        Projectile.rotation += PiOver4 / 80;

    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = Textures._light[2].Value;

        Vector2 drawOrigin = new Vector2(texture.Width, texture.Height) * 0.5f;

        Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
        Color color = Color.Purple.MultiplyRGB(Projectile.GetAlpha(lightColor)) * 0.5f;

        Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation * 0.66f, drawOrigin, Projectile.scale * 0.9f, SpriteEffects.None, 0);

        texture = Textures._light[1].Value;
        color = Color.Black.MultiplyRGB(Projectile.GetAlpha(lightColor)) * 0.5f;

        Main.EntitySpriteDraw(texture, drawPos, null, color, -Projectile.rotation, drawOrigin, Projectile.scale * 0.75f, SpriteEffects.None, 0);
        Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale * 0.5f, SpriteEffects.None, 0);

        texture = Textures._light[0].Value;

        Main.EntitySpriteDraw(texture, drawPos, null, color, -Projectile.rotation, drawOrigin, Projectile.scale * 0.33f, SpriteEffects.None, 0);
        Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale * 0.15f, SpriteEffects.None, 0);

        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        var owner = Main.player[Projectile.owner];

        var power = 1;
        var duration = 600;
        owner.GetModPlayer<TemporaryLifePlayer>().bonuses.Add(new LifeBonus(power, duration, p => !p.GetModPlayer<BlackHolePlayer>().hunger, BlackHolePlayer.StatSource));

        if (target.active)
            target.AddBuff(ModContent.BuffType<Siphoned>(), duration);

        var percent = target.life / (float)target.lifeMax;

        target.lifeMax -= power;
        target.life = (int)Math.Floor(percent * target.lifeMax);

        target.GetGlobalNPC<SiphonedNPC>().SiphonedLife += power;

        target.velocity += target.Center.DirectionTo(Projectile.Center) * 3;
    }
}*/