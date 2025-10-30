using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.Items;

[AutoloadEquip(EquipType.Head)]
public class PlagueMask : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetStaticDefaults()
    {
        ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
    }

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.width = 38;
        Item.height = 36;
        Item.value = Item.sellPrice(0, 1, 30);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateEquip(Player player) => player.SpawnProjectileIfNotSpawned(ModContent.ProjectileType<PoisonAura>(), player.GetProjectileSource_Accessory(Item));

    //public override void UpdateAccessory(Player player, bool hideVisual) => player.SpawnProjectileIfNotSpawned(ModContent.ProjectileType<PoisonAura>(), player.GetProjectileSource_Accessory(Item));

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<SerpentsKiss>())
            .AddIngredient(ItemID.Leather, 7)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}
