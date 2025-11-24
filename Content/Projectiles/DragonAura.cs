using System;
using System.Linq;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Common.Registries;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.Projectiles;

public class DragonAura : ModProjectile
{
    public override string Texture => ContentPath + "Projectiles/" + "Cleave";

    public override void SetDefaults()
    {
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.width = 50;
        Projectile.friendly = true;
        Projectile.height = 50;
        Projectile.timeLeft = 50;
        Projectile.scale = 0.1f;
    }

    public override void AI()
    {
        var owner = Main.player[Projectile.owner];
        Projectile.scale += 0.1f;
        Projectile.Resize((int)(50f * Projectile.scale), (int)(50f * Projectile.scale));
        Projectile.rotation += PiOver4 / 20 + Projectile.whoAmI % 10 * PI / 360;
        if (Projectile.timeLeft <= 20)
            Projectile.alpha += 10;

        
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        if (targetHitbox.Center().DistanceSQ(projHitbox.Center()) <= projHitbox.Width * projHitbox.Height)
            return base.Colliding(projHitbox, targetHitbox);
        
        return false;
    }

    public override bool? CanDamage()
    {
        if (Projectile.timeLeft > 15)
            return base.CanDamage();
        
        return false;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.HitDirectionOverride = Math.Sign(target.Center.X - Projectile.Center.X);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire3, 60);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var texture = Textures._light[0];

        var scale = Projectile.Size / texture.Size() * 1.33f;

        var color = (lightColor.MultiplyRGB(Color.Red)) with { A = (byte)(lightColor.A - Projectile.alpha) } * 0.75f;
        for (int i = 0; i < 2; i++)
            Main.spriteBatch.DrawPixellated(texture.Value, Projectile.Center - Main.screenPosition, null, scale, Projectile.rotation - i, texture.Size() / 2, color, Common.Systems.PixellationSystem.RenderType.Additive, Common.Systems.PixellationSystem.RenderLayer.Projectiles);

        texture = Textures._light[0];

        color = (lightColor.MultiplyRGB(Color.Orange)) with { A = (byte)(lightColor.A - Projectile.alpha) } * 0.75f;
        for (int i = 0; i < 2; i ++)
            Main.spriteBatch.DrawPixellated(texture.Value, Projectile.Center - Main.screenPosition, null, scale * 0.75f, -Projectile.rotation + i, texture.Size() / 2, color, Common.Systems.PixellationSystem.RenderType.Additive, Common.Systems.PixellationSystem.RenderLayer.Projectiles);

        texture = Textures._light[0];

        color = (lightColor.MultiplyRGB(Color.Yellow)) with { A = (byte)(lightColor.A - Projectile.alpha) };
        Main.spriteBatch.DrawPixellated(texture.Value, Projectile.Center - Main.screenPosition, null, scale * 0.33f, Projectile.rotation / 2, texture.Size() / 2, color, Common.Systems.PixellationSystem.RenderType.Additive, Common.Systems.PixellationSystem.RenderLayer.Projectiles);

        return false;
    }
}