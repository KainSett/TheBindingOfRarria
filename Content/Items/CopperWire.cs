namespace TheBindingOfRarria.Content.Items;

public class CopperWire : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 20;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(0, 0, 1);
        Item.maxStack = Item.CommonMaxStack;
    }

    public override void AddRecipes()
    {
        CreateRecipe(5)
            .AddIngredient(ItemID.CopperBar, 1)
            .AddIngredient(ItemID.Wire, 5)
            .AddTile(TileID.Anvils)
            .Register();
    }
}