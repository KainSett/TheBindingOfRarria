using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Common.Registries;
using TheBindingOfRarria.Common.Systems;

namespace TheBindingOfRarria.Content.Projectiles;

public class OrbitalStar : ModProjectile
{
    public override string Texture => Helper.GetVanillaItemTexture(ItemID.NebulaPickup2);

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.width = 32;
        Projectile.height = 32;
    }

    public override void AI()
    {
        float rotation = Main.GlobalTimeWrappedHourly * 2f + TwoPi / 6 * Projectile.ai[1];
        var r = 200;

        Projectile.OrbitingPlayer(0.08f, r, rotation);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Projectile.scale = 0.2f;
        var color = Color.White with { A = 250 };

        PixellationSystem.QueuePixellationAction(() =>
        {
            var texture = Textures.Muzzle[2];


            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (i > 4)
                    texture = Textures.Muzzle[1];
                color = Color.White.MultiplyRGBA(color.MultiplyRGB(Color.MediumPurple) with { A = (byte)(200 - i * 10) });
                Main.spriteBatch.Draw(texture.Value, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, color, Projectile.oldRot[i], texture.Size() * new Vector2(0.5f, 0.75f), Projectile.scale, SpriteEffects.None, 0);
            }
        }, PixellationSystem.RenderType.Additive, PixellationSystem.RenderLayer.Projectiles);

        return false;
    }
}