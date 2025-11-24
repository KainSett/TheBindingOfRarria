using System;
using System.Collections.Generic;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.Projectiles;

public class StalwartAura : ModProjectile
{
    public override string Texture => ContentPath + "Projectiles/" + Name;

    public override void SetDefaults()
    {
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.damage = 0;
        Projectile.netImportant = true;
        Projectile.timeLeft = 16;
        Projectile.alpha = 255;
        //Projectile.scale = 0.1f;
        Projectile.width = 52;
        Projectile.height = 52;
    }

    public override void AI()
    {
        Projectile.scale += 0.1f;
        Projectile.CenteredOnPlayer();
        var owner = Main.player[Projectile.owner];
        var p = owner.GetModPlayer<PalePlayer>();
        Projectile.alpha = (int)(255f * p.counter / 20f);
        if (p.counter > 20)
            Projectile.Kill();
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        //overPlayers.Add(index);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var owner = Main.player[Projectile.owner];
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        lightColor *= 0.33f * (255f - Projectile.alpha) / 255f;


        Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, owner.gfxOffY - owner.height / 4), null, lightColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

        return false;
    }
}