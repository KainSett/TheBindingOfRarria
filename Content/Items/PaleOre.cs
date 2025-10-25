using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using TheBindingOfRarria.Content.Tiles;

namespace TheBindingOfRarria.Content.Items;

public class PaleOre : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<PaleOreTile>());
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.sellPrice(0, 0, 20);
        Item.rare = ItemRarityID.Green;
    }
}

public class CrateLootPaleOre : GlobalItem
{
    public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
    {
        if (item.type == ItemID.FrozenCrate || item.type == ItemID.FrozenCrateHard)
        {
            var rule = ItemDropRule.Common(ModContent.ItemType<PaleOre>(), 9, 2, 5);
            itemLoot.Add(rule);
        }
    }
}