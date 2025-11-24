using Terraria.GameContent.ItemDropRules;

namespace TheBindingOfRarria.Content.Items;

public class LifeJuice : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 20;
        Item.rare = ItemRarityID.Lime;
        Item.value = Item.sellPrice(0, 0, 10);
        Item.maxStack = Item.CommonMaxStack;
    }
}

public class PlantJuiceDrop : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        if (npc.type == NPCID.Plantera)
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LifeJuice>(), minimumDropped: 5, maximumDropped: 10));
        else if (npc.type == NPCID.SpikedJungleSlime)
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.DownedPlantera(), ModContent.ItemType<LifeJuice>(), minimumDropped: 1, maximumDropped: 2));
    }
}