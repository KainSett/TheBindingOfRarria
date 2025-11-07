using TheBindingOfRarria.Content.Tiles;

namespace TheBindingOfRarria.Content.Items;

[AutoloadEquip(EquipType.Legs)]
public class PaleGreaves : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetStaticDefaults()
    {
        ArmorIDs.Legs.Sets.HidesBottomSkin[Item.legSlot] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 16;
        Item.defense = 11;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(0, 0, 30, 0);
    }

    public override void UpdateEquip(Player player)
    {
        player.aggro += 100;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<PaleOre>(), 7)
            .AddTile(ModContent.TileType<TeslaStationTile>())
            .Register();
    }
}