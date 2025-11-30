using System.Collections.Generic;
using Terraria.Localization;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.Items;

/*public class MicroPlanet : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;
    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.width = 30;
        Item.height = 32;
        Item.defense = 3;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(0, 1, 11, 11);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.ownedProjectileCounts[ModContent.ProjectileType<OrbitalStar>()] < 3)
        {
            for (int i = 0; i < 6; i++)
                Projectile.NewProjectile(player.GetProjectileSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<OrbitalStar>(), 40, 1, ai1: i);
        }
    }
}*/