using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Content.ArtifactSets;

namespace TheBindingOfRarria.Content.Items;

public class RobotCarcass : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.height = 32;
        Item.width = 32;
        Item.accessory = true;
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.sellPrice(0, 1, 60, 0);
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<RobotConversionPlayer>().Robot = true;
        player.statDefense += player.GetModPlayer<RobotConversionPlayer>().counter * 3;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.InsertArtifactSetBonusTooltip(Type);
    }
}

public class RobotItemShop : GlobalNPC
{
    public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
    {
        if (npc.type == NPCID.Merchant && NPC.downedMechBossAny)
        {
            var index = Array.FindLastIndex(items, i => i is not null && (i.expert || i.type == ItemID.IronAnvil || i.type == ItemID.LeadAnvil));
            if (index == -1)
                return;

            for (int i = items.Length - 1; i > index; i--)
            {
                items[i] = items[i - 1];
            }

            items[index + 1] = new Item(ModContent.ItemType<RobotCarcass>());
        }
    }
}

public class RobotConversionPlayer : ModPlayer
{
    public bool Robot = false;

    public int counter = 0;

    public int timer = 0;

    public override void ResetEffects()
    {
        Robot = false;
    }

    public override void Load()
    {
        On_Player.UpdateLifeRegen += On_Player_UpdateLifeRegen;
    }

    public override void Unload()
    {
        On_Player.UpdateLifeRegen -= On_Player_UpdateLifeRegen;
    }

    private static void On_Player_UpdateLifeRegen(On_Player.orig_UpdateLifeRegen orig, Player self)
    {
        orig(self);

        var p = self.GetModPlayer<RobotConversionPlayer>();
        if (p.Robot)
        {
            if (p.timer != 0 && p.counter != self.lifeRegen / 2)
            {
                p.timer--;
                return;
            }
            p.counter = self.lifeRegen / 2;
            p.timer = 60;
        }
    }

    public override void UpdateBadLifeRegen()
    {
        if (Robot)
            Player.lifeRegen -= counter / (Player.GetModPlayer<Android>().android ? 2 : 1);

        Player.GetModPlayer<Android>().android = false;
    }
}