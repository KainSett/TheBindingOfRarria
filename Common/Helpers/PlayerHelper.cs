using Terraria;
using Terraria.DataStructures;

namespace TheBindingOfRarria.Common.Helpers;

public static partial class Helper
{
    public static bool OwnsProjectile(this Player player, int type) =>
        player.ownedProjectileCounts[type] > 0;

    public static void SpawnProjectileIfNotSpawned(this Player player, int type, IEntitySource source, Vector2 position, int damage = 0)
    {
        Projectile proj = null;

        if (!player.OwnsProjectile(type) && Main.myPlayer == player.whoAmI)
            Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, 0, player.whoAmI);

        else
            proj = Main.ActiveProjectiles.Find(proj => proj.type == type && proj.active && proj.owner == player.whoAmI);

        if (proj != null)
        {
            proj.timeLeft = 5;
            proj.netUpdate = true;
        }
    }

    public static void SpawnProjectileIfNotSpawned(this Player player, int type, IEntitySource source, int damage = 0)
    {
        Projectile proj = null;

        if (!player.OwnsProjectile(type) && Main.myPlayer == player.whoAmI)
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, 0, player.whoAmI);

        else if (player.OwnsProjectile(type))
            proj = Main.ActiveProjectiles.Find(proj => proj.type == type && proj.active && proj.owner == player.whoAmI);

        if (proj != null)
        {
            proj.timeLeft = 5;
            proj.netUpdate = true;
        }
    }

    public static void SpawnProjectileIfNotSpawned(this Player player, int type, Vector2 position, IEntitySource source, int damage = 0)
    {
        Projectile proj = null;

        if (!player.OwnsProjectile(type) && Main.myPlayer == player.whoAmI)
            Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, 0, player.whoAmI);

        else
            proj = Main.ActiveProjectiles.Find(proj => proj.type == type && proj.active && proj.owner == player.whoAmI);

        if (proj != null)
        {
            proj.timeLeft = 5;
            proj.netUpdate = true;
        }
    }
}
