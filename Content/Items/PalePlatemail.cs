using TheBindingOfRarria.Content.Tiles;

namespace TheBindingOfRarria.Content.Items;

[AutoloadEquip(EquipType.Body)]
public class PalePlatemail : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetStaticDefaults()
    {
        ArmorIDs.Body.Sets.HidesTopSkin[Item.bodySlot] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 30;
        Item.defense = 6;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(0, 0, 70, 0);
    }

    public override void UpdateEquip(Player player)
    {

    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<PaleOre>(), 15)
            .AddTile(ModContent.TileType<TeslaStationTile>())
            .Register();
    }
}