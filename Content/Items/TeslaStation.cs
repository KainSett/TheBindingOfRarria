using TheBindingOfRarria.Content.Tiles;

namespace TheBindingOfRarria.Content.Items;

public class TeslaStation : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<TeslaStationTile>());
        Item.height = 32;
        Item.width = 32;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(0, 0, 13, 33);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Wire, 20)
            .AddIngredient(ItemID.IronBar, 6)
            .AddIngredient(ModContent.ItemType<Fulgurbloom>(), 10)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}