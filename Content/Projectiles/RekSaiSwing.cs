namespace TheBindingOfRarria.Content.Projectiles;

public class RekSaiSwing : ModProjectile
{
    public override string Texture => ContentPath + "Projectiles/CEFist";

    public override void SetDefaults()
    {
        Projectile.width = 60;
        Projectile.height = 20;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.DamageType = DamageClass.Default;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 10;
    }

    public override void AI()
    {

    }
}