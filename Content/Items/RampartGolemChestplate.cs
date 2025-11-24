namespace TheBindingOfRarria.Content.Items;

[AutoloadEquip(EquipType.Body)]
public class RampartGolemChestplate : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetStaticDefaults()
    {
        ArmorIDs.Body.Sets.HidesTopSkin[Item.bodySlot] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 26;
        Item.defense = 25;
        Item.rare = ItemRarityID.Lime;
        Item.value = Item.sellPrice(0, 5, 0, 0);
    }

    public override void UpdateEquip(Player player)
    {
        player.GetDamage(DamageClass.Generic) += 0.10f;
        player.GetCritChance(DamageClass.Generic) += 10f;
    }
}