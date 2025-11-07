using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using System.Linq;
using System;

namespace TheBindingOfRarria.Content.Items;
/*
public class RestorativeLocket : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.height = 32;
        Item.width = 32;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 1, 60, 0);
        Item.expert = true;
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<RestorativeDebuffsLocketPlayer>().HasLocket = true;
    }
}

public partial class NurseItemsShop : GlobalNPC
{
    public void AddHealLocket(NPC npc, ref Item[] items)
    {
        if (npc.type == NPCID.Merchant && Main.expertMode && Main.npc.Any(target => target.type == NPCID.Nurse && target.Center.DistanceSQ(npc.Center) <= 800 * 800))
        {
            var index = Array.FindLastIndex(items, i => i is not null && (i.master || i.type == ItemID.IronAnvil || i.type == ItemID.LeadAnvil));
            if (index == -1)
                return;

            for (int i = items.Length - 1; i > index; i--)
            {
                items[i] = items[i - 1];
            }

            items[index + 1] = new Item(ModContent.ItemType<RestorativeLocket>());
        }
    }
}*/

public class RestorativeDebuffsLocketPlayer : ModPlayer
{
    public bool HasLocket = false;

    public override void UpdateLifeRegen()
    {
        if (!HasLocket)
            return;

        foreach (var target in Main.ActiveNPCs)
        {
            if (!target.friendly && target.lifeMax > 5 && target.CanBeChasedBy() && !target.immortal && !target.CountsAsACritter && target.lifeRegen < 0 && target.Center.DistanceSQ(Player.Center) < 700 * 700)
            {
                Player.lifeRegen += (int)float.Floor(-target.lifeRegen / 8f);
                if (Main.GlobalTimeWrappedHourly % 0.33f == 0)
                    Dust.NewDustPerfect(target.Center, DustID.VampireHeal, target.Center.DirectionTo(Player.Center) * 5);
            }
        }

        HasLocket = false;
    }
}