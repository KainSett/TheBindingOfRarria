using System.Collections.Generic;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.ArtifactSets;

namespace TheBindingOfRarria.Content.Items;

public class Longclaw : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.height = 32;
        Item.width = 34;
        Item.value = Item.sellPrice(0, 6);
        Item.rare = ItemRarityID.Expert;
        Item.expert = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        var p = player.GetModPlayer<LongclawPlayer>();
        if (player.TryGetModPlayer<BeastPlayer>(out var plr) && plr.counter > 0)
        {
            p.pen = 12;
            p.Scale = 0.6f;
        }
        else
        {
            p.pen = 6;
            p.Scale = 0.3f;
        }

        player.GetArmorPenetration(DamageClass.Melee) += p.pen;
        p.SlayQueen = true;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
         tooltips.InsertArtifactSetBonusTooltip(Type);
    }
}

public class LongclawPlayer : ModPlayer
{
    public bool SlayQueen = false;

    public int pen = 6;

    public float Scale = 0.3f;

    public override void ResetEffects() => SlayQueen = false;

    public override void ModifyItemScale(Item item, ref float scale)
    {
        if (SlayQueen)
        {
            scale *= (1 + Scale);
        }
    }

}

public class LongclawItemNPCShop : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        if (shop.NpcType == NPCID.BestiaryGirl)
        {
            shop.Add(new Item(ModContent.ItemType<Longclaw>()), Condition.InExpertMode, Condition.DownedMechBossAny);
        }
    }
}