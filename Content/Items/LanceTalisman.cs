namespace TheBindingOfRarria.Content.Items;

public class LanceTalisman : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[ItemID.JoustingLance] = Type;
        ItemID.Sets.ShimmerTransformToItem[ItemID.ShadowJoustingLance] = Type;
        ItemID.Sets.ShimmerTransformToItem[ItemID.HallowJoustingLance] = Type;
    }

    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 38;
        Item.accessory = true;
        Item.rare = ItemRarityID.Gray;
        Item.value = Item.sellPrice(0, 0, 10);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.mount.Active)
        {
            player.statLifeMax2 += 50;
            player.statDefense += 5;
            player.GetDamage(DamageClass.Generic) += 0.1f;
        }
    }
}