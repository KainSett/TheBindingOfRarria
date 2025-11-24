using System;
using System.Collections.Generic;

namespace TheBindingOfRarria.Content.Projectiles;

public class MagmaShield : ModProjectile
{
    public override string Texture => ContentPath + "Projectiles/" + Name;

    public override void SetStaticDefaults() => Main.projFrames[Type] = 4;

    public override void SetDefaults()
    {
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.width = 72;
        Projectile.height = 72;
        Projectile.damage = 0;
        Projectile.netImportant = true;
        Projectile.timeLeft = 16;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        overPlayers.Add(index);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        int frame = Math.Max(0, 3 - Projectile.timeLeft / 4);
        Rectangle rect = texture.Frame(1, 4, 0, frame, 0, -2);

        Color color = Color.White;

        Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition - (Projectile.Size * 0.5f), rect, color with { A = 0 });

        return false;
    }
}