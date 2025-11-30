using Terraria.GameContent.ItemDropRules;

namespace TheBindingOfRarria.Content.Items;

public class BloodcourseMedallion : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 32;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 2);
        Item.expert = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<BloodcoursePlayer>().Veins = true;
    }
}

public class BloodcoursePlayer : ModPlayer
{
    public bool Veins = false;

    public override void NaturalLifeRegen(ref float regen)
    {
        if (Veins)
        {
            Player.lifeRegenTime += 1.5f;
            regen *= 1.5f;
        }
    }
}
public class CrateLootMedallion : GlobalItem
{
    public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
    {
        if (item.type == ItemID.JungleFishingCrateHard)
        {
            IItemDropRule rule = ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<BloodcourseMedallion>(), 5);
            itemLoot.Add(rule);
        }
    }
}